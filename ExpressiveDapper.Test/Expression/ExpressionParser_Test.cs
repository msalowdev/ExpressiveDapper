using ExpressiveDapper.Expressions;
using ExpressiveDapper.Interfaces;
using ExpressiveDapper.TableAttribute;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveDapper.Test.Expression
{
    [TestClass]
    public class ExpresionParser_Test
    {
        private class Person : ITable
        {
            [PrimaryKey]
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int CompanyId { get; set; }
        }
        [TestMethod]
        public void Given_An_Orm_Object_When_Comparing_Parameter_String_Value_For_Equal_Then_SQL_Valid_Equal_Compare_With_Value_Of_Object_String_Returned()
        {
            //given
            var person = new Person
            {
                CompanyId = 7,
                FirstName = "Bilbo",
                LastName = "Baggins",
                Id = 10
            };

            ExpressionParser parser = new ExpressionParser();
            //when
            var result = parser.ParseCompareFunction<Person>(per => per.FirstName == person.FirstName);

            //then
            var expectedResult = $"[Person].[FirstName] = @parm0";

            Assert.AreEqual(result.SqlStatement, expectedResult);

        }
        [TestMethod]
        public void Given_An_Orm_Object_When_Comparing_Parameter_Int_Value_For_Equal_Then_SQL_Valid_Equal_Compare_With_Value_Of_Object_String_Returned()
        {
            //given
            var person = new Person
            {
                CompanyId = 7,
                FirstName = "Bilbo",
                LastName = "Baggins",
                Id = 10
            };

            ExpressionParser parser = new ExpressionParser();
            //when
            var result = parser.ParseCompareFunction<Person>(per => per.CompanyId == person.CompanyId);

            //then
            var expectedResult = $"[Person].[CompanyId] = @parm0";

            Assert.AreEqual(result.SqlStatement, expectedResult);

        }

        [TestMethod]
        public void Given_An_Orm_Object_When_Comparing_Parameter_Int_Value_For_Greater_Than_Then_SQL_Valid_Greater_Than_Compare_With_Value_Of_Object_String_Returned()
        {
            //given
            var person = new Person
            {
                CompanyId = 7,
                FirstName = "Bilbo",
                LastName = "Baggins",
                Id = 10
            };

            ExpressionParser parser = new ExpressionParser();
            //when
            var result = parser.ParseCompareFunction<Person>(per => per.CompanyId > person.CompanyId);

            //then
            var expectedResult = $"[Person].[CompanyId] > @parm0";

            Assert.AreEqual(result.SqlStatement, expectedResult);

        }

        [TestMethod]
        public void Given_A_Constant_String_Value_When_Comparing_Parameter_Value_Then_SQL_Valid_Equal_Compare_String_Returned
            ()
        {

            ExpressionParser parser = new ExpressionParser();
            //when
            var result = parser.ParseCompareFunction<Person>(per => per.FirstName == "Bilbo");

            //then
            var expectedResult = $"[Person].[FirstName] = @parm0";

            Assert.AreEqual(result.SqlStatement, expectedResult);
        }

        [TestMethod]
        public void Given_A_String_Variable_Value_When_Comparing_Parameter_Value_Then_SQL_Valid_Equal_Compare_String_Returned()
        {
            ExpressionParser parser = new ExpressionParser();

            //given
            var value = "Bilbo";

            //when
            var result = parser.ParseCompareFunction<Person>(per => per.FirstName == value);

            //then
            var expectedResult = $"[Person].[FirstName] = @parm0";

            Assert.AreEqual(result.SqlStatement, expectedResult);
        }
        [TestMethod]
        public void Given_A_Constant_When_Using_Logical_Compare_With_Equal_And_Greater_Than_Compares_Then_SQL_Valid_And_Compare_String_Returned()
        {
            //given
            var firstNameTest = "Bilbo";
            var companyIdTest = 3;

            ExpressionParser parser = new ExpressionParser();

            //when
            var result =
                parser.ParseCompareFunction<Person>(
                    per => per.FirstName == firstNameTest && per.CompanyId > companyIdTest);
            //then
            var expectedResult = $"[Person].[FirstName] = @parm0 and [Person].[CompanyId] > @parm1";

            Assert.AreEqual(result.SqlStatement, expectedResult);
        }
        [TestMethod]
        public void Given_An_Orm_Object_When_Comparing_Parameter_Int_Value_For_Not_Equal_Then_SQL_Valid_Not_Equal_Compare_With_Value_Of_Object_String_Returned()
        {
            //given
            var person = new Person
            {
                CompanyId = 7,
                FirstName = "Bilbo",
                LastName = "Baggins",
                Id = 10
            };

            ExpressionParser parser = new ExpressionParser();
            //when
            var result = parser.ParseCompareFunction<Person>(per => per.CompanyId != person.CompanyId);

            //then
            var expectedResult = $"[Person].[CompanyId] != @parm0";

            Assert.AreEqual(result.SqlStatement, expectedResult);
        }
    }
}
