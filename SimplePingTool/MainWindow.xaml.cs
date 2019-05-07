using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace SimplePingTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string AddressOrIp { get; set; }
        private int PingInterval { get; set; }
        private PingHost pingHost;
        public Log logging;

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

        public MainWindow()
        {
            InitializeComponent();
            SetDefaults();
        }
        //Set default values and 
        private void SetDefaults()
        {
   
            //Populate combo box items
            cbAddressOrIp.ItemsSource = addressOrIpList;

            //Generate Labels
            lbAverageLatency.Content = LabelText.NA;
            lbPacketsLost.Content = LabelText.NA;
            lbPacketsSent.Content = LabelText.NA;
            lbMaxLatency.Content = LabelText.NA;

            //instantiate var for logging
            logging = new Log();
        }

        private void ResetUIandData()
        {
            //clear data
            dgPingResults.Items.Clear();
            results.Clear();

            //Clear "Ping Stat" labels
            lbAverageLatency.Content = LabelText.NA;
            lbPacketsLost.Content = LabelText.NA;
            lbPacketsSent.Content = LabelText.NA;
            lbMaxLatency.Content = LabelText.NA;

        }
        //update UI before ping is started
        private void BeforePingRoutine()
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
            ResetUIandData();
        }

        private void UpdatePingStats(PingResult pingResults)
        {

            results.Add(pingResults);

            //Update UI for "Ping Stats"
            lbAverageLatency.Content = Math.Round(results.Average(x => x.Latency));
            lbPacketsSent.Content = results.Count;

            //ignore errors in the event no packets succeed
            try
            {
                lbMaxLatency.Content = results
                .Where(x => x.Status == LabelText.SUCCESS.ToString())
                .Max(x => x.Latency);
            }
            catch { }

            lbPacketsLost.Content = results
                .Count(x => x.Status != LabelText.SUCCESS.ToString()).ToString();
            
        }

        //update UI after stop button pressed
        private void AfterPingRoutine()
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

        private void BtnStopPing_Click(object sender, RoutedEventArgs e)
        {
            pingHost.IsPingRunning = false;

            //Post Ping UI/Var Routine
            AfterPingRoutine();
        }

        private async void BtnStartPing_Click(object sender, RoutedEventArgs e)
        {
            AddressOrIp = tbAddressOrIp.Text;

            //check an address or ip has been entered
            if (AddressOrIp == null || AddressOrIp == "")
            {
                MessageBox.Show("Address field cannot be empty.", "Error");
            }

            //Begin ping
            else
            {
                
                //Pre-Ping UI/Var routine
                BeforePingRoutine();

                pingHost = new PingHost(AddressOrIp);

                do
                {
                    if (!pingHost.IsPingRunning)
                    {
                        break;
                    }
                    //check for interval changes
                    pingHost.IntervalBetweenPings = (int)slInterval.Value * 1000;
                    
                    //ping address
                    PingResult pingResult = await pingHost.StartPingAsync();

                    //add results to datagrid
                    dgPingResults.Items.Insert(0, pingResult);

                    //Update "Ping Stats" details
                    UpdatePingStats(pingResult);

                    //log to file if selected
                    if ((bool)chbEnableLogging.IsChecked)
                    {
                        logging.LogToTextFile(pingResult);
                    }

                } while (pingHost.IsPingRunning);
            }
        }

        private void TbAddressOrIp_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CbAddressOrIp_DropDownClosed(object sender, EventArgs e)
        {
            //update addressOrIP var and text box when dropdown is closed.
            var addressSelection = (ComboBox)sender;
            tbAddressOrIp.Text = addressSelection.Text;
        }

        private void TbAddressOrIp_GotFocus(object sender, RoutedEventArgs e)
        {
            //when focused on this textbox field, clear the field.
            var tbAddress = (TextBox)sender;
            tbAddress.Text = null;
        }

        private void TbAddressOrIp_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tbAddress = (TextBox)sender;
            AddressOrIp = tbAddress.Text;
        }
    }
}
