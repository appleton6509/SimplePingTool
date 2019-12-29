using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PingData
{
    public class PingStats : INotifyPropertyChanged
    {

        private List<int> _successfulPings = new List<int>();

        /// <summary>
        /// calculated average latency
        /// </summary>
        public double AverageLatency { 
            get {
                if (_successfulPings.Count == 0)
                {
                    return 0;
                }

                else
                {
                    if (_successfulPings.Count > 0)    //if Successful pings are found, return average 
                    {
                        return Math.Round(_successfulPings.Average());
                    }
                    else        //no successful pings, return 0
                        return 0;
                }
            } 
        }
        
        /// <summary>
        /// Total number of packets sent within the results list 
        /// </summary>
        public int PacketsSent { 
            get { return _packetsSent; } 
            set 
            {
                _packetsSent = value;
                OnPropertyChanged();
            }  
        }
        private int _packetsSent;


        /// <summary>
        /// Max latency value found within the results list
        /// </summary>
        public int MaxLatency { 
            get { return _maxLatency; } 
            set 
            {
                _maxLatency = value;
                OnPropertyChanged();
            } 
        }
        private int _maxLatency;

        /// <summary>
        /// Total number of packets lost within the results list
        /// </summary>
        public int PacketsLost { 
            get { return _packetsLost; } 
            set
            {
                _packetsLost = value;
                OnPropertyChanged();
            }
        
        }
        private int _packetsLost;

        public PingStats()
        {
            _packetsLost = 0;
            _packetsSent = 0;
            _maxLatency = 0;
        }

        /// <summary>
        /// adds a new ping results to the stats
        /// </summary>
        /// <param name="result"></param>
        public void Add(PingResult result)
        {
            
            if (result.AddressOrIp == null)         //packet has no host, return and do nothing
                return;
            
            
            if (result.Status == PingHost.Status.SUCCESS)       
            {
                //On successful packet send, determine if max latency increased
                if (result.Latency > _maxLatency)
                {
                    MaxLatency = result.Latency;
                }
                //Latency is less then zero, set to zero
                if (result.Latency < 0)
                {
                    result.Latency = 0;
                }
            }
            else // errors or time outs increase packets lost value
            {
                _packetsLost++;
            }

            PacketsSent++;

            if (result.Status == PingHost.Status.SUCCESS)
                _successfulPings.Add(result.Latency);

            OnPropertyChanged(nameof(AverageLatency));
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
