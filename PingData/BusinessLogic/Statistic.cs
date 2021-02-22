 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PingData.BusinessLogic
{
    public class Statistic : INotifyPropertyChanged
    {

        #region Private Members
        private List<int> _successfulPings = new List<int>();
        private int _packetsSent;
        private int _maxLatency;
        private int _packetsLost;
        #endregion
        #region Public Properties
        /// <summary>
        /// calculated average latency
        /// </summary>
        public double AverageLatency
        {
            get
            {
                return GetAverageLatency();
            }
        }
        /// <summary>
        /// Total number of packets sent within the results list 
        /// </summary>
        public int PacketsSent
        {
            get { return _packetsSent; }
            set
            {
                _packetsSent = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Max latency value found within the results list
        /// </summary>
        public int MaxLatency
        {
            get { return _maxLatency; }
            set
            {
                _maxLatency = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Total number of packets lost within the results list
        /// </summary>
        public int PacketsLost
        {
            get { return _packetsLost; }
            set
            {
                _packetsLost = value;
                OnPropertyChanged();
            }

        }
        /// <summary>
        /// Returns the success rate(percentage) of all ping results.
        /// </summary>
        public double SuccessfulPingRate
        {
            get
            {
                return GetSuccessfulPingRate();
            }
        }
        #endregion
        public Statistic()
        {
            SetDefaults();
        }

        /// <summary>
        /// adds a new ping results to the stats
        /// </summary>
        /// <param name="result"></param>
        public void Add(Response result)
        {
            if (String.IsNullOrEmpty(result.AddressOrIp))
                return;


            if (result.Status == Response.StatusMessage.SUCCESS)
            {
                SetAllLatencyProperties(result);
                SetSuccessfulPing(result);
            }
            else 
                _packetsLost++;

            SetPacketsSent();
        }

        private void SetPacketsSent()
        {
            PacketsSent++;
            OnPropertyChanged(nameof(SuccessfulPingRate));
        }

        private void SetSuccessfulPing(Response result)
        {
            _successfulPings.Add(result.Latency);
            OnPropertyChanged(nameof(AverageLatency));
        }

        private void SetAllLatencyProperties(Response result)
        {
            if (result.Latency > _maxLatency)
                MaxLatency = result.Latency;

            if (result.Latency < 1)
                result.Latency = 1;
        }

        /// <summary>
        /// Clears all ping results and properties
        /// </summary>
        public void SetDefaults()
        {
            PacketsLost = 0;
            PacketsSent = 0;
            MaxLatency = 0;
        }

        private double GetSuccessfulPingRate()
        {
            int success = _successfulPings.Count;
            int total = _packetsSent;
            if (_packetsSent > 0 && _successfulPings.Count > 0)
            {
                double result = ((success / total) * 100);
                return Math.Round(result);
            }

            else
                return 0;
        }

        private double GetAverageLatency()
        {
            return (_successfulPings.Count == 0) ? 0 : Math.Round(_successfulPings.Average());
        }

        #region INotifyPropertyChanged

        /// <summary>
        /// Event when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged
    }
}
