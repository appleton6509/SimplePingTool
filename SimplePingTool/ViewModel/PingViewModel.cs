using LiveCharts;
using LiveCharts.Wpf;
using PingData;
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
        #region Private Members
        private bool _isPingNotRunning = true;
        private bool _isPingRunning = false;
        private bool _showConfiguration = false;
        #endregion

        #region Public Binding Properties
        /// <summary>
        /// Indicates if a Ping is NOT currently running. 
        /// </summary>
        public bool IsPingNotRunning
        {
            get
            {
                return _isPingNotRunning;
            }
            set
            {
                if (_isPingNotRunning == value || Ping.IDataErrors.Count > 0)
                    return;

                _isPingNotRunning = value;
                IsPingRunning = !value;
                RaisePropertyChange();
            }
        }


        /// <summary>
        /// Indicates if a ping is currently running
        /// </summary>
        public bool IsPingRunning
        {
            get
            {
                return _isPingRunning;
            }
            set
            {
                if (_isPingRunning == value || Ping.IDataErrors.Count > 0)
                {
                    return;
                }
                _isPingRunning = value;
                IsPingNotRunning = !value;
                RaisePropertyChange();

            }
        }

        /// <summary>
        /// Bool value indicating whether to show or not the configuration.
        /// </summary>
        public bool ShowConfiguration
        {
            get
            {
                return _showConfiguration;
            }
            set
            {
                _showConfiguration = value;
                RaisePropertyChange();
            }
        }

        /// <summary>
        /// Bool value that returns true if logging is enabled
        /// </summary>
        public bool IsLoggingEnabled { get; set; }

        /// <summary>
        /// Returns the success rate(percentage) of all ping results.
        /// </summary>
        public double SuccessfulPingRate {

            get
            {
                double success = PingResultsList.Count(x => x.Status == PingResult.StatusMessage.SUCCESS);
                double total = PingResultsList.Count;

                if (total > 0 && success > 0)
                {
                    double result = ((success / total) * 100);
                    return Math.Round(result);
                }

                else
                    return 0;
            }
        }

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
        /// The chosen path to save the log file to
        /// </summary>
        public string SelectedLogFilePath { 
            get
            {
                return LogPingResult.Path;
            }
            set
            {
                try
                {
                    LogPingResult.Path = value;
                } catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        } 


        /// <summary>
        /// A Live Charts property that converts a value to calculate memory size and returns it
        /// </summary>
        public Func<double, string> ToPercentageFormatter { get; set; } = value =>
        {
            return String.Concat(value, "%");
        };


        /// <summary>
        /// Maintains a bindable list of default Ping locations
        /// </summary>
        public ObservableCollection<string> AddressList { get; set; } = new ObservableCollection<string>()
        {
            "8.8.8.8",
            "8.8.4.4",
            "www.yahoo.com",
            "www.youtube.com",
        };

        /// <summary>
        /// A collection for storing ping results
        /// </summary>
        public ObservableCollection<PingResult> PingResultsList { get; set; } = new ObservableCollection<PingResult>();

        /// <summary>
        /// a collection for successful ping results stored as its latency value
        /// </summary>
        public ChartValues<double> SuccessfulPing { get; set; } = new ChartValues<double>();

        /// <summary>
        /// a collection for Failed ping results stored as a latency value
        /// </summary>
        public ChartValues<double> FailedPing { get; set; } = new ChartValues<double>();

        /// <summary>
        /// a collection of ping stats
        /// </summary>
        public PingStats Stats { get; set; } = new PingStats();

        /// <summary>
        /// Instantiates a ping for sending and receiving ping results
        /// </summary>
        public PingHost Ping { get; set; } = new PingHost();

        #endregion Public Properties

        #region ICommands 

        public ICommand StartPingCommand { get; set; }
        public ICommand StopPingCommand { get; set; }

        #endregion ICommands

        public PingViewModel()
        {
            IsPingNotRunning = true;

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
            if (Ping.IDataErrors.Count > 0)
            {
                IsPingRunning = false;
                return;
            }

            ClearResults();

            while (IsPingRunning)
            {
                await StartPingTask();
            }
        }

        /// <summary>
        /// Stops a currently running Ping Request
        /// </summary>
        /// <param name="param"></param>
        private void StopPing(object param = null)
        {
            IsPingRunning = false;
        }


        /// <summary>
        /// Start an async ping
        /// </summary>
        /// <returns></returns>
        private async Task StartPingTask()
        {
            //ping host and log results
            PingResult pingResult = await Ping.StartPingAsync();

            if ((bool)IsPingRunning)
                PingResultsList.Add(pingResult);
        }

        private void ClearResults()
        {
            //clear data
            PingResultsList.Clear();
            Stats.Clear();
            SuccessfulPing.Clear();
            FailedPing.Clear();
        }

        /// <summary>
        /// Log ping results to a file
        /// </summary>
        /// <param name="pingResult">new PingResult to append to the file</param>
        private void LogToFile(PingResult pingResult)
        {
            if (IsLoggingEnabled)
                LogPingResult.LogToTextFile(pingResult);
        }

        /// <summary>
        /// Updates all collections when a new ping result is received
        /// </summary>
        /// <param name="newPingResult"></param>
        private void UpdatePingResultCollections(PingResult newPingResult)
        {


            Stats.Add(newPingResult);

            RaisePropertyChange(nameof(SuccessfulPingRate)); //Notify UI of ping rate change

            //set chart plot point to zero millisecond, effectively hiding failedping results when ping successful 
            int chartHidePingLatency = 0;
            //set chart plot point to 10 millisecond, effectively displaying a failed ping result on the chart.
            int chartShowFailedPing = 10; 

            if (newPingResult.Status.Equals(PingResult.StatusMessage.SUCCESS))
            {
                SuccessfulPing.Add(newPingResult.Latency);
                FailedPing.Add(chartHidePingLatency);
            }

            else 
            {
                FailedPing.Add(chartShowFailedPing);
                SuccessfulPing.Add(chartHidePingLatency);
            }
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
                var newPingResult = ((ObservableCollection<PingResult>)sender)[e.NewStartingIndex];

                UpdatePingResultCollections(newPingResult);

                //log to file
                LogToFile(newPingResult);

            }

        }

        #endregion

    }
}
