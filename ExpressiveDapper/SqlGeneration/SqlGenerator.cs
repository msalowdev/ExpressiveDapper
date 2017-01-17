using System;
using System.Linq.Expressions;
using ExpressiveDapper.Expressions;
using ExpressiveDapper.Interfaces;
using ExpressiveDapper.Extensions;

namespace ExpressiveDapper.SqlGeneration
{
    public static class SqlGenerator
    {
        public static SqlStatement GenerateSelectStatement<TTable>(Expression<Func<TTable, bool>> where) where TTable: ITable
        {

            var tableType = typeof (TTable);
            ExpressionParser parser = new ExpressionParser();

            SqlStatement sqlStatement = new SqlStatement();

            var selectParms = TypeParser.BuildSqlForSelect( tableType);

            var parsedWhereExpression = parser.ParseCompareFunction(where);

            sqlStatement.Statement = $"select {selectParms} from {tableType.GetName()} {(string.IsNullOrEmpty(parsedWhereExpression.SqlStatement) ? string.Empty : " where " + parsedWhereExpression.SqlStatement)}";
            sqlStatement.Parameters = parsedWhereExpression.Parameters.ToDynamicParameters();
            return sqlStatement;
        }

        public static SqlStatement GenerateInsertStatement<TTable>(TTable tableObj) where TTable : ITable
        {
            var tableType = typeof (TTable);
            SqlStatement sqlStatement = new SqlStatement();
            var insertInfo = TypeParser.BuildSqlForInsert(tableObj);
            sqlStatement.Statement = $"insert into {tableType.GetName()} ({insertInfo.InsertStatement}) values ({insertInfo.ParameterStatement})";
            sqlStatement.Parameters = insertInfo.Parameters.ToDynamicParameters();
            return sqlStatement;
        }

        public static SqlStatement GenerateUpdateStatement<TTable>(TTable tableObject,
            Expression<Func<TTable, bool>> where) where TTable : ITable
        {
            var tableType = typeof(TTable);
            ExpressionParser parser = new ExpressionParser();

            SqlStatement sqlStatement = new SqlStatement();

            var updateInfo = TypeParser.BuildSqlForUpdate(tableObject);

            var parsedWhereExpression = parser.ParseCompareFunction(where);

            

            sqlStatement.Statement = $"update {tableType.GetName()} set {updateInfo.UpdateStatement} {(string.IsNullOrEmpty(parsedWhereExpression.SqlStatement) ? string.Empty: "where " + parsedWhereExpression.SqlStatement)}";
            sqlStatement.Parameters = updateInfo.Parameters.ToDynamicParameters(parsedWhereExpression.Parameters); 
            return sqlStatement;
        }

        public static SqlStatement GenerateUpdateStatement<TTable>(dynamic objectToUpdate,
            Expression<Func<TTable, bool>> where) where TTable: ITable
        {
            var tableType = typeof(TTable);
            ExpressionParser parser = new ExpressionParser();

            SqlStatement sqlStatement = new SqlStatement();

            FormattedUpdateProperties updateInfo = TypeParser.ParseDynamicForUpdate(objectToUpdate);

            var parsedWhereExpression = parser.ParseCompareFunction(where);

            sqlStatement.Statement = $"update {tableType.GetName()} set {updateInfo.UpdateStatement} {(string.IsNullOrEmpty(parsedWhereExpression.SqlStatement) ? string.Empty : "where " + parsedWhereExpression.SqlStatement)}";
            sqlStatement.Parameters = updateInfo.Parameters.ToDynamicParameters(parsedWhereExpression.Parameters);
            return sqlStatement;
        }

        public static SqlStatement GenerateDeleteStatement<TTable>(Expression<Func<TTable, bool>> where)
            where TTable : ITable
        {
            var tableType = typeof(TTable);
            ExpressionParser parser = new ExpressionParser();

            SqlStatement sqlStatement = new SqlStatement();

            var parsedWhereExpression = parser.ParseCompareFunction(where);

            sqlStatement.Statement = $"delete from {tableType.GetName()} {(string.IsNullOrEmpty(parsedWhereExpression.SqlStatement) ? string.Empty : " where " + parsedWhereExpression.SqlStatement)}";
            sqlStatement.Parameters = parsedWhereExpression.Parameters.ToDynamicParameters();
            return sqlStatement;
        }
    }
}
