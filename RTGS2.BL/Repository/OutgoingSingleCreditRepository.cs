using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Code.Common;
using System.Data.Entity.Core.Objects;

namespace RTGS2.BL.Repository
{
    public class OutgoingSingleCreditRepository
    {
        string connectionString;

        public OutgoingSingleCreditRepository(string _connectionString)
        {
            connectionString = _connectionString;
        }

        public IList<OutgoingSingleCredit> GetAllOutgoingSingleCredit(TransactionStatus transactionStatus)
        {
            using (var db = new RTGSGen2DBContext(connectionString))
            {
                IList<OutgoingSingleCredit> outgoingSingleCreditList = db.GetAllOutgoingSingleCredit().ToList();
                return outgoingSingleCreditList
                        .Where(o => o.Status == transactionStatus.ToString()).ToList();
            }
        }

        public OutgoingSingleCredit ProcessOutgoingSingleCredit(string transactionCode, string transactionDate)
        {
            if (string.IsNullOrEmpty(transactionCode)) throw new ArgumentException("Kode transaksi null/kosong");
            if (string.IsNullOrEmpty(transactionDate)) throw new ArgumentException("Tanggal transaksi null/kosong");

            using (var db = new RTGSGen2DBContext(connectionString))
            {
                var outgoingSingleCreditList = db.ProcessOutgoingSingleCredit(tRN: transactionCode, date: transactionDate);
                var outgoingSingleCredit = new OutgoingSingleCredit();

                foreach (var item in outgoingSingleCreditList)
                {
                    outgoingSingleCredit.TRN = item.TRN;
                    outgoingSingleCredit.Sequence = item.Sequence;
                    outgoingSingleCredit.Date = item.Date;
                    outgoingSingleCredit.Amount = item.Amount;
                    outgoingSingleCredit.OrderingCustomerAccount = item.OrderingCustomerAccount;
                    outgoingSingleCredit.OrderingCustomerName = item.OrderingCustomerName;
                    outgoingSingleCredit.OrderingInstitutionAccount = item.OrderingInstitutionAccount;
                    outgoingSingleCredit.OrderingInstitutionBank = item.OrderingInstitutionBank;
                    outgoingSingleCredit.SenderCorrespondentAccount = item.SenderCorrespondentAccount;
                    outgoingSingleCredit.SenderCorrespondentBank = item.SenderCorrespondentBank;
                    outgoingSingleCredit.BeneficiaryInstitutionAccount = item.BeneficiaryInstitutionAccount;
                    outgoingSingleCredit.BeneficiaryInstitutionBank = item.BeneficiaryInstitutionBank;
                    outgoingSingleCredit.BeneficiaryCustomerAccount = item.BeneficiaryCustomerAccount;
                    outgoingSingleCredit.BeneficiaryCustomerName = item.BeneficiaryCustomerName;
                    outgoingSingleCredit.Details = item.Details;
                    outgoingSingleCredit.Inputter = item.Inputter;
                    outgoingSingleCredit.CompanyCode = item.CompanyCode;
                    outgoingSingleCredit.Status = item.Status;
                }

                return outgoingSingleCredit;
            }
        }

        public int GenerateOutgoingSingleCredit(string transactionCode)
        {
            if (string.IsNullOrEmpty(transactionCode)) throw new ArgumentException("Kode transaksi null/kosong");

            using (var db = new RTGSGen2DBContext(connectionString))
            {
                return db.GenerateOutgoingSingleCredit(tRN: transactionCode);
            }
        }
    }
}
