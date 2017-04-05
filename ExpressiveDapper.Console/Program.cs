using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Dapper;
using ExpressiveDapper.Extensions;
using ExpressiveDapper.Interfaces;
using ExpressiveDapper.TableAttribute;

namespace ExpressiveDapper.Console
{
    class Program
    {

        public interface IHasId: ITable
        {
             Guid Id { get; set; }
        }
        public class BillingTransaction : ITable, IHasId
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
        public class TestTable :ITable, IHasId
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

            var guidList = new List<Guid>() {new Guid("f1fb286d-21fb-49f8-a66a-c3a3acb20e52"), new Guid("6f6d1376-035f-4cff-885f-86431fc59509") };

            Guid? nullableGuid = null;//new Guid("f1fb286d-21fb-49f8-a66a-c3a3acb20e52");

            var nonNullGuid = new Guid("6f6d1376-035f-4cff-885f-86431fc59509");

            using (var connection = BuildConnection())
            {

                var result = connection.Get<TestTable>(i => guidList.Contains(i.Id) && i.AnotherField == "Hello");

                //var result = connection.Query<TestTable>("select * from TestTable where Id in @Value",
                //    new { Value = guidList });

                //var result = connection.Get<TestTable>(i => i.NullableKey == nullableGuid);
                //var innerResult = connection.Get<TestTable>(i => i.Id == Guid.NewGuid()).SingleOrDefault();

            }

            //var result = GetValue<TestTable>();
        }
        //todo: solve this with expressive dapper if possible. Taking a generic and identifying the parameter
        private static TValue GetValue<TValue>() where TValue: IHasId
        {
            var value = default(TValue);
            var tableName = $"[{typeof (TValue).Name}]";
            using (var con = BuildConnection())
            {
                var guidToTest = new Guid("f1fb286d-21fb-49f8-a66a-c3a3acb20e52");

                value = con.Get<TValue>(i => i.Id == guidToTest).SingleOrDefault();

                //value =
                //    con.Query<TValue>(
                //        $"select * from {tableName} where {tableName}.{nameof(IHasId.Id)} = @Id",
                //        new {Id =new Guid("f1fb286d-21fb-49f8-a66a-c3a3acb20e52") }).SingleOrDefault();
            }

            return value;
        }

        private static SqlConnection BuildConnection()
        {
            return new SqlConnection("Data Source=192.168.1.25,2137;Initial Catalog=dappertest;User Id=dbtestuser; Password=dbtest");
        }

    }
}
