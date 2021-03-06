﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ExpressiveDapper.Extensions;
using ExpressiveDapper.Interfaces;

namespace ExpressiveDapper.Expressions
{
    public class ExpressionParser
    {
        private const string NULL_VALUE = " NULL ";

        public SqlParsedExpression ParseCompareFunction<TTable>(Expression<Func<TTable, bool>> expression) where TTable : ITable
        {

            List<SqlParsedParameter> parsedParms = new List<SqlParsedParameter>();
            var tableName = typeof(TTable).GetName();
            var statement = string.Empty;

            //special check for an expression that only has a single method call to contains on a list.
            if (expression.Body.IsContainsCall())
            {
                statement = ParseListContainsToInStatement(tableName, expression.Body as MethodCallExpression, parsedParms);
            }
            else if (expression.Body is BinaryExpression)
            {
                var binaryExpression = (BinaryExpression)expression.Body;

                statement = ParseBinaryExpression(tableName, binaryExpression, parsedParms);
            }
            else
            {
                throw new NotSupportedException($"Expression Type {expression.Body.GetType().Name} not supported");
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            foreach (var sqlParsedParameter in parsedParms)
            {
                parameters.Add(sqlParsedParameter.ParameterName, sqlParsedParameter.Value);

            }
            SqlParsedExpression parsedExpression = new SqlParsedExpression(statement, parameters);

            return parsedExpression;
        }

        private string ParseBinaryExpression(string tableName, BinaryExpression expression,
            List<SqlParsedParameter> parsedParms)
        {

            var left = expression.Left;
            var right = expression.Right;
            var nodeType = expression.NodeType;

            var leftValue = ParseExpression(tableName, left, parsedParms);
            var rightValue = ParseExpression(tableName, right, parsedParms);

            bool nullCheck = leftValue == NULL_VALUE || rightValue == NULL_VALUE;

            return leftValue + nodeType.GetSql(nullCheck) + rightValue;
        }

        private string ParseExpression(string tableName, Expression expression, List<SqlParsedParameter> parsedParms)
        {
            string parsedExpression = string.Empty;

           
            if (expression is ConstantExpression)
            {
                parsedExpression = ParseForSqlParameter(expression, parsedParms);
            }
            else if (expression is MethodCallExpression)
            {
                parsedExpression = expression.IsContainsCall() ? 
                    ParseListContainsToInStatement(tableName, (MethodCallExpression) expression,parsedParms) : 
                    ParseForSqlParameter(expression, parsedParms);
                
            }
            else if (expression is MemberExpression)
            {
                var memberExpression = (MemberExpression) expression;

                parsedExpression = memberExpression.IsParameterAccess() ? 
                    ParseParameterExpression(tableName, memberExpression) : 
                    ParseForSqlParameter(expression, parsedParms);
            }
            else if (expression is BinaryExpression)
            {
                var binaryExpression = (BinaryExpression) expression;
                parsedExpression = ParseBinaryExpression(tableName, binaryExpression, parsedParms);
            }
            else if (expression is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression) expression;
                if (expression.NodeType == ExpressionType.Convert)
                {
                    var memberExpression = unaryExpression.Operand as MemberExpression;

                    if (memberExpression?.IsParameterAccess() ?? false)
                    {
                        parsedExpression = ParseParameterExpression(tableName, memberExpression);
                    }
                    else if (memberExpression != null)
                    {
                        parsedExpression = ParseForSqlParameter(expression, parsedParms);
                    }
                    else
                    {

                        throw new Exception("This type of convert is not currently supported");
                    }

                }
                else
                {
                    parsedExpression = ParseForSqlParameter(expression, parsedParms);
                }
            }
            else
            {
                throw new ArgumentException(
                    $"Unknown expression type: {expression.GetType()}, Node Type {expression.NodeType}. Could not be converted to a binary expression");
            }
            return parsedExpression;
        }

        private string ParseForSqlParameter(Expression expression, List<SqlParsedParameter> parsedParms)
        {
            string paramExpression = string.Empty;
            var value = GetValueFromExpresion(expression);

            if (value == null)
                paramExpression = NULL_VALUE;
            else
            {
                paramExpression = "@parm" + parsedParms.Count;
                parsedParms.Add(new SqlParsedParameter
                {
                    ParameterName = paramExpression,
                    Value = value
                });
            }
            return paramExpression;
        }

        private string ParseListContainsToInStatement(string tableName, MethodCallExpression methodCallExpression,
            List<SqlParsedParameter> parsedParms)
        {
            string statement = string.Empty;
            if (methodCallExpression != null && methodCallExpression.Method.Name == "Contains")
            {

                var memberExpression = (MemberExpression)methodCallExpression.Object;
                var getCaller = Expression.Lambda<Func<object>>(memberExpression).Compile();
                var list = (IEnumerable)getCaller();

                string inParmList = string.Empty;

                foreach (var item in list)
                {
                    parsedParms.Add(new SqlParsedParameter
                    {
                        ParameterName = "@parm" + parsedParms.Count,
                        Value = item
                    });
                }
                inParmList = string.Join(",", parsedParms.Select(i => i.ParameterName));

                var parameter = ParseParameterExpression(tableName,
                    methodCallExpression.Arguments[0] as MemberExpression);
                statement = $"{parameter} in ({inParmList})";

            }
            else
            {
                throw new Exception($"Method {methodCallExpression?.Method.Name ?? "No expression found"} is not supported");
            }

            return statement;
        }

        private string ParseParameterExpression(string tableName, MemberExpression memberExpression)
        {
            return $"{tableName}.[{memberExpression.Member.Name}]";
        }
        private object GetValueFromExpresion(Expression expression)
        {

            var constantExpression = expression as ConstantExpression;
            if(constantExpression != null)
                return constantExpression.Value;

            var function = Expression.Lambda(expression).Compile();
            var value = function.DynamicInvoke();
            return value;
        }


        private object GetValueFromConstantExpression(ConstantExpression constantExpression)
        {
            return constantExpression.Value;
        }
    }
}
