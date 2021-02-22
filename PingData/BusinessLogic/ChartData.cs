using LiveCharts;

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
            Success.Add(GetSuccessValue(response));
            Failures.Add(GetFailureValue(response));
            
        }

        private int GetSuccessValue(Response response)
        {
            bool pingFailed = !response.Status.Equals(Response.StatusMessage.SUCCESS);
            int successValue;
            if (pingFailed)
                successValue = (int)DisplayLatency.HIDE;
            else
                successValue = response.Latency;
            return successValue;
        }

        private int GetFailureValue(Response response)
        {
            bool pingFailed = !response.Status.Equals(Response.StatusMessage.SUCCESS);
            int failedValue;
            if (pingFailed)
                failedValue = (int)DisplayLatency.SHOW;
            else
                failedValue = (int)DisplayLatency.HIDE;
            return failedValue;
        }


        public void Clear()
        {  
            Failures.Clear();
            Success.Clear();
        }

    }
}
