using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Wpf;

namespace PingData.BusinessLogic
{
    public class ChartData
    {
        private enum DisplayLatency
        {
            SHOW = 10,         //used to set chart plot point to 10 millisecond, effectively displaying a failed ping result on the chart.
            HIDE = 0         //used to set chart plot point to zero millisecond, effectively hiding failedping results when ping successful 
        }
        /// <summary>
        /// a collection for successful ping results stored as its latency value
        /// </summary>
        public ChartValues<double> Success { get; } = new ChartValues<double>();

        /// <summary>
        /// a collection for Failed ping results stored as a latency value
        /// </summary>
        public ChartValues<double> Failures { get; } = new ChartValues<double>();

        public void Add(Response response)
        {
            int successValue = response.Latency;
            int failedValue = (int)DisplayLatency.HIDE;
            bool pingFailed = !response.Status.Equals(Response.StatusMessage.SUCCESS);
            if (pingFailed)
            {
                successValue = (int)DisplayLatency.HIDE;
                failedValue = (int)DisplayLatency.SHOW;
            }
            Success.Add(successValue);
            Failures.Add(failedValue);
        }

        public void Clear()
        {
            
            Failures.Clear();
            Success.Clear();
        }

    }
}
