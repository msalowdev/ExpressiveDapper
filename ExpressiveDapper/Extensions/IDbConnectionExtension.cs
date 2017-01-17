using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using ExpressiveDapper.Interfaces;
using ExpressiveDapper.SqlGeneration;

namespace ExpressiveDapper.Extensions
{
    public static class IDbConnectionExtension
    {
        public static void Insert<TTable>(this IDbConnection con, TTable objectToInsert, IDbTransaction transaction) where TTable : ITable
        {
            SqlStatement statement = SqlGenerator.GenerateInsertStatement(objectToInsert);

            con.Execute(statement.Statement, statement.Parameters, transaction);
        }
        public static void Update<TTable>(this IDbConnection con, TTable objectToUpdate, Expression<Func<TTable, bool>> where, IDbTransaction transaction) where TTable: ITable
        {
            SqlStatement statement = SqlGenerator.GenerateUpdateStatement<TTable>(objectToUpdate, where);

            con.Execute(statement.Statement, statement.Parameters, transaction);
        }
        public static void Update<TTable>(this IDbConnection con, dynamic objectToUpdate, Expression<Func<TTable, bool>> where, IDbTransaction transaction) where TTable : ITable
        {
            SqlStatement statement = SqlGenerator.GenerateUpdateStatement<TTable>(objectToUpdate, where);

            con.Execute(statement.Statement, statement.Parameters, transaction);
        }

        public static void Delete<TTable>(this IDbConnection con, Expression<Func<TTable, bool>> where, IDbTransaction transaction) where TTable: ITable
        {
            SqlStatement statement = SqlGenerator.GenerateDeleteStatement<TTable>(where);

            con.Execute(statement.Statement, statement.Parameters, transaction);
        }


        public static List<TTable> Get<TTable>(this IDbConnection con, Expression<Func<TTable, bool>> where, IDbTransaction transaction) where TTable: ITable
        {
            SqlStatement statement = SqlGenerator.GenerateSelectStatement<TTable>(where);

            return con.Query<TTable>(statement.Statement, statement.Parameters, transaction).ToList();
        }

    
    }
}
