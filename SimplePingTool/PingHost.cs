using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;


//just making some notes
namespace SimplePingTool
{
    public class PingHost
    {
        //interval is based on milliseconds
        private int IntervalBetweenPings { get; set; }
        private string AddressOrIp { get; set; }
        public bool IsPingRunning { get; set; }

        //potential ping statuses
        public enum Status
        {
            ERROR,
            SUCCESS,
            FAILURE,
            TIMED_OUT,
        }

        public PingHost(string addressOrIp = "8.8.8.8",int intervalBetweenPings = 1000)
        {
            this.AddressOrIp = addressOrIp;
            this.IntervalBetweenPings = intervalBetweenPings;
            this.IsPingRunning = true;
        }

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
                //start ping
                PingReply reply = await sender.SendPingAsync(this.AddressOrIp);

                //log the ping reply status code
                result.StatusCode = reply.Status.GetHashCode();

                //check what the status of the ping reply is
                switch (reply.Status.GetHashCode())
                {
                    //Cases determine the result of the ping test and writes the appropriate error message per case.
                    //
                    //
                    case 0: //SUCCESS
                        result.Status = Status.SUCCESS.ToString();

                        //roundtrips less than 1ms are reported as 1 ms.
                        if (reply.RoundtripTime < 1)
                        {
                            result.Latency = 1;

                        } else
                        {
                            result.Latency = (int)reply.RoundtripTime;
                        }
                        break;

                    case 11001:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "The reply buffer was too small.";
                        break;
                    case 11002:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "The destination network was unreachable";
                        break;
                    case 11003:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "the destination host was unreachable";
                        break;
                    case 11004:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "The destination protocol was unreachable";
                        break;
                    case 11005:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "The destination port was unreachable";
                        break;
                    case 11006:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "Insufficient IP resources were available";
                        break;
                    case 11007:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "A bad IP Option was specified";
                        break;
                    case 11008:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "A hardware error occured";
                        break;
                    case 11009:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "The packet was too big";
                        break;
                    case 11010: //TIMEOUT
                        result.Status = Status.TIMED_OUT.ToString();
                        result.ErrorMessage = "Packet has timed out";
                        break;
                    case 11011:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "A bad request";
                        break;
                    case 11012:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "A bad route";
                        break;
                    case 11013:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "The TTL expired in transit";
                        break;
                    case 11014:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "The TTL expired during fragment reassembly";
                        break;
                    case 11015:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "a parameter problem";
                        break;
                    case 11016:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "Datagrams are arriving too fast to be processed and have been discarded";
                        break;
                    case 11017:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "An IP option was too big";
                        break;
                    case 11018:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "a bad destination";
                        break;
                    case 11050:
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "A general failure. This error can be returned for some malformed ICMP packets";
                        break;
                    default: //All other Errors, this shouldnt happen as all errors have been accounted for
                        result.Status = Status.ERROR.ToString();
                        result.ErrorMessage = "Unknown error";
                        break;
                }

            }
            catch (ArgumentNullException b)
            {
                //exception occured when address is NULL
                //Set result to ERROR;
                result.Status = Status.ERROR.ToString();
                result.ErrorMessage = b.InnerException.Message;
                result.AddressOrIp = b.Message;
            }
            catch (PingException b)
            {
                //exception occured, address is in incorrect format
                //Set result to ERROR
                result.Status = Status.ERROR.ToString();
                result.ErrorMessage = b.InnerException.Message;
                result.AddressOrIp = b.Message;
            }
            catch (Exception b)
            {
                //unknown error
                result.Status = Status.ERROR.ToString();
                result.ErrorMessage = b.InnerException.Message;
                result.AddressOrIp = b.Message;
            }
            finally
            {
                //if ping has error, set Latency to zero
                if (result.Status != Status.SUCCESS.ToString())
                {
                    result.Latency = 0;
                }

                //wait for the designated interval 
                await Task.Delay(IntervalBetweenPings);
                
                //release resource
                sender.Dispose();
            }
            //return the ping results
            return result;
        }
        //call to stop a ping thats contained within a loop
        public void StopPing()
        {
            this.IsPingRunning = false;
        }
    }
}
