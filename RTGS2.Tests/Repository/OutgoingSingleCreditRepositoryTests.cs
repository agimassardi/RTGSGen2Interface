using Microsoft.VisualStudio.TestTools.UnitTesting;
using RTGS2.BL.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Code.Common;

namespace RTGS2.BL.Repository.Tests
{
    [TestClass()]
    public class OutgoingSingleCreditRepositoryTests
    {
        private string connectionString;

        public OutgoingSingleCreditRepositoryTests()
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
        public void GetOutgoingSingleCreditUnprocessedTestNotNull()
        {
            var outgoingSingleCredit = new OutgoingSingleCreditRepository(connectionString);
            IList<OutgoingSingleCredit> listTransactions = outgoingSingleCredit.GetAllOutgoingSingleCredit(TransactionStatus.Unprocessed);

            Assert.IsNotNull(listTransactions);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessOutgoingSingleCreditTransactionsCodeNull()
        {
            var outgoingSingleCredit = new OutgoingSingleCreditRepository(connectionString);

            var actual = outgoingSingleCredit.ProcessOutgoingSingleCredit(null, "150415");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessOutgoingSingleCreditTransactionsCodeEmpty()
        {
            var outgoingSingleCredit = new OutgoingSingleCreditRepository(connectionString);

            var actual = outgoingSingleCredit.ProcessOutgoingSingleCredit("", "150415");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessOutgoingSingleCreditTransactionsDateNull()
        {
            var outgoingSingleCredit = new OutgoingSingleCreditRepository(connectionString);

            var actual = outgoingSingleCredit.ProcessOutgoingSingleCredit("A01.FT151058V6RY", null);

        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessOutgoingSingleCreditTransactionsDateEmpty()
        {
            var outgoingSingleCredit = new OutgoingSingleCreditRepository(connectionString);

            var actual = outgoingSingleCredit.ProcessOutgoingSingleCredit("A01.FT151058V6RY", "");

        }

        [TestMethod()]
        public void ProcessOutgoingSingleCreditUnprocessedTransactionsCodeValidNoTransactionRemains()
        {
            var outgoingSingleCredit = new OutgoingSingleCreditRepository(connectionString);
            var listTransactions = outgoingSingleCredit.GetAllOutgoingSingleCredit(TransactionStatus.Unprocessed);

            foreach (var item in listTransactions)
            {
                outgoingSingleCredit.ProcessOutgoingSingleCredit(item.TRN, item.Date);
            }

            var listTransactionsAfter = outgoingSingleCredit.GetAllOutgoingSingleCredit(TransactionStatus.Unprocessed)
                                            .Where(o => o.Status == TransactionStatus.Unprocessed.ToString());

            Assert.AreEqual(0, listTransactionsAfter.Count());
        }
    }

}