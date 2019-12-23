using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingData
{
    public class PingStats
    {
        readonly List<PingResult> Results = new List<PingResult>();

        public double AverageLatency { 
            get {
                if (Results.Count == 0)
                    return 0;
                else 
                    return Math.Round(Results.Average(x => x.Latency)); 
            } 
        }

        private int _packetsSent;
        public int PacketsSent { get { return _packetsSent; }  }

        private int _maxLatency;
        public int MaxLatency { get { return _maxLatency; } }

        private int _packetsLost;
        public int PacketsLost { get { return _packetsLost; } }

        public PingStats()
        {
            _packetsLost = 0;
            _packetsSent = 0;
            _maxLatency = 0;
        }

        public void Add(PingResult result)
        {
            Results.Add(result);

            _packetsSent++;

            if (result.Latency > _maxLatency)
            {
                _maxLatency = result.Latency;
            }

            if (result.Status != PingHost.Status.SUCCESS.ToString())
            {
                _packetsLost++;
            }

            OnStatsChange(EventArgs.Empty);
        }
        public void Clear()
        {
            Results.Clear();
            _packetsLost = 0;
            _packetsSent = 0;
            _maxLatency = 0;

            OnStatsChange(EventArgs.Empty);
        }

        public event EventHandler StatsChanged;

        protected virtual void OnStatsChange(EventArgs e)
        {
            StatsChanged?.Invoke(this, e);
        } 
    }
}
