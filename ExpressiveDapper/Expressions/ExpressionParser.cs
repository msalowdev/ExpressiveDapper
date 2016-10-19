using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using ExpressiveDapper.Extensions;
using ExpressiveDapper.Interfaces;
using ExpressiveDapper.SqlGeneration;

namespace ExpressiveDapper.Expressions
{
    public class ExpressionParser
    {
        public SqlParsedExpression ParseCompareFunction<TTable>(Expression<Func<TTable, bool>> expression) where TTable : ITable
        {
            var body = expression.Body as BinaryExpression;
            var tableName = typeof(TTable).GetName();
           
            List<SqlParsedParameter> parsedParms = new List<SqlParsedParameter>();
            var statement = ParseBinaryExpression(tableName, body, parsedParms);

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            foreach (var sqlParsedParameter in parsedParms)
            {
                parameters.Add(sqlParsedParameter.ParameterName, sqlParsedParameter.Value);
            }

            SqlParsedExpression parsedExpression = new SqlParsedExpression(statement, parameters);

            return parsedExpression;
        }

        private string ParseBinaryExpression(string tableName, BinaryExpression expression, List<SqlParsedParameter> parsedParms)
        {

            var left = expression.Left;
            var right = expression.Right;
            var nodeType = expression.NodeType;
            return ParseExpression(tableName, left, parsedParms) + nodeType.GetSql() + ParseExpression(tableName, right, parsedParms);
        }

        private string ParseExpression(string tableName, Expression expression, List<SqlParsedParameter> parsedParms)
        {
            string parsedExpression = string.Empty;

            if (expression is ConstantExpression)
            {
                var value = GetValueFromConstantExpression((ConstantExpression) expression);
                parsedExpression = "@parm" + parsedParms.Count;
                parsedParms.Add(new SqlParsedParameter
                {
                    ParameterName = parsedExpression,
                    Value = value
                });
            }
            else if (expression is MemberExpression)
            {
                var memberExpression = (MemberExpression) expression;
                if (memberExpression.Expression.NodeType == ExpressionType.Parameter)
                {
                    parsedExpression = ParseParameterExpression(tableName, memberExpression);
                }
                else
                {
                    var value = GetValueFromMemberExpresion(memberExpression);
                    parsedExpression = "@parm" + parsedParms.Count;
                    parsedParms.Add(new SqlParsedParameter
                    {
                        ParameterName = parsedExpression,
                        Value = value
                    });
                }
            }
            else if (expression is BinaryExpression)
            {
                var binaryExpression = (BinaryExpression) expression;
                parsedExpression = ParseBinaryExpression(tableName, binaryExpression, parsedParms);
            }
            else
            {
                throw new ArgumentException($"Unknown expression type: {expression.NodeType}. Could not be converted to a binary expression");
            }
            return parsedExpression;
        }


        private string ParseParameterExpression(string tableName, MemberExpression memberExpression)
        {
            return $"{tableName}.[{memberExpression.Member.Name}]";
        }
        private object GetValueFromMemberExpresion(MemberExpression memberExpression)
        {
            var function = Expression.Lambda(memberExpression).Compile();
            var value = function.DynamicInvoke();

            return value;
        }

        private object GetValueFromConstantExpression(ConstantExpression constantExpression)
        {
            return constantExpression.Value;
        }
    }
}
