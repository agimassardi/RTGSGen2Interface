using RTGS2.BL;
using RTGS2.BL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Code.Common;

namespace RTGS2.Interface
{
    public class TransactionController
    {
        AppConfiguration config;
        ConsoleLogLibrary log;

        public TransactionController(AppConfiguration _config,
                                     ConsoleLogLibrary _log)
        {
            config = _config;
            log = _log;
        }

        public bool ProcessAllTransactions()
        {
            try
            {
                GetProcessedTransactions();
                GetUnprocessedTransactions();                
            }
            catch (Exception ex)
            {
                log.Write(ex.Message);
            }

            return false;
        }

        private void GetProcessedTransactions()
        {
            var outgoingSingleCreditRepository = new OutgoingSingleCreditRepository(config.T24ConnectionString());
            var listTransaction = outgoingSingleCreditRepository.GetAllOutgoingSingleCredit(TransactionStatus.Processed);

            if (listTransaction.Count() > 0)
            {
                foreach (var item in listTransaction)
                {
                    GenerateTextFile(item);
                    Console.WriteLine();
                }
            }
        }

        private void GetUnprocessedTransactions()
        {

            var outgoingSingleCreditRepository = new OutgoingSingleCreditRepository(config.T24ConnectionString());
            var listTransaction = outgoingSingleCreditRepository.GetAllOutgoingSingleCredit(TransactionStatus.Unprocessed);

            if (listTransaction.Count() > 0)
            {
                foreach (var item in listTransaction)
                {
                    try
                    {
                        var unprocessedTransaction = outgoingSingleCreditRepository
                                                        .ProcessOutgoingSingleCredit(item.TRN, item.Date);
                        log.Write("Transaction " + item.TRN + " has been processed");

                        GenerateTextFile(unprocessedTransaction);
                        Console.WriteLine();
                    }
                    catch (Exception ex)
                    {
                        log.Write("Transaction " + item.TRN + " failed to be processed\n" + ex.Message);
                    }
                }
            }

        }        

        private void GenerateTextFile(IList<OutgoingSingleCredit> outgoingSingleCreditList)
        {
            foreach (var outgoingSingleCredit in outgoingSingleCreditList)
            {
                ProcessGenerateTextFile(outgoingSingleCredit);
            }
        }

        private void GenerateTextFile(OutgoingSingleCredit outgoingSingleCredit)
        {
            ProcessGenerateTextFile(outgoingSingleCredit);
        }

        private void ProcessGenerateTextFile(OutgoingSingleCredit outgoingSingleCredit)
        {
            var outgoingSingleCreditController = new OutgoingSingleCreditController(config.T24ConnectionString());
            var transactionData = outgoingSingleCreditController.ConvertToRTGSTextFileFormat(outgoingSingleCredit);
            var fileName = outgoingSingleCredit.TRN + ".txt";

            var result = outgoingSingleCreditController.GenerateTextFile(transactionData: transactionData,
                                                                         ftpServerRtgs: config.FtpServerRTGS,
                                                                         ftpUser: config.FtpUser,
                                                                         ftpPassword: config.FtpPassword,
                                                                         fileName: fileName);

            if (result.Success)
            {
                UpdateGeneratedTransactionStatus(outgoingSingleCreditController, outgoingSingleCredit.TRN, result);
            }
            else
            {
                log.WriteErrorLog("Generate text file failed.", result);
            }
        }

        private void UpdateGeneratedTransactionStatus(OutgoingSingleCreditController outgoingSingleCreditController,
                                                        string transactionCode, OperationResult op)
        {
            outgoingSingleCreditController.UpdateGeneratedTransactionStatus(transactionCode, op);

            if (op.Success)
            {
                log.Write("Text file " + transactionCode + " has been sent to RTGS");
            }
            else
            {
                log.WriteErrorLog("Update generated transaction status failed.", op);
            }
        }
    }
}
