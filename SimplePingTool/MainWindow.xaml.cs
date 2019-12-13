using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SimplePingTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// test
    public partial class MainWindow : Window
    {
        #region Properties 
        private string AddressOrIp { get; set; }
        private readonly PingHost pingHost = new PingHost();
        private bool isPingRunning = false;
        public Log logging = new Log();


        //TODO test regex expressions
        Regex ValidIpAddressRegex = new Regex("^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");

        Regex ValidHostnameRegex = new Regex("^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\\-]*[a-zA-Z0-9])\\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\\-]*[A-Za-z0-9])$");

        public List<PingResult> results = new List<PingResult>();
        public List<string> addressOrIpList = new List<string>()
        {
            "8.8.8.8",
            "8.8.4.4",
            "www.yahoo.com",
            "www.youtube.com",
        };

        enum LabelText
        {
            START,
            STARTED,
            NA,
            SUCCESS,
        }
        #endregion Properties

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            cbAddressOrIp.ItemsSource = addressOrIpList;
        }

        #endregion Constructor

        #region Helper Methods

        private bool validateIpAddress()
        {
            //check an address or ip has been entered
            if (AddressOrIp == null || AddressOrIp == string.Empty)
            {
                MessageBox.Show("Address field cannot be empty.", "Error");
                return false;
            }
            return true;
        }

        private void ClearPingResultDataUI()
        {
            //clear data
            dgPingResults.Items.Clear();
            results.Clear();

            //Clear "Ping Stat" labels
            string pingStatsLblTag = "stats";

            grdPingStats.Children.OfType<Label>()
                .Where(label => (string)label.Tag == pingStatsLblTag)
                .Select(label => label.Content = LabelText.NA);
        }
        //update UI before ping is started
        private void PrePingUpdateControls()
        {
            //Enable stop ping btn
            btnStopPing.IsEnabled = true;

            //update text on start button text and disable it
            btnStartPing.Content = LabelText.STARTED;
            btnStartPing.IsEnabled = false;

            //disable textbox and combobox when running
            tbAddressOrIp.IsEnabled = false;
            cbAddressOrIp.IsEnabled = false;

            //Resets UI labels and grids/ reset vars.
        }

        //update UI after stop button pressed
        private void PostPingUpdateControls()
        {
            //enable textbox and combobox when running
            tbAddressOrIp.IsEnabled = true;
            cbAddressOrIp.IsEnabled = true;

            //update text on start button and enable it
            btnStartPing.Content = LabelText.START;
            btnStartPing.IsEnabled = true;

            //Disable stop button
            btnStopPing.IsEnabled = false;

        }

        private void UpdatePingStatsUI(PingResult pingResults)
        {

            //add results to ping list for populating ping stats
            results.Add(pingResults);

            //Update UI for "Ping Stats"
            double averageLatency = Math.Round(results.Average(x => x.Latency));
            lbAverageLatency.Content = averageLatency;
            lbPacketsSent.Content = results.Count;

            //ignore errors in the event no packets succeed
            try
            {
                lbMaxLatency.Content = results
                .Where(x => x.Status == LabelText.SUCCESS.ToString())
                .Max(x => x.Latency);
            }
            catch { /*ignore*/ }

            lbPacketsLost.Content = results
                .Count(x => x.Status != LabelText.SUCCESS.ToString()).ToString();

        }

        private async Task StartPingTask()
        {
            bool isLoggingEnabled = (bool)chbEnableLogging.IsChecked;

            //ping host and log results
            PingResult pingResult = await pingHost.StartPingAsync();

            if (isPingRunning)
            {
                //add results to datagrid
                dgPingResults.Items.Insert(0, pingResult);

                UpdatePingStatsUI(pingResult);

                if (isLoggingEnabled)
                    logging.LogToTextFile(pingResult);
            }
        }

        #endregion Helper Methods

        #region Event Handlers

        private void BtnStopPing_Click(object sender, RoutedEventArgs e)
        {
            isPingRunning = false;

            //Post Ping UI/Var Routine
            PostPingUpdateControls();
        }

        private async void BtnStartPing_Click(object sender, RoutedEventArgs e)
        {
            if (!validateIpAddress())
            {
                e.Handled = true;
            }

            //Pre-Ping UI/Var routine
            PrePingUpdateControls();

            ClearPingResultDataUI();

            isPingRunning = true;
            while (isPingRunning)
            {
                await StartPingTask();
            }
        }

        private void CbAddressOrIp_DropDownClosed(object sender, EventArgs e)
        {
            //update addressOrIP var and text box when dropdown is closed.
            ComboBox addressSelection = (ComboBox)sender;
            tbAddressOrIp.Text = addressSelection.Text;
        }

        private void TbAddressOrIp_GotFocus(object sender, RoutedEventArgs e)
        {
            //when focused on this textbox field, clear the field.
            TextBox tbAddress = (TextBox)sender;
            tbAddress.Text = string.Empty;
        }

        private void TbAddressOrIp_TextChanged(object sender, TextChangedEventArgs e)
        {

            //TODO add regex validation and error providor
            TextBox tbAddress = (TextBox)sender;
            AddressOrIp = tbAddress.Text;

            pingHost.AddressOrIp = AddressOrIp;
        }

        private void slInterval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            Slider sliderIntervalValue = (Slider)sender;
            int intervalInMilliseconds = (int)sliderIntervalValue.Value * 1000;

            if (sliderIntervalValue.IsLoaded)
            {
                tbIntervalValue.Text = sliderIntervalValue.Value.ToString();
            }

            //update pinghost interval
            pingHost.IntervalBetweenPings = intervalInMilliseconds;
        }

        #endregion Event Handlers
    }
}
