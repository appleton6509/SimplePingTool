using PingData;
using PingData.BusinessLogic;
using PingData.Utils;
using SimplePingTool.HelperClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SimplePingTool.ViewModel
{
    public class PingViewModel : BaseViewModel
    {

        #region Public Binding Properties
        public Settings Settings { get; set; } = new Settings();
        public ChartData ChartData { get; set; } = new ChartData();
        /// <summary>
        /// A collection for storing ping results
        /// </summary>
        public ObservableCollection<Response> PingResultsList { get; set; } = new ObservableCollection<Response>();
        /// <summary>
        /// a collection of ping stats
        /// </summary>
        public Statistic Stats { get; set; } = new Statistic();
        /// <summary>
        /// Instantiates a ping for sending and receiving ping results
        /// </summary>
        public PingHost Ping { get; set; } = new PingHost();
        /// <summary>
        /// The selected ping interval in millisecond
        /// </summary>
        public double SelectedMillisecond
        {
            get
            {
                return (double)Ping.IntervalBetweenPings;
            }
            set
            {
                if (value > 0 && !Int16.Equals(value, Ping.IntervalBetweenPings))
                    Ping.IntervalBetweenPings = (int)value;
            }
        }
        /// <summary>
        /// A Live Charts property that converts a value to calculate memory size and returns it
        /// </summary>
        public Func<double, string> ToPercentageFormatter { get; set; } = value =>
        {
            return String.Concat(value, "%");
        };
        #endregion Public Properties

        #region ICommands 

        public ICommand StartPingCommand { get; set; }
        public ICommand StopPingCommand { get; set; }

        #endregion ICommands

        public PingViewModel()
        {
            Settings.IsPingNotRunning = true;
            Settings.AddressList = new ObservableCollection<string>()
            {
                "8.8.8.8",
                "8.8.4.4",
                "www.yahoo.com",
                "www.youtube.com",
            };

            StartPingCommand = new RelayCommand<object>(StartPing);
            StopPingCommand = new RelayCommand<object>(StopPing);
            PingResultsList.CollectionChanged += PingResultsList_CollectionChanged;
        }

        #region Private Methods

        /// <summary>
        /// Starts a Ping Request
        /// </summary>
        /// <param name="obj"></param>
        private async void StartPing(object obj = null)
        {
            if(IsErrorPresent())
            {
                Settings.IsPingRunning = false;
                return;
            }
            ClearPreviousPingResults();
            while (Settings.IsPingRunning && !IsErrorPresent())
                await StartPingTask();
        }

        private bool IsErrorPresent()
        {
            if (Ping.IDataErrors.Count > 0)
                return true;
            return false;
        }

        /// <summary>
        /// Stops a currently running Ping Request
        /// </summary>
        /// <param name="param"></param>
        private void StopPing(object param = null)
        {
            Settings.IsPingRunning = false;
        }

        /// <summary>
        /// Start an async ping
        /// </summary>
        /// <returns></returns>
        private async Task StartPingTask()
        {
            Response pingResult = await Ping.StartPingAsync();
            PingResultsList.Add(pingResult);
        }

        private void ClearPreviousPingResults()
        {
            PingResultsList.Clear();
            Stats.SetDefaults();
            ChartData.Clear();
        }

        /// <summary>
        /// Log ping results to a file
        /// </summary>
        /// <param name="pingResult">new PingResult to append to the file</param>
        private void LogToFile(Response pingResult)
        {
            if (Settings.IsLoggingEnabled)
                LogUtil.LogToTextFile(pingResult);
        }

        private void PerformTasksOnPingResult(Response newPingResult)
        {
            Stats.Add(newPingResult);
            ChartData.Add(newPingResult);
            LogToFile(newPingResult);
        }
        #endregion Private Methods

        #region Events

        /// <summary>
        /// Event fires when a new ping result is received and added to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PingResultsList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action.Equals(NotifyCollectionChangedAction.Add))
            {
                var newPingResult = ((ObservableCollection<Response>)sender)[e.NewStartingIndex];
                PerformTasksOnPingResult(newPingResult);
            }
        }


        #endregion

    }
}
