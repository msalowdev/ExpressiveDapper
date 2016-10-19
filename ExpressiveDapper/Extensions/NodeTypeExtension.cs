using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressiveDapper.Extensions
{
    public static class NodeTypeExtension
    {
        public static string GetSql(this ExpressionType expressionType)
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
                    sql = " = ";
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
                default:
                    throw new ArgumentException($"Could not parse expression type {expressionType}");

            }

            return sql;
        }
    }
}
