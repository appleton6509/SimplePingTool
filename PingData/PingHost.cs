using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;


//just making some notes
namespace PingData
{
    public class PingHost
    {
        /// <summary>
        /// Interval between ping replies in milliseconds
        /// </summary>
        private int _IntervalBetweenPings;
        /// <summary>
        /// Interval between ping replies in seconds
        /// </summary>
        public int IntervalBetweenPings
        {
            get
            {
                return _IntervalBetweenPings / 1000;
            }
            set {
                _IntervalBetweenPings = value * 1000;
            }
        }

        /// <summary>
        /// DNS or IP of host to ping
        /// </summary>
        public string AddressOrIp { get; set; }

        /// <summary>
        /// List of status codes and definitions returns from a ping reply
        /// </summary>
        private Dictionary<int, string> ErrorCodes { get; } = new Dictionary<int, string>()
        {
            { -1, "Unknown Error" },
            { 0, "SUCCESS" },
            { 11001, "The reply buffer was too small." },
            { 11002, "The destination network was unreachable" },
            { 11003, "the destination host was unreachable" },
            { 11004, "The destination protocol was unreachable" },
            { 11005, "The destination port was unreachable" },
            { 11006, "Insufficient IP resources were available" },
            { 11007, "A bad IP Option was specified" },
            { 11008, "A hardware error occured" },
            { 11009, "The packet was too big" },
            { 11010, "Packet has timed out" },
            { 11011, "A bad request" },
            { 11012, "A bad route" },
            { 11013, "The TTL expired in transit" },
            { 11014, "The TTL expired during fragment reassembly" },
            { 11015, "a parameter problem" },
            { 11016, "Datagrams are arriving too fast to be processed and have been discarded" },
            { 11017, "An IP option was too big" },
            { 11018, "a bad destination" },
            { 11050, "A general failure. This error can be returned for some malformed ICMP packets" },
        };

        /// <summary>
        /// All possible ping status results
        /// </summary>
        public enum Status
        {
            ERROR,
            SUCCESS,
            FAILURE,
            TIMED_OUT,
        }
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addressOrIp">DNS or IP address of host to ping</param>
        /// <param name="intervalBetweenPings">Interval in milliseconds between pings</param>
        public PingHost(string addressOrIp = "8.8.8.8", int intervalBetweenPings = 1000)
        {
            this.AddressOrIp = addressOrIp;
            this._IntervalBetweenPings = intervalBetweenPings;
        }
        public PingHost() {
            this._IntervalBetweenPings = 1000;
        }
        #endregion Constructor

        #region Public Methods
        public async Task<PingResult> StartPingAsync()
        {
            Ping sender = new Ping();

            PingResult result = new PingResult()
            {
                TimeStamp = DateTime.Now,
                AddressOrIp = this.AddressOrIp
            };


            try
            {
                
                PingReply reply = await sender.SendPingAsync(this.AddressOrIp);
                result = GetStatus(result, reply);
                result = GetLatency(result, reply);
            }
            catch (ArgumentNullException b)
            {
                //exception occured when address is NULL
                //Set result to ERROR;
                result.Status = Status.ERROR;
                result.ErrorMessage = b.InnerException.Message;
                result.AddressOrIp = b.Message;
            }
            catch (PingException b)
            {
                //exception occured, address is in incorrect format
                //Set result to ERROR
                result.Status = Status.ERROR;
                result.ErrorMessage = b.InnerException.Message;
                result.AddressOrIp = b.Message;
            }
            catch (Exception b)
            {
                //unknown error
                result.Status = Status.ERROR;
                result.ErrorMessage = b.InnerException.Message;
                result.AddressOrIp = b.Message;
            }
            finally
            {
                //wait for the designated interval 
                await Task.Delay(_IntervalBetweenPings);

                //release resource
                sender.Dispose();
            }
            //return the ping results
            return result;
        }
        #endregion Public Methods


        #region Private Methods
        /// <summary>
        /// Returns the latency from a ping reply object
        /// </summary>
        /// <param name="result"></param>
        /// <param name="reply"></param>
        /// <returns></returns>
        private PingResult GetLatency(PingResult result, PingReply reply)
        {

            int successCode = 0;

            if (reply.Status.GetHashCode() != successCode) //ping was not successfull
            {
                result.Latency = 0;
            } else
            {
                if (reply.RoundtripTime < 1)           //roundtrips less than 1ms are reported as 1 ms.
                {
                    result.Latency = 1;

                }
                else
                {
                    result.Latency = (int)reply.RoundtripTime;
                }
            }

            return result;

        }

        /// <summary>
        /// Returns the ping status from a pingreply object
        /// </summary>
        /// <param name="result"></param>
        /// <param name="reply"></param>
        /// <returns></returns>
        private PingResult GetStatus(PingResult result, PingReply reply)
        {
            //log the ping reply status code
            result.StatusCode = reply.Status.GetHashCode();


            int successCode = 0;
            if (result.StatusCode == successCode) //Successful ping
            {
                result.Status = Status.SUCCESS;

            }
            else if (ErrorCodes.ContainsKey(result.StatusCode)) //Ping encountered an error
            {
                result.Status = Status.ERROR;
                result.ErrorMessage = ErrorCodes[result.StatusCode];
            }
            else // Ping encountered and unknown error
            {
                result.Status = Status.ERROR;
                result.ErrorMessage = ErrorCodes[-1];
            }

            return result;
        }
        #endregion Private Methods
    }
}
