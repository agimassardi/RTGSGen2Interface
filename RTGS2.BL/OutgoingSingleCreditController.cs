using Code.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTGS2.BL.Repository;
using System.Net;

namespace RTGS2.BL
{
    public class OutgoingSingleCreditController
    {
        string connectionString;

        public OutgoingSingleCreditController(string _connectionString)
        {
            connectionString = _connectionString;
        }

        public string ConvertToRTGSTextFileFormat(OutgoingSingleCredit outgoingSingleCredit)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{1:F01RIPAIDJ1R0090002" + outgoingSingleCredit.Sequence.AddLeadingZero(6) + "}");
            sb.Append("{2:I103INDOIDJRXXXXN}");
            sb.Append("{3:{113:0070}}");
            sb.Append("{4:");
            sb.AppendLine(":20:" + outgoingSingleCredit.TRN);
            sb.AppendLine(":23B:CRED");
            sb.AppendLine(":23E:SDVA");
            sb.AppendLine(":26T:100");
            sb.AppendLine(":32A:" + outgoingSingleCredit.Date + outgoingSingleCredit.Amount);
            sb.AppendLine(":50K:/" + outgoingSingleCredit.OrderingCustomerAccount);
            sb.AppendLine(outgoingSingleCredit.OrderingCustomerName);
            sb.AppendLine(":52A:/" + outgoingSingleCredit.OrderingInstitutionAccount);
            sb.AppendLine(outgoingSingleCredit.OrderingInstitutionBank);
            sb.AppendLine(":53A:/" + outgoingSingleCredit.SenderCorrespondentAccount);
            sb.AppendLine(outgoingSingleCredit.SenderCorrespondentBank);
            sb.AppendLine(":57A:/" + outgoingSingleCredit.BeneficiaryInstitutionAccount);
            sb.AppendLine(outgoingSingleCredit.BeneficiaryInstitutionBank);
            sb.AppendLine(":59:/" + outgoingSingleCredit.BeneficiaryCustomerAccount);
            sb.AppendLine(outgoingSingleCredit.BeneficiaryCustomerName);
            sb.AppendLine(":70:" + outgoingSingleCredit.Details);
            sb.AppendLine(":71A:OUR");
            sb.AppendLine(":72:/CODTYPTR/100");
            sb.AppendLine(":77B:/FEAB/R");
            sb.AppendLine("/PTR/LOCAL-LOCAL");
            sb.Append("-}");

            return sb.ToString();
        }

        public OperationResult GenerateTextFile(string transactionData,
                                                string ftpServerRtgs,
                                                string ftpUser,
                                                string ftpPassword,
                                                string fileName)
        {
            var op = new OperationResult();

            if (string.IsNullOrWhiteSpace(transactionData)) op.AddMessage("Transaction Data is Null");
            if (string.IsNullOrWhiteSpace(ftpServerRtgs)) op.AddMessage("Destination Path is Null");
            if (string.IsNullOrWhiteSpace(ftpUser)) op.AddMessage("FTP User is Null");
            if (string.IsNullOrWhiteSpace(ftpPassword)) op.AddMessage("FTP Password is Null");
            if (op.MessageList.Count() > 0) op.Success = false;

            if (op.Success)
            {
                try
                {
                    var tempFile = AppDomain.CurrentDomain.BaseDirectory + fileName;
                    System.IO.File.WriteAllText(tempFile, transactionData);

                    UploadFileToFTP(tempFile: tempFile,
                                    fileName: fileName,
                                    ftpServerRtgs: ftpServerRtgs,
                                    ftpUser: ftpUser,
                                    ftpPassword: ftpPassword,
                                    op: op);            
                }
                catch (Exception ex)
                {
                    op.Success = false;
                    op.AddMessage(ex.Message);
                }
            }

            return op;
        }

        public void UploadFileToFTP(string tempFile,
                                    string fileName,
                                    string ftpServerRtgs,
                                    string ftpUser,
                                    string ftpPassword,
                                    OperationResult op)
        {
            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                client.UploadFile(ftpServerRtgs + "/" + fileName, "STOR", tempFile);
            }

            System.IO.File.Delete(tempFile);
        }

        public void UpdateGeneratedTransactionStatus(string transactionCode, OperationResult op)
        {
            try
            {
                var outgoingSingleCreditRepository = new OutgoingSingleCreditRepository(connectionString);
                outgoingSingleCreditRepository.GenerateOutgoingSingleCredit(transactionCode);
            }
            catch (Exception ex)
            {
                op.Success = false;
                op.AddMessage(ex.Message);
            }
        }
    }
}
