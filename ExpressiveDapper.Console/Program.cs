using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressiveDapper.Extensions;
using ExpressiveDapper.Interfaces;
using ExpressiveDapper.SqlGeneration;
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
        }

        static void Main(string[] args)
        {

            var firstTable = new TestTable
            {
                Id = Guid.NewGuid(),
                Name = "FirstTable",
                NullableKey = Guid.Empty
            };

            var secondTable = new TestTable
            {
                Id = Guid.NewGuid(),
                Name = "Second Table",
                NullableKey = null
            };

            Guid? guidToTest = null;

            using (var connection = BuildConnection())
            {
                //connection.Insert(firstTable);
                //connection.Insert(secondTable);
                var table =
                connection.Get<TestTable>(i => i.NullableKey == guidToTest);
            }
        }

        private static SqlConnection BuildConnection()
        {
            return new SqlConnection("Data Source<ServeName>;Initial Catalog=<DBName>;");
        }
    }
}
