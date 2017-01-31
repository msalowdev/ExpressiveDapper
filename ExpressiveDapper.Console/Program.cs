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
        public class Books : ITable
        {
            [PrimaryKey]
            public int BookId { get; set; }
            public string BookName { get; set; }
            public DateTime PublishDate { get; set; }
            public int Count { get; set; }
        }

        public class Library: ITable
        {
            public int Id { get; set; }
            public int PeopleWorkingHere { get; set; }
            public int BookId { get; set; }
            public string LibraryName { get; set; }
        }

        static void Main(string[] args)
        {

            var newBook = new Books()
            {
                BookId = 15,
                BookName = "Updated Book again",
                Count = 12,
                PublishDate = DateTime.Now
            };

        }

        private static SqlConnection BuildConnection()
        {
            return new SqlConnection("Data Source=<Server>;Initial Catalog=<DB>;");
        }
    }
}
