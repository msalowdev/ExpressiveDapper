using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressiveDapper.Extensions
{
    public static class FormatExtension
    {
        public static string FormatForSql(this object value)
        {
            string formatedValue;
            if (value is string)
            {
                formatedValue = $"'{value}'";
            }
            else
            {
                formatedValue = value.ToString();
            }

            return formatedValue;
        }
    }
}
