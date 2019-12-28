using PingData;
using SimplePingTool.Helper_Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public bool IsPingNotRunning { 
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

        public bool IsPingRunning { 
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

        private enum LabelText
        {
            START,
            STARTED,
            NA,
            SUCCESS,
        }

        public ICommand StartPingCommand { get; set; }
        public ICommand StopPingCommand { get; set; }

        public PingHost Ping { get; set; }

        public ObservableCollection<PingResult> PingResultsList { get; set; } 

        public PingStats Stats { get; set; }

        public ObservableCollection<string> AddressList { get; set; } = new ObservableCollection<string>()
        {
            "8.8.8.8",
            "8.8.4.4",
            "www.yahoo.com",
            "www.youtube.com",
        };
     

        public MainWindowModelView()
        {
            IsPingNotRunning = true;

            StartPingCommand = new RelayCommand<object>(StartPing);
            StopPingCommand = new RelayCommand<object>(StopPing);

            Ping = new PingHost();
            Stats = new PingStats();
            PingResultsList = new ObservableCollection<PingResult>();
        }

        #region ICommand Methods

        public async void StartPing(object obj = null)
        {
            //Ping is already running, return.
            if (IsPingRunning || Ping.IDataErrors.Count > 0)
                return;

            ClearResultData();

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

            //discard ping results for longer intervals if ping was manually stopped by user
            if (IsPingRunning)
            {
                //add results to ping result list
                PingResultsList.Add(pingResult);

                //update ping stats
                Stats.Add(pingResult);

                //log to file
                LogToFile(pingResult);
            }
        }

        private void ClearResultData()
        {
            //clear data
            PingResultsList.Clear();
            Stats.Clear();
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
