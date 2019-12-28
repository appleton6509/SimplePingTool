using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace PingData
{
    public static class Validator
    {
        /// <summary>
        /// A regex value for validating IP address format
        /// </summary
        private static Regex IpAddressRegex = new Regex("^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");

        /// <summary>
        /// a regex value for validating DNS format
        /// </summary>
        private static Regex HostNameRegex = new Regex("^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\\-]*[a-zA-Z0-9])\\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\\-]*[A-Za-z0-9])$");

        public static readonly Dictionary<string, string> Errors = new Dictionary<string, string>()
        {
            {"EmptyAddress","Please enter an IP or DNS" },
            {"InvalidAddress","Enter a valid IP address or DNS" },
        };

        /// <summary>
        /// Checks if the format of an IP or DNS is correct
        /// </summary>
        /// <param name="ipOrDNS">Ip address or DNS</param>
        /// <returns></returns>
        public static bool IsValidAddress(string ipOrDNS)
        {
            string address = ipOrDNS;
            if (address == String.Empty || address == null)
            {
                return false;
            }
            
            if (IpAddressRegex.IsMatch(address) || HostNameRegex.IsMatch(address))
            {
                return true;
            }
            else
            {
                return false;
            }
        } 
    }
}
