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

        //public class Perosn : ITable
        //{
        //    public int Id { get; set; }
        //}

        //public class TestTable : ITable
        //{
        //    public int Id { get; set; }
        //    public string Field1 { get; set; }
        //    public DateTime Field2 { get; set; }
        //    public int? Field3 { get; set; }
        //}

        static void Main(string[] args)
        {

            var newBook = new Books()
            {
                BookId = 15,
                BookName = "Updated Book again",
                Count = 12,
                PublishDate = DateTime.Now
            };

            using (var con = BuildConnection())
            {
                con.Open();

                using (var trans = con.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        //con.Insert(newBook, trans);

                        con.Update(newBook, i => i.BookId == newBook.BookId, trans);

                        var retreivedBook =
                        con.Get<Books>(i => i.BookId == newBook.BookId, trans).FirstOrDefault();
                        System.Console.WriteLine(retreivedBook);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {

                        System.Console.WriteLine(ex.Message);
                        trans.Rollback();
                    }      
                }
            }

        }

        private static SqlConnection BuildConnection()
        {
            return new SqlConnection("Data Source=<DBName>;Initial Catalog=DapperTest;Integrated Security=True");
        }
    }
}
