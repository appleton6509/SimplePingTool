using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using PingData;
using System.Collections.ObjectModel;
using SimplePingTool.ModelView;

namespace SimplePingTool
{
    /// <summary>
    /// Main Application of the Simple Ping Tool
    /// </summary>
    /// test
    public partial class MainWindow : Window
    {
        #region Properties 
        private bool isPingRunning { get; set; } = false;
        private PingHost Ping { get; set; } = new PingHost();

        public PingStats Stats { get; set; } = new PingStats();

        private PingLog PingLogger { get; set; } = new PingLog();

        public ObservableCollection<PingResult> PingResultsList = new ObservableCollection<PingResult>();

        public ObservableCollection<string> AddressList { get; set; } = new ObservableCollection<string>()
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


            cbAddressOrIp.ItemsSource = AddressList;

            dgPingResults.ItemsSource = PingResultsList;

            //update ping results on changes
            PingResultsList.CollectionChanged  += PingResultsList_CollectionChanged;

            //update ping stats when changes occur
            Stats.StatsChanged += Stats_StatsChanged;
        }


        #endregion Constructor

        #region Helper Methods

        private void ClearPingResultDataUI()
        {
            //clear data
            PingResultsList.Clear();
            Stats.Clear();

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
            //Disable stop button
            btnStopPing.IsEnabled = false;

            //update text on start button and enable it
            btnStartPing.Content = LabelText.START;
            btnStartPing.IsEnabled = true;

            //enable textbox and combobox when running
            tbAddressOrIp.IsEnabled = true;
            cbAddressOrIp.IsEnabled = true;

        }

        /// <summary>
        /// Update Ping Stats UI
        /// </summary>
        /// <param name="list"></param>
        private void UpdatePingStatsUI()
        {
           lbAverageLatency.Content = Stats.AverageLatency;
            lbMaxLatency.Content = Stats.MaxLatency;
            lbPacketsLost.Content = Stats.PacketsLost;
            lbPacketsSent.Content = Stats.PacketsSent;
        }

        private void HandleNewPingResult(PingResult newPingResult)
        {

            //update ping stats
            Stats.Add(newPingResult);

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
                PingLogger.LogToTextFile(pingResult);
        }

        /// <summary>
        /// Start an async ping
        /// </summary>
        /// <returns></returns>
        private async Task StartPingTask()
        {
            //ping host and log results
            PingResult pingResult = await Ping.StartPingAsync();

            //discard ping results for longer intervals if ping was manually stopped by user
            if (isPingRunning)
            {
                //add results to ping result list
                PingResultsList.Add(pingResult);
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
            if (!Validator.IsValidAddress(tbAddressOrIp.Text))
            {
                return;
            }
            else
            {
                Ping.AddressOrIp = tbAddressOrIp.Text;
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

        private void TbAddressOrIp_GotFocus(object sender, RoutedEventArgs e)
        {
            //when focused on this textbox field, clear the field.
            TextBox tbAddress = (TextBox)sender;
            tbAddress.Text = string.Empty;
        }

        private void slInterval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            Slider sliderIntervalValue = (Slider)sender;
            int intervalInMilliseconds = (int)sliderIntervalValue.Value * 1000;

            //update pinghost interval
            Ping.IntervalBetweenPings = intervalInMilliseconds;
        }

        private void Stats_StatsChanged(object sender, EventArgs e)
        {
            UpdatePingStatsUI();
        }

        private void PingResultsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if ((sender as ObservableCollection<PingResult>).Count > 0)
            {
                foreach (PingResult newPing in e.NewItems)
                {
                    HandleNewPingResult(newPing);
                }
            }
        }

        #endregion Event Handlers
    }
}
