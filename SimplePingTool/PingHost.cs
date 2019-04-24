using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace SimplePingTool
{
    public class PingHost
    {
        private int IntervalBetweenPings { get; set; }
        private string AddressOrIp { get; set; }

        Ping sender = new Ping();
        
        public enum Status
        {
            ERROR,
        }

        public PingHost(string addressOrIp = "8.8.8.8",int intervalBetweenPings = 1000)
        {
            this.AddressOrIp = addressOrIp;
            this.IntervalBetweenPings = intervalBetweenPings;
        }

        public async Task<PingResult> StartPingAsync()
        {
            PingResult result = new PingResult()
            {
                TimeStamp = DateTime.Now
            };
            PingReply reply;

            try
            {
                //start ping
                reply = await sender.SendPingAsync(AddressOrIp);

                //Ping completed, store results
                result.Status = reply.Status.ToString();
                result.AddressOrIp = AddressOrIp;

                if (reply.RoundtripTime == 0)
                {
                    result.Latency = "<1";
                } else
                {
                    result.Latency = reply.RoundtripTime.ToString();
                }
            }
            catch (ArgumentNullException)
            {
                //exception occured when address is NULL
                //Set result to ERROR
                result.Status = Status.ERROR.ToString() ;
                result.AddressOrIp = "Address is empty";

            }
            catch (PingException b)
            {
                //exception occured, address is in incorrect format
                //Set result to ERROR
                result.Status = Status.ERROR.ToString();
                result.AddressOrIp = "Address not found";
                Console.WriteLine(b.InnerException);
            }
            catch
            {
                //unknown error
                result.Status = Status.ERROR.ToString();
                result.AddressOrIp = "Unknown Error";

            }
            finally
            {
                //If ping fails due to issue in ping transit or exception error, report latency as "0"
                if (result.Latency == null)
                {
                    result.Latency = "0";

                }

                //wait for the designated interval 
                await Task.Delay(IntervalBetweenPings);
                
                //release resource
                sender.Dispose();
            }
            //return the ping results
            return result;
        }
    }
}
