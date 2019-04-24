using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimplePingTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string AddressOrIp { get; set; }
        private int PingInterval { get; set; }
        private bool IsPingRunning { get; set; }
        private string LogPath { get; set; }
        

        List<int> latencyData = new List<int>();
        List<string> pingStatusData = new List<string>();

        List<string> addressOrIpList = new List<string>()
        {
            "8.8.8.8",
            "8.8.4.4",
            "www.yahoo.com",
            "www.youtube.com",
        };

        enum LabelText
        {
            Start,
            Started,
            NA,
            Success,
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

            //Sets a default Log Path
            LogPath = "C:/PingResult/Log.txt";
            tbLogFilePath.Text = LogPath;
        }

        //update UI before ping is started
        private void BeforePingRoutine()
        {
            //Tell UI Ping is running
            IsPingRunning = true;

            //clear datagrid before starting
            dgPingResults.Items.Clear();

            //Enable stop ping btn
            btnStopPing.IsEnabled = true;

            //update text on start button text and disable it
            btnStartPing.Content = LabelText.Started;
            btnStartPing.IsEnabled = false;

            //disable textbox and combobox when running
            tbAddressOrIp.IsEnabled = false;
            cbAddressOrIp.IsEnabled = false;

            //Clear "Ping Stat" vars and labels
            latencyData.Clear();
            pingStatusData.Clear();
            lbAverageLatency.Content = LabelText.NA;
            lbPacketsLost.Content = LabelText.NA;
            lbPacketsSent.Content = LabelText.NA;
            lbMaxLatency.Content = LabelText.NA;
        }

        //Update Ping Stat labels
        private void UpdatePingStats(PingResult pingResult) {

            //store latency 
            //
            //checks if latency was set to less than 1
            if (pingResult.Latency.Contains("<1"))
            {
                latencyData.Add(1);
            } else
            {
                latencyData.Add(int.Parse(pingResult.Latency));
            }
            
            //Store status data
            pingStatusData.Add(pingResult.Status);

            //calculate "Ping Stats"
            lbAverageLatency.Content = Math.Round(latencyData.Average());
            lbPacketsSent.Content = latencyData.Count;
            lbMaxLatency.Content = latencyData.Max();

            //calculates total "failed" pings
            int countFailedPings = 0;
            foreach (string status in pingStatusData)
            {
                if (status != LabelText.Success.ToString())
                {
                    countFailedPings++;
                }

            }
            lbPacketsLost.Content = countFailedPings;
        }

        //update UI after stop button pressed
        private void AfterPingRoutine()
        {
            //enable textbox and combobox when running
            tbAddressOrIp.IsEnabled = true;
            cbAddressOrIp.IsEnabled = true;

            //update text on start button and enable it
            btnStartPing.Content = LabelText.Start;
            btnStartPing.IsEnabled = true;

            //Disable stop button
            btnStopPing.IsEnabled = false;

        }

        private void BtnStopPing_Click(object sender, RoutedEventArgs e)
        {
            IsPingRunning = false;

            //Post Ping UI/Var Routine
            AfterPingRoutine();
        }

        private async void BtnStartPing_Click(object sender, RoutedEventArgs e)
        {

            //check an address or ip has been entered
            if (AddressOrIp == null)
            {
                MessageBox.Show("Address field cannot be empty.", "Error");
            }

            //Begin ping
            else
            {
                //Pre-Ping UI/Var routine
                BeforePingRoutine();

                do
                {
                    //ping address
                    PingHost pingHost = new PingHost(AddressOrIp, 1000);
                    PingResult pingResult = await pingHost.StartPingAsync();

                    //add results to datagrid
                    dgPingResults.Items.Insert(0, pingResult);
                                       
                    //Update "Ping Stats" details
                    UpdatePingStats(pingResult);

                } while (IsPingRunning);
            }
        }

        private void CbAddressOrIp_DropDownClosed(object sender, EventArgs e)
        {
            //update addressOrIP var and text box when dropdown is closed.
            var addressSelection = (ComboBox)sender;
            AddressOrIp = addressSelection.Text;
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
