using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using PingData;
using SimplePingTool.Helper_Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimplePingTool.ModelView
{

    //TODO: Validation for user input

    public class MainWindowModelView : INotifyPropertyChanged
    {


        public bool IsPingNotRunning
        {
            get
            {
                return _isPingNotRunning;
            }
            set
            {
                if (_isPingNotRunning == value)
                    return;

                _isPingNotRunning = value;
                IsPingRunning = !value;
                OnPropertyChange();
            }
        }
        private bool _isPingNotRunning;

        public bool IsPingRunning
        {
            get
            {
                return _isPingRunning;
            }
            set
            {
                if (_isPingRunning == value)
                    return;

                _isPingRunning = value;
                IsPingNotRunning = !value;
                OnPropertyChange();
            }
        }
        private bool _isPingRunning;

        public bool IsLoggingEnabled { get; set; }


        public ObservableCollection<string> AddressList { get; set; } = new ObservableCollection<string>()
        {
            "8.8.8.8",
            "8.8.4.4",
            "www.yahoo.com",
            "www.youtube.com",
        };

        #region ICommands 

        public ICommand StartPingCommand { get; set; }
        public ICommand StopPingCommand { get; set; }

        #endregion ICommands

        #region Model Objects

        public PingHost Ping { get; set; } = new PingHost();

        public ObservableCollection<PingResult> PingResultsList { get; set; } = new ObservableCollection<PingResult>();

        public ChartValues<double> SuccessfulPing { get; set; } = new ChartValues<double>();

        public ChartValues<double> FailedPing { get; set; } = new ChartValues<double>();


        public PingStats Stats { get; set; } = new PingStats();

        #endregion Model Objects


        #region Chart Collections

        #endregion Chart Collections


        public MainWindowModelView()
        {
            IsPingNotRunning = true;

            StartPingCommand = new RelayCommand<object>(StartPing);
            StopPingCommand = new RelayCommand<object>(StopPing);

            PingResultsList.CollectionChanged += PingResultsList_CollectionChanged;
        }

        private void PingResultsList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
           if (e.Action  == NotifyCollectionChangedAction.Add)
            {
                var newPingResult = ((ObservableCollection<PingResult>)sender)[e.NewStartingIndex];

                Stats.Add(newPingResult);

                if (newPingResult.Status == PingHost.Status.SUCCESS)
                {
                    SuccessfulPing.Add(newPingResult.Latency);
                    FailedPing.Add(0);
                }

                else
                {
                    FailedPing.Add(10);
                    SuccessfulPing.Add(0);
                }
                //log to file
                LogToFile(newPingResult);

            }

        }


        #region ICommand Methods

        public async void StartPing(object obj = null)
        {
            //Ping is already running, return.
            if (IsPingRunning || Ping.IDataErrors.Count > 0)
                return;

            //ClearResults();

            IsPingRunning = true;

            while (IsPingRunning)
            {
                await StartPingTask();
            }
        }

        private void StopPing(object param = null)
        {
            IsPingRunning = false;

        }

        #endregion ICommand Methods

        #region Private Methods


        /// <summary>
        /// Start an async ping
        /// </summary>
        /// <returns></returns>
        private async Task StartPingTask()
        {
            //ping host and log results
            PingResult pingResult = await Ping.StartPingAsync();

            if (IsPingRunning)
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
        #endregion Private Methods

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChange([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


    }
}
