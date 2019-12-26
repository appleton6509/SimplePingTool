using Microsoft.VisualStudio.TestTools.UnitTesting;
using PingData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingData.Tests
{
    [TestClass()]
    public class PingStatsTests
    {
        [TestMethod()]
        public void Add_AddressResultIsNull()
        {
            //Assign
            PingStats stats = new PingStats();
            PingResult result = new PingResult()
            {
                AddressOrIp = null,
            };
            bool expectedResult = true;
            bool actualResult;

            //act

            stats.Add(result);
            actualResult = (stats.MaxLatency == 0 && stats.PacketsLost == 0 && stats.PacketsSent == 0 && stats.AverageLatency == 0);

            //assert
            Assert.AreEqual(expectedResult, actualResult);

        }

        [TestMethod()]
        public void Add_LatencyResultIsNegative()
        {
            //Assign
            PingStats stats = new PingStats();
            PingResult result = new PingResult()
            {
                AddressOrIp = "www.google.com",
                Status = PingHost.Status.SUCCESS,
                Latency = -1,
            };
            bool expectedResult = true;
            bool actualResult;

            //act
            stats.Add(result);
            actualResult = (stats.MaxLatency == 0 && stats.PacketsLost == 0 && stats.PacketsSent == 1 && stats.AverageLatency == 0);

            //assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod()]
        public void Add_ResultIsSuccessful()
        {
            //Assign
            PingStats stats = new PingStats();
            PingResult result = new PingResult()
            {
                AddressOrIp = "www.google.com",
                Latency = 1,
                Status = PingHost.Status.SUCCESS,
            };
            bool expectedResult = true;
            bool actualResult;

            //act
            stats.Add(result);
            actualResult = (stats.MaxLatency == 1 && stats.PacketsLost == 0 && stats.PacketsSent == 1 && stats.AverageLatency == 1);

            //assert
            Assert.AreEqual(expectedResult, actualResult);
        }
        [TestMethod()]
        public void Add_ResultIsFailure()
        {
            //Assign
            PingStats stats = new PingStats();
            PingResult result = new PingResult()
            {
                AddressOrIp = "www.google.com",
                Latency = 1,
                Status = PingHost.Status.FAILURE,
            };
            bool expectedResult = true;
            bool actualResult;

            //act
            stats.Add(result);
            actualResult = (stats.MaxLatency == 0 && stats.PacketsLost == 1 && stats.PacketsSent == 1 && stats.AverageLatency == 0);

            //assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod()]
        public void Add_ResultIsFailureAndSuccess()
        {
            //Assign
            PingStats stats = new PingStats();
            PingResult result = new PingResult()
            {
                AddressOrIp = "www.google.com",
                Latency = 1,
                Status = PingHost.Status.FAILURE,
            };
            PingResult result2 = new PingResult()
            {
                AddressOrIp = "www.google.com",
                Latency = 5,
                Status = PingHost.Status.SUCCESS,
            };
            bool expectedResult = true;
            bool actualResult;

            //act
            stats.Add(result);
            stats.Add(result2);
            actualResult = (stats.MaxLatency == 5 && stats.PacketsLost == 1 && stats.PacketsSent == 2 && stats.AverageLatency == 5);

            //assert
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}