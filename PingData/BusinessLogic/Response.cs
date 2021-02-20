using System;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace PingData.BusinessLogic
{
    public class Response : INotifyPropertyChanged
    {
        private int _latency;
        /// <summary>
        /// the latency in milliseconds for the ping to complete
        /// </summary>
        public int Latency
        {
            get
            {
                if (this.Status == StatusMessage.SUCCESS)
                    return _latency;
                else
                    return 0;
            }
            set
            {
                if (value < 1 && value >= 0)
                    _latency = 1;
                else
                    _latency = value;
            }
        }

        /// <summary>
        /// All possible ping status results
        /// </summary>
        public enum StatusMessage
        {
            ERROR = -1,
            SUCCESS = 0,
            FAILURE,
            TIMED_OUT,
        }

        /// <summary>
        /// The address or ip of the ping destination
        /// </summary>
        public string AddressOrIp { get; set; }

        /// <summary>
        /// The status associated with the completed ping request
        /// </summary>
        public StatusMessage Status { get; set; }

        /// <summary>
        /// the status code associated with the completed ping request
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// the error message associated to the ping request, if any
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The timestamp of when the ping request was sent
        /// </summary>
        public DateTime TimeStamp { get; set; }

        public void AddReplyException(Exception e)
        {
            //Exception error exists

            this.Status = StatusMessage.ERROR;
            this.Latency = 0;

            if (e.InnerException != null)
            {
               // this.AddressOrIp = e.Message;
                this.ErrorMessage = e.InnerException.Message;
                this.StatusCode = e.InnerException.GetHashCode();

            }
            else
            {
                this.ErrorMessage = e.Message;
                this.StatusCode = (int)StatusMessage.ERROR;
            }
        }

        public void AddReplyData(PingReply reply)
        {
            this.StatusCode = reply.Status.GetHashCode();


            if (reply.Status == IPStatus.Success) //Successful ping
            {
                this.Status = StatusMessage.SUCCESS;
                this.Latency = (int)reply.RoundtripTime;
            }
            else //failed ping
            {
                int errorLatency = 0;
                bool errorIsKnown = PingStatus.Message.ContainsKey(this.StatusCode);

                this.Status = StatusMessage.ERROR;
                this.Latency = errorLatency;
                this.ErrorMessage = errorIsKnown ? PingStatus.Message[this.StatusCode] : PingStatus.Message[-1];
            }

        }

        public override string ToString()
        {
            string txtFileLine;

            if (this.StatusCode == 0) //Ping Successful
            {
                txtFileLine = $"{this.TimeStamp}\t Host: {this.AddressOrIp} Status: {this.Status} Latency: {this.Latency} ms";
            }
            else //Ping Failed 
            {
                txtFileLine = $"{this.TimeStamp}\t Error: {this.AddressOrIp} Status: {this.Status} Error Code: {this.StatusCode} Error Message:{this.ErrorMessage}";
            }
            return txtFileLine;
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
