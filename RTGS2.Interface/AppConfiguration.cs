using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RTGS2.Interface
{
    public class AppConfiguration
    {
        public int Interval { get; set; }
        public string T24SqlServer { get; set; }
        public string T24User { get; set; }
        public string T24Password { get; set; }
        public string FtpServerRTGS { get; set; }
        public string FtpUser { get; set; }
        public string FtpPassword { get; set; }

        public void SetConfiguration(string configFilePath)
        {
            var configData = System.IO.File.ReadAllText(configFilePath);
            configData = configData.Replace(@"\", @"\\");
            var config = new AppConfiguration();
            config = JsonConvert.DeserializeObject<AppConfiguration>(configData);

            Interval = config.Interval;
            T24SqlServer = config.T24SqlServer;
            T24User = config.T24User;
            T24Password = config.T24Password;
            FtpServerRTGS = config.FtpServerRTGS;
            FtpUser = config.FtpUser;
            FtpPassword = config.FtpPassword;
        }

        public string T24ConnectionString()
        {
            return string.Format(@"metadata=res://*/RTGSGen2DBContext.csdl|
                                    res://*/RTGSGen2DBContext.ssdl|
                                    res://*/RTGSGen2DBContext.msl;
                                    provider=System.Data.SqlClient;
                                    provider connection string=""
                                    data source = {0};
                                    initial catalog = RTGS_G2;
                                    persist security info = True;
                                    user id = {1};
                                    password = {2};
                                    MultipleActiveResultSets = True;
                                    App = EntityFramework""", T24SqlServer, T24User, T24Password);
        }

    }
}
