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
                Status = PingResult.StatusMessage.SUCCESS,
                Latency = -1,
            };
            bool expectedResult = true;
            bool actualResult;

            //act
            stats.Add(result);
            actualResult = (stats.MaxLatency == 0 && stats.PacketsLost == 0 && stats.PacketsSent == 1 && stats.AverageLatency == 1);

            //assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod()]
        public void Add_ResultStatusSuccess()
        {
            //Assign
            PingStats stats = new PingStats();
            PingResult result = new PingResult()
            {
                AddressOrIp = "www.google.com",
                Latency = 1,
                Status = PingResult.StatusMessage.SUCCESS,
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
        public void Add_ResultStatusFailure()
        {
            //Assign
            PingStats stats = new PingStats();
            PingResult result = new PingResult()
            {
                AddressOrIp = "www.google.com",
                Latency = 1,
                Status = PingResult.StatusMessage.FAILURE,
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
        public void Add_ResultStatusFailureAndSuccess()
        {
            //Assign
            PingStats stats = new PingStats();
            PingResult result = new PingResult()
            {
                AddressOrIp = "www.google.com",
                Latency = 1,
                Status = PingResult.StatusMessage.FAILURE,
            };
            PingResult result2 = new PingResult()
            {
                AddressOrIp = "www.google.com",
                Latency = 5,
                Status = PingResult.StatusMessage.SUCCESS,
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

        [TestMethod()]
        public void AverageLatency_PingCountIsZero()
        {
            //Assign
            PingStats stats = new PingStats();
            double expectedResult = 0;
            double actualResult;

            //act
            actualResult = stats.AverageLatency;

            //assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod()]
        public void AverageLatency_PingCountIsThree()
        {
            //Assign
            PingStats stats = new PingStats();
            PingResult result1 = new PingResult()
            {
                AddressOrIp = "www.google.com",
                Latency = 0,
                Status = PingResult.StatusMessage.SUCCESS,
            };
            PingResult result2 = new PingResult()
            {
                AddressOrIp = "www.google.com",
                Latency = 50,
                Status = PingResult.StatusMessage.SUCCESS,
            };
            PingResult result3 = new PingResult()
            {
                AddressOrIp = "www.google.com",
                Latency = 100,
                Status = PingResult.StatusMessage.SUCCESS,
            };

            double expectedResult = 50;
            double actualResult;

            //act
            stats.Add(result1);
            stats.Add(result2);
            stats.Add(result3);
            actualResult = stats.AverageLatency;

            //assert
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}