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

        #region Insert
        public static void Insert<TTable>(this IDbConnection con, TTable objectToInsert, IDbTransaction transaction) where TTable : ITable
        {
            SqlStatement statement = SqlGenerator.GenerateInsertStatement(objectToInsert);

            con.Execute(statement.Statement, statement.Parameters, transaction);
        }

        public static void Insert<TTable>(this IDbConnection con, TTable objectToInsert)
            where TTable : ITable
        {
            SqlStatement statement = SqlGenerator.GenerateInsertStatement(objectToInsert);

            con.Execute(statement.Statement, statement.Parameters);
        }

        #endregion

        #region Update

        public static void Update<TTable>(this IDbConnection con, TTable objectToUpdate, Expression<Func<TTable, bool>> where, IDbTransaction transaction) where TTable: ITable
        {
            SqlStatement statement = SqlGenerator.GenerateUpdateStatement<TTable>(objectToUpdate, where);

            con.Execute(statement.Statement, statement.Parameters, transaction);
        }

        public static void Update<TTable>(this IDbConnection con, TTable objectToUpdate,
            Expression<Func<TTable, bool>> where) where TTable : ITable
        {
            SqlStatement statement = SqlGenerator.GenerateUpdateStatement<TTable>(objectToUpdate, where);

            con.Execute(statement.Statement, statement.Parameters);
        }

        public static void Update<TTable>(this IDbConnection con, TTable objectToUpdate,
        Expression<Func<TTable, bool>> where, Func<TTable, dynamic> fieldsToIgnore) where TTable : ITable
        {
            SqlStatement statement = SqlGenerator.GenerateUpdateStatement<TTable>(objectToUpdate, where, fieldsToIgnore);

            con.Execute(statement.Statement, statement.Parameters);
        }
        public static void Update<TTable>(this IDbConnection con, TTable objectToUpdate,
         Expression<Func<TTable, bool>> where, Func<TTable, dynamic> fieldsToIgnore, IDbTransaction transaction) where TTable : ITable
        {
            SqlStatement statement = SqlGenerator.GenerateUpdateStatement<TTable>(objectToUpdate, where, fieldsToIgnore);

            con.Execute(statement.Statement, statement.Parameters, transaction);
        }
        public static void Update<TTable>(this IDbConnection con, dynamic objectToUpdate, Expression<Func<TTable, bool>> where, IDbTransaction transaction) where TTable : ITable
        {
            SqlStatement statement = SqlGenerator.GenerateUpdateStatement<TTable>(objectToUpdate, where);

            con.Execute(statement.Statement, statement.Parameters, transaction);
        }

        public static void Update<TTable>(this IDbConnection con, dynamic objectToUpdate,
            Expression<Func<TTable, bool>> where) where TTable : ITable
        {
            SqlStatement statement = SqlGenerator.GenerateUpdateStatement<TTable>(objectToUpdate, where);

            con.Execute(statement.Statement, statement.Parameters);
        }
        #endregion

        #region Delete
        public static void Delete<TTable>(this IDbConnection con, Expression<Func<TTable, bool>> where) where TTable : ITable
        {
            SqlStatement statement = SqlGenerator.GenerateDeleteStatement<TTable>(where);

            con.Execute(statement.Statement, statement.Parameters);
        }

        public static void Delete<TTable>(this IDbConnection con, Expression<Func<TTable, bool>> where, IDbTransaction transaction) where TTable: ITable
        {
            SqlStatement statement = SqlGenerator.GenerateDeleteStatement<TTable>(where);

            con.Execute(statement.Statement, statement.Parameters, transaction);
        }

        #endregion

        #region Get
        public static List<TTable> Get<TTable>(this IDbConnection con, Expression<Func<TTable, bool>> where, IDbTransaction transaction) where TTable: ITable
        {
            SqlStatement statement = SqlGenerator.GenerateSelectStatement<TTable>(where);

            return con.Query<TTable>(statement.Statement, statement.Parameters, transaction).ToList();
        }
        public static List<TTable> Get<TTable>(this IDbConnection con, Expression<Func<TTable, bool>> where) where TTable : ITable
        {
            SqlStatement statement = SqlGenerator.GenerateSelectStatement<TTable>(where);

            return con.Query<TTable>(statement.Statement, statement.Parameters).ToList();
        }
        public static List<TTable> Get<TTable>(this IDbConnection con, IDbTransaction transaction) where TTable : ITable
        {
            SqlStatement statement = SqlGenerator.GenerateSelectStatement<TTable>(null);

            return con.Query<TTable>(statement.Statement, statement.Parameters, transaction).ToList();
        }
        public static List<TTable> Get<TTable>(this IDbConnection con) where TTable : ITable
        {
            SqlStatement statement = SqlGenerator.GenerateSelectStatement<TTable>(null);

            return con.Query<TTable>(statement.Statement, statement.Parameters).ToList();
        }
        #endregion

    }
}
