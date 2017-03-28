using System;
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
            //Note: so if only contains, then the body is not a binary expression.
            var binaryExpression = expression.Body as BinaryExpression;
            var methodExpression = expression.Body as MethodCallExpression;
            if (binaryExpression != null)
            {
                statement = ParseBinaryExpression(tableName, binaryExpression, parsedParms);
            }
            else if(methodExpression != null && methodExpression.Method.Name == "Contains")
            {
               
                var memberExpression =
                    ((MemberExpression) methodExpression.Object);
                var getCaller = Expression.Lambda<Func<object>>(memberExpression).Compile();
                //todo: now we have our list all we need to do is iterate over it and build the parameters.
                var list = (IEnumerable) getCaller();

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

                statement = $"{ParseParameterExpression(tableName, methodExpression.Arguments[0] as MemberExpression)} in ({inParmList})"; 
                
            }
            else
            {
                if (methodExpression != null)
                {
                    throw new NotSupportedException($"Only the Contains method is currently supported for Expression Type {expression.Body.GetType().Name}");
                }
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

            //Need to handle conversion. In the situations I have seen
            //we need to convert them to member functions if they are parameters
            //and we need to get the values then convert if they are values to be passed as parameters
          
            if (expression is ConstantExpression)
            {
                var value = GetValueFromConstantExpression((ConstantExpression) expression);
                if (value == null)
                {
                    parsedExpression = NULL_VALUE;
                }
                else
                {
                    parsedExpression = "@parm" + parsedParms.Count;
                    parsedParms.Add(new SqlParsedParameter
                    {
                        ParameterName = parsedExpression,
                        Value = value
                    });
                }
               
            }
            else if (expression is MethodCallExpression)
            {
                var value = GetValueFromExpresion(expression);

                if (value == null)
                    parsedExpression = NULL_VALUE;
                else
                {
                    parsedExpression = "@parm" + parsedParms.Count;
                    parsedParms.Add(new SqlParsedParameter
                    {
                        ParameterName = parsedExpression,
                        Value = value
                    });
                }
            }
            else if (expression is MemberExpression)
            {
                var memberExpression = (MemberExpression) expression;
                if (memberExpression.Expression?.NodeType == ExpressionType.Parameter)
                {
                    parsedExpression = ParseParameterExpression(tableName, memberExpression);
                }
                else
                {
                    var value = GetValueFromExpresion(memberExpression);

                    if (value == null)
                        parsedExpression = NULL_VALUE;
                    else
                    {
                        parsedExpression = "@parm" + parsedParms.Count;
                        parsedParms.Add(new SqlParsedParameter
                        {
                            ParameterName = parsedExpression,
                            Value = value
                        });
                    }


                }
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

                    if (memberExpression?.Expression?.NodeType == ExpressionType.Parameter)
                    {
                        parsedExpression = ParseParameterExpression(tableName, memberExpression);
                    }
                    else if (memberExpression != null)
                    {
                        var value = GetValueFromExpresion(memberExpression);

                        if (value == null)
                            parsedExpression = NULL_VALUE;
                        else
                        {
                            parsedExpression = "@parm" + parsedParms.Count;
                            parsedParms.Add(new SqlParsedParameter
                            {
                                ParameterName = parsedExpression,
                                Value = value
                            });
                        }
                    }
                    else
                    {
                        
                        throw new Exception("This type of convert is not currently supported");
                    }

                }
                else
                {
                    var value = GetValueFromExpresion(expression);
                    if (value == null)
                    {
                        parsedExpression = NULL_VALUE;
                    }
                    else
                    {
                        parsedExpression = "@parm" + parsedParms.Count;
                        parsedParms.Add(new SqlParsedParameter
                        {
                            ParameterName = parsedExpression,
                            Value = value
                        });
                    }
                }


            }
            else
            {
                throw new ArgumentException(
                    $"Unknown expression type: {expression.GetType()}, Node Type {expression.NodeType}. Could not be converted to a binary expression");
            }
            return parsedExpression;
        }

        private string ParseParameterExpression(string tableName, MemberExpression memberExpression)
        {
            return $"{tableName}.[{memberExpression.Member.Name}]";
        }
        private object GetValueFromExpresion(Expression expression)
        {
            object value;
            var function = Expression.Lambda(expression).Compile();
            value = function.DynamicInvoke();
            return value;
        }

        
        private object GetValueFromConstantExpression(ConstantExpression constantExpression)
        {
            return constantExpression.Value;
        }
    }
}
