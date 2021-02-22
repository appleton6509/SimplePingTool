using PingData.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PingData.BusinessLogic
{
    public class Settings : INotifyPropertyChanged
    {
        private bool _isPingNotRunning = true;
        private bool _isPingRunning = false;
        private bool _showConfiguration;
        private bool _isLoggingEnabled;

        public string SelectedLogFilePath
        {
            get
            {
                return LogUtil.Path;
            }
            set
            {
                try
                {
                    LogUtil.Path = value;
                    OnPropertyChanged();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
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
                OnPropertyChanged();
            }
        }

        public bool IsLoggingEnabled
        {
            get { return _isLoggingEnabled; }
            set { 
                _isLoggingEnabled = value;
                OnPropertyChanged();
            }
        }

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
                if (_isPingNotRunning == value)
                    return;

                _isPingNotRunning = value;
                IsPingRunning = !value;
                OnPropertyChanged();
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
                if (_isPingRunning == value)
                {
                    return;
                }
                _isPingRunning = value;
                IsPingNotRunning = !value;
                OnPropertyChanged();

            }
        }

        /// <summary>
        /// Maintains a bindable list of default Ping locations
        /// </summary>
        public ObservableCollection<string> AddressList { get; set; } = new ObservableCollection<string>()
        {

        };


        #region INotifyPropertyChanged

        /// <summary>
        /// Event when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged
    }
}
