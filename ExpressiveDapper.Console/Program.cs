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
                    using (var trans = con.BeginTransaction())
                    {
                        con.Delete<TestTable>(i => i.Id == 6, trans);

                        var result =
                        con.Get<TestTable>(i => i.Field1 == "Hello2", trans);
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
            return new SqlConnection("Data Source=swisql300;Initial Catalog=DapperTest;Integrated Security=True");
        }
    }
}
