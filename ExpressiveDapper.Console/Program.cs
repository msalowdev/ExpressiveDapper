using System;
using System.Data.SqlClient;
using ExpressiveDapper.Extensions;
using ExpressiveDapper.Interfaces;
using ExpressiveDapper.TableAttribute;

namespace ExpressiveDapper.Console
{
    class Program
    {
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


         


            using (var connection = BuildConnection())
            {
                //connection.Insert(firstTable);
                //connection.Insert(secondTable);
                connection.Update<TestTable>(firstTable, where => where.Id == firstTable.Id, ignor => new {ignor.Name, ignor.AnotherField});

            }
        }

        private static SqlConnection BuildConnection()
        {
            return new SqlConnection("Data Source<ServeName>;Initial Catalog=<DBName>;");
        }

    }
}
