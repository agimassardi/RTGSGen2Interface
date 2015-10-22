using Microsoft.VisualStudio.TestTools.UnitTesting;
using RTGS2.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RTGS2.BL.Tests
{
    [TestClass()]
    public class OutgoingSingleCreditControllerTests
    {
        private string connectionString;

        public OutgoingSingleCreditControllerTests()
        {
            connectionString = @"metadata=res://*/RTGSGen2DBContext.csdl|
                                    res://*/RTGSGen2DBContext.ssdl|
                                    res://*/RTGSGen2DBContext.msl;
                                    provider=System.Data.SqlClient;
                                    provider connection string=""
                                    data source = 172.16.11.11;
                                    initial catalog = RTGS_G2;
                                    persist security info = True;
                                    user id = t24-dev;
                                    password = dev@2014;
                                    MultipleActiveResultSets = True;
                                    App = EntityFramework""";
        }

        [TestMethod()]
        public void ConvertToRTGSTextFileFormatTestDataValid()
        {
            var outgoingSingleCreditController = new OutgoingSingleCreditController(connectionString);
            var outgoingSingleCreditHistory = new OutgoingSingleCredit()
            {
                TRN = "AL-004",
                Sequence = 136,
                Date = "151019",
                Amount = "IDR150000000,00",
                OrderingCustomerAccount = "1000063463456",
                OrderingCustomerName = "PUTU SURYA SUDARMADI",
                OrderingInstitutionAccount = "523466000990",
                OrderingInstitutionBank = "RIPAIDJ1",
                SenderCorrespondentAccount = "523466000990",
                SenderCorrespondentBank = "RIPAIDJ1",
                BeneficiaryInstitutionAccount = "520002000990",
                BeneficiaryInstitutionBank = "BRINIDJA",
                BeneficiaryCustomerAccount = "156789000002",
                BeneficiaryCustomerName = "RUDY ARTHA",
                Details = "SIT RTGS Gen II"
            };

            var sb = new StringBuilder();
            sb.Append("{1:F01RIPAIDJ1R0090002000136}");
            sb.Append("{2:I103INDOIDJRXXXXN}");
            sb.Append("{3:{113:0070}}");
            sb.Append("{4:");
            sb.AppendLine(":20:AL-004");
            sb.AppendLine(":23B:CRED");
            sb.AppendLine(":23E:SDVA");
            sb.AppendLine(":26T:100");
            sb.AppendLine(":32A:151019IDR150000000,00");
            sb.AppendLine(":50K:/1000063463456");
            sb.AppendLine("PUTU SURYA SUDARMADI");
            sb.AppendLine(":52A:/523466000990");
            sb.AppendLine("RIPAIDJ1");
            sb.AppendLine(":53A:/523466000990");
            sb.AppendLine("RIPAIDJ1");
            sb.AppendLine(":57A:/520002000990");
            sb.AppendLine("BRINIDJA");
            sb.AppendLine(":59:/156789000002");
            sb.AppendLine("RUDY ARTHA");
            sb.AppendLine(":70:SIT RTGS Gen II");
            sb.AppendLine(":71A:OUR");
            sb.AppendLine(":72:/CODTYPTR/100");
            sb.AppendLine(":77B:/FEAB/R");
            sb.AppendLine("/PTR/LOCAL-LOCAL");
            sb.Append("-}");

            var expected = sb.ToString();

            var actual = outgoingSingleCreditController.ConvertToRTGSTextFileFormat(outgoingSingleCreditHistory);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GenerateTextFileTestValid()
        {
            var outgoingSingleCreditController = new OutgoingSingleCreditController(connectionString);
            var outgoingSingleCredit = new OutgoingSingleCredit()
            {
                TRN = "AL-004",
                Sequence = 136,
                Date = "151019",
                Amount = "IDR150000000,00",
                OrderingCustomerAccount = "1000063463456",
                OrderingCustomerName = "PUTU SURYA SUDARMADI",
                OrderingInstitutionAccount = "523466000990",
                OrderingInstitutionBank = "RIPAIDJ1",
                SenderCorrespondentAccount = "523466000990",
                SenderCorrespondentBank = "RIPAIDJ1",
                BeneficiaryInstitutionAccount = "520002000990",
                BeneficiaryInstitutionBank = "BRINIDJA",
                BeneficiaryCustomerAccount = "156789000002",
                BeneficiaryCustomerName = "RUDY ARTHA",
                Details = "SIT RTGS Gen II"
            };

            var transactionData = outgoingSingleCreditController.ConvertToRTGSTextFileFormat(outgoingSingleCredit);
            var ftpServerRtgs = "ftp://10.1.10.24/";
            var ftpUser = "rtupload";
            var ftpPassword = "rtgs@2015";
            var fileName = outgoingSingleCredit.TRN + ".txt";

            var actual = outgoingSingleCreditController.GenerateTextFile(transactionData: transactionData,
                                                                         ftpServerRtgs: ftpServerRtgs,
                                                                         ftpUser: ftpUser,
                                                                         ftpPassword: ftpPassword,
                                                                         fileName: fileName);
           

            Assert.AreEqual(0, actual.MessageList.Count());           

            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                var fileExists = client.DownloadString(ftpServerRtgs + "/" + fileName);
                Assert.IsNotNull(fileExists);
            }

            
        }
    }
}