using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq.Expressions;

namespace ExpressiveDapper.Extensions
{
    public static class NodeTypeExtension
    {
        public static string GetSql(this ExpressionType expressionType, bool checkingNulls = false)
        {
            string sql;
            switch (expressionType)
            {
                case ExpressionType.AndAlso:
                    sql = " and ";
                    break;
                case ExpressionType.OrElse:
                    sql = " or ";
                    break;
                case ExpressionType.Equal:
                    sql = checkingNulls ? " is " : " = ";
                    break;
                case ExpressionType.GreaterThan:
                    sql = " > ";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sql = " >= ";
                    break;
                case ExpressionType.LessThan:
                    sql = " < ";
                    break;
                case ExpressionType.LessThanOrEqual:
                    sql = " <= ";
                    break;
                    case ExpressionType.NotEqual:
                    sql = checkingNulls ? " is not " : " != ";
                    break;
                default:
                    throw new ArgumentException($"Could not parse expression type {expressionType}");

            }

            return sql;
        }
    }
}
