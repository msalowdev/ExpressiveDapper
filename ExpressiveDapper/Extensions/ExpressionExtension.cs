using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressiveDapper.Extensions
{
    public static class ExpressionExtension
    {
        public static bool IsContainsCall(this Expression expression)
        {
            bool isContainsCall = false;

            var methodExpression = expression as MethodCallExpression;

            if (methodExpression != null && methodExpression.Method.Name == "Contains")
            {
                isContainsCall = true;
            }
            return isContainsCall;
        }
    }
}
