using Microsoft.VisualStudio.TestTools.UnitTesting;
using RTGS2.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTGS2.Tests
{
    [TestClass()]
    public class ConfigurationTests
    {
        [TestMethod()]
        public void ConfigurationTestValidData()
        {
            string configFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\config.json";
            var config = new AppConfiguration();
            config.SetConfiguration(configFilePath);

            var expected = new AppConfiguration()
            {
                Interval = 10,
                T24SqlServer = "172.16.11.11",
                FtpServerRTGS = @"ftp://10.1.10.24/"
            };

            var actual = config;

            Assert.AreEqual(expected.Interval, actual.Interval);
            Assert.AreEqual(expected.T24SqlServer, actual.T24SqlServer);
            Assert.AreEqual(expected.FtpServerRTGS, actual.FtpServerRTGS);

        }
    }
}