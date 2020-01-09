using LiveCharts;
using LiveCharts.Wpf;
using PingData;
using ITBox.HelperClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ITBox.ViewModel
{
    public class PingViewModel : BaseViewModel
    {

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
        private bool _isPingNotRunning = true;

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

        private bool _isPingRunning = false;

        private bool _showControlConfiguration = false;
        public bool ShowControlConfiguration
        {
            get
            {
                return _showControlConfiguration;
            }
            set
            {
                _showControlConfiguration = value;
                RaisePropertyChange();
            }
        }

        /// <summary>
        /// Indicates if logging is currently enabled
        /// </summary>
        public bool IsLoggingEnabled { get; set; }

        /// <summary>
        /// Returns the success rate(percentage) of all ping results
        /// </summary>
        public double SuccessfulPingRate { 
            
            get
            {
                double success = PingResultsList.Count(x => x.Status == PingHost.Status.SUCCESS);
                double total = PingResultsList.Count;

                if (total > 0 && success > 0)
                {
                    double result = ((success / total)*100);
                    return Math.Round(result);
                }
                    
                else
                    return 0;

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
        /// a collection for successful ping results stored as a latency value
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

            int zeroMillisecond = 0;
            int tenMillisecond = 10;

            if (newPingResult.Status.Equals(PingHost.Status.SUCCESS))
            {
                SuccessfulPing.Add(newPingResult.Latency);
                FailedPing.Add(zeroMillisecond);
            }

            else
            {
                FailedPing.Add(tenMillisecond);
                SuccessfulPing.Add(zeroMillisecond);
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

                RaisePropertyChange(nameof(SuccessfulPingRate));

                //log to file
                LogToFile(newPingResult);

            }

        }

        #endregion

    }
}
