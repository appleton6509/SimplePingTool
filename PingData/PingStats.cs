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
        /// <summary>
        /// A list for populating Ping Statistics
        /// </summary>
        readonly List<PingResult> Results = new List<PingResult>();

        /// <summary>
        /// calculated average latency
        /// </summary>
        public double AverageLatency { 
            get {
                if (Results.Count == 0)
                {
                    return 0;
                }

                else
                {
                    List<PingResult> found = Results.FindAll(x => x.Status == PingHost.Status.SUCCESS);

                    if (found.Count > 0)    //if Successful pings are found, return average 
                    {
                        return Math.Round(found.Average(x => x.Latency));
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
                    _maxLatency = result.Latency;
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

            Results.Add(result);

            OnPropertyChanged("AverageLatency");

            OnStatsChange(EventArgs.Empty);
        }

        /// <summary>
        /// Clears all ping results and properties
        /// </summary>
        public void Clear()
        {
            Results.Clear();
            PacketsLost = 0;
            PacketsSent = 0;
            MaxLatency = 0;

            OnStatsChange(EventArgs.Empty);
        }

        /// <summary>
        /// Event when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Event handler when stats have changed
        /// </summary>
        public event EventHandler StatsChanged;
        protected virtual void OnStatsChange(EventArgs e)
        {
            StatsChanged?.Invoke(this, e);
        } 
    }
}
