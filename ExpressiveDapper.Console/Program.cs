using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressiveDapper.Extensions;
using ExpressiveDapper.Interfaces;
using ExpressiveDapper.SqlGeneration;

namespace ExpressiveDapper.Console
{
    class Program
    {

        public class Perosn : ITable
        {
            public int Id { get; set; }
        }

        public class TestTable : ITable
        {
            public int Id { get; set; }
            public string Field1 { get; set; }
            public DateTime Field2 { get; set; }
            public int? Field3 { get; set; }
        }

        public class PolicyTerm : ITable
        {
            public Guid Id { get; set; }
            public string PolicyNumber { get; set; }
            public DateTime EffectiveDate { get; set; }
            public DateTime ExpirationDate { get; set; }
            public decimal StateFeeRate { get; set; }
            public decimal TaxRate { get; set; }
            public int TermLength { get; set; }
            public DateTime DateCreated { get; set; }
            public DateTime DateModified { get; set; }
        }

        static void Main(string[] args)
        {

            var newTestTable = new TestTable()
            {
                Id = 1,
                Field1 = "Hello2",
                Field2 = new DateTime(2012, 1, 1),
                Field3 = 12
            };


            try
            {

                using (var con = BuildConnection())
                {
                    con.Open();
                    using (var trans = con.BeginTransaction())
                    {

                        var result =
                        con.Get<PolicyTerm>(i => i.Id != Guid.Empty, trans);
                        System.Console.WriteLine(result.Count);
                        System.Console.ReadLine();
                        trans.Commit();
                    }
                }

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.ReadLine();
            }


        }

        private static SqlConnection BuildConnection()
        {
            return new SqlConnection("Data Source=192.168.1.24,2137;Initial Catalog=SWIBilling;User Id=dbtestuser; Password=dbtest");
        }
    }
}
