 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PingData
{
    public class Statistic : INotifyPropertyChanged
    {

        #region Private Members
        private List<int> _successfulPings = new List<int>();
        private int _packetsSent;
        private int _maxLatency;
        private int _packetsLost;
        #endregion


        /// <summary>
        /// calculated average latency
        /// </summary>
        public double AverageLatency
        {
            get
            {

                if (_successfulPings.Count == 0)
                    return 0;
                else
                    return Math.Round(_successfulPings.Average());
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
        }

        public Statistic()
        {
            _packetsLost = 0;
            _packetsSent = 0;
            _maxLatency = 0;
        }

        /// <summary>
        /// adds a new ping results to the stats
        /// </summary>
        /// <param name="result"></param>
        public void Add(Response result)
        {

            if (String.IsNullOrEmpty(result.AddressOrIp))
                return;

            if (result.Status.Equals(Response.StatusMessage.SUCCESS))
            {
                //On successful packet send, determine if max latency increased
                if (result.Latency > _maxLatency)
                    MaxLatency = result.Latency;

                //Latency is less then 1, set to zero
                if (result.Latency < 1)
                    result.Latency = 1;

                _successfulPings.Add(result.Latency);
                OnPropertyChanged(nameof(AverageLatency));
            }
            else // errors or time outs increase packets lost value
                _packetsLost++;

            PacketsSent++;
            OnPropertyChanged(nameof(SuccessfulPingRate));
        }

        /// <summary>
        /// Clears all ping results and properties
        /// </summary>
        public void Clear()
        {
            PacketsLost = 0;
            PacketsSent = 0;
            MaxLatency = 0;
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
