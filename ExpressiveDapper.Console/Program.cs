using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Dapper;
using ExpressiveDapper.Extensions;
using ExpressiveDapper.Interfaces;
using ExpressiveDapper.TableAttribute;

namespace ExpressiveDapper.Console
{
    class Program
    {
        public class BillingTransaction : ITable
        {
            [PrimaryKey]
            public Guid Id { get; set; }
            public Guid PolicyTermId { get; set; }
            public decimal Amount { get; set; }
            public int BillingTransactionType { get; set; }
            public Guid? PaymentId { get; set; }
            public DateTime TransactionDate { get; set; }
            public DateTime TransactionEffectiveDate { get; set; }
            public DateTime DateCreated { get; set; }
        }
        public class Payment : ITable
        {
            public Guid Id { get; set; }
            public int PaidBy { get; set; }
            public int PaymentType { get; set; }
            public string CheckNumber { get; set; }
            public string CCAuthCode { get; set; }
            public string CCTransId { get; set; }
            public string InsuredName { get; set; }
            public bool IsInSuspense { get; set; }
            public string SuspenseNumber { get; set; }
            public decimal Amount { get; set; }
            public DateTime ReceivedDate { get; set; }
            public DateTime? DepositDate { get; set; }
            public DateTime? NSFDate { get; set; }
            public string NSFUser { get; set; }
            public string PaidByOtherName { get; set; }
            public DateTime DateCreated { get; set; }
            public DateTime DateModified { get; set; }
        }
        public class TestTable :ITable
        {
            [PrimaryKey]
            public Guid Id { get; set; }
            public string Name { get; set; }
            public Guid? NullableKey { get; set; }
            public string AnotherField { get; set; }
        }

        static void Main(string[] args)
        {

            var firstTable = new TestTable
            {
                Id = new Guid("7925d7e5-518d-492d-ad04-2af15bd3343a"),
                Name = "Should not be my name",
                NullableKey = Guid.NewGuid(),
                AnotherField = "Not Updated"
            };

            var guidList = new List<Guid>() {Guid.NewGuid(), Guid.NewGuid()};

            Guid? termId = new Guid("95f36616-d927-4036-ba62-02dc3c643e9e");
            Guid notNullGuild = new Guid("95f36616-d927-4036-ba62-02dc3c643e9e");
            using (var connection = BuildConnection())
            {

                //var result = connection.Query<TestTable>("select * from TestTable where Id in @Value",
                //    new {Value = guidList});

                //var result = connection.Get<TestTable>(i => i.Id == Guid.NewGuid());
                var result = connection.Get<TestTable>(i => guidList.Contains(i.Id));

            }
        }

        private static SqlConnection BuildConnection()
        {
            return null; 
        }

    }
}
