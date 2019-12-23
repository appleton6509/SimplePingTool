using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ComponentModel;
using PingData;

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

        private bool isPingRunning = false;
        private readonly PingHost pingHost = new PingHost();
        BindingList<PingResult> pingResultsList = new BindingList<PingResult>();
        private readonly PingStats stats = new PingStats();
        public PingLog logging = new PingLog();


        //TODO implement and test regex expressions
        Regex ValidIpAddressRegex = new Regex("^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");

        Regex ValidHostnameRegex = new Regex("^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\\-]*[a-zA-Z0-9])\\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\\-]*[A-Za-z0-9])$");



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

            //bind combo box list to address list
            cbAddressOrIp.ItemsSource = addressOrIpList;

            //update ping results on changes
            pingResultsList.ListChanged += PingResultsList_ListChanged;

            //update ping stats when changes occur
            stats.StatsChanged += Stats_StatsChanged;
        }



        #endregion Constructor

        #region Helper Methods

        private bool ValidateAddress()
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
            pingResultsList.Clear();
            stats.Clear();

        }
        //update UI before ping is started
        private void PingRunningControlAccess()
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
        private void PingNotRunningControlAccess()
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

        /// <summary>
        /// Update Ping Stats UI
        /// </summary>
        /// <param name="list"></param>
        private void UpdatePingStatsUI()
        {
            if (stats.PacketsSent == 0)
            {
                //Clear "Ping Stat" labels
                string pingStatsLblTag = "stats";

                grdPingStats.Children.OfType<Label>()
                    .Where(label => (string)label.Tag == pingStatsLblTag)
                    .Select(label => label.Content = LabelText.NA);
            }

            lbAverageLatency.Content = stats.AverageLatency;
            lbMaxLatency.Content = stats.MaxLatency;
            lbPacketsLost.Content = stats.PacketsLost;
            lbPacketsSent.Content = stats.PacketsSent;
        }

        private void HandleNewPingResult(PingResult newPingResult)
        {
            //update datagrid
            dgPingResults.Items.Add(newPingResult);

            //update ping stats
            stats.Add(newPingResult);

            //log to file
            LogToFile(newPingResult);
        }

        /// <summary>
        /// Log ping results to a file
        /// </summary>
        /// <param name="pingResult">new PingResult to append to the file</param>
        private void LogToFile(PingResult pingResult)
        {
            bool isLoggingEnabled = (bool)chbEnableLogging.IsChecked;

            if (isLoggingEnabled)
                logging.LogToTextFile(pingResult);
        }

        /// <summary>
        /// Start an async ping
        /// </summary>
        /// <returns></returns>
        private async Task StartPingTask()
        {
            //ping host and log results
            PingResult pingResult = await pingHost.StartPingAsync();

            //discard ping results for longer intervals if ping was manually stopped by user
            if (isPingRunning)
            {
                //add results to ping result list
                pingResultsList.Add(pingResult);
            }
        }

        #endregion Helper Methods

        #region Event Handlers

        private void BtnStopPing_Click(object sender, RoutedEventArgs e)
        {
            isPingRunning = false;

            //Post Ping UI/Var Routine
            PingNotRunningControlAccess();
        }

        private async void BtnStartPing_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateAddress())
            {
                e.Handled = true;
            }

            //Pre-Ping UI/Var routine
            PingRunningControlAccess();

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


        private void PingResultsList_ListChanged(object sender, ListChangedEventArgs e)
        {
            BindingList<PingResult> list = (BindingList<PingResult>)sender;
            PingResult newPingResult = list[e.NewIndex];

            if (list.Count > 0)
                HandleNewPingResult(newPingResult);
        }

        private void Stats_StatsChanged(object sender, EventArgs e)
        {
            UpdatePingStatsUI();
        }

        #endregion Event Handlers
    }
}
