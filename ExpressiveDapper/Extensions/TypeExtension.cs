using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ExpressiveDapper.TableAttribute;

namespace ExpressiveDapper.Extensions
{
    public static class TypeExtension
    {
        public static List<PropertyInfo> GetSqlPropertyInfo(this Type type)
        {
            return type.GetProperties(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public).Where(i => i.GetCustomAttribute<IgnoreAttribute>() == null).ToList();
        }
        public static List<PropertyInfo> GetSqlPropertyInfo(this Type type, List<string> propertiesToIgnore )
        {
            return type.GetProperties(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public).Where(i => i.GetCustomAttribute<IgnoreAttribute>() == null && propertiesToIgnore.All(p => i.Name != p)).ToList();
        }

        public static List<string> GetPropertiesFromExpression<T>(this Expression<Func<T, object>> expression)
        {
            var propertyNames = new List<string>();
            NewArrayExpression array = expression.Body as NewArrayExpression;
            foreach (object obj in (IEnumerable<object>)(array.Expressions))
            {
                propertyNames.Add(obj.ToString());
                //Debug.Write(obj.ToString());
            }

            return propertyNames;
        }

        public static List<string> GetPropertyNames(this Type type)
        {
            return
                type.GetProperties(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public)
                    .Select(i => i.Name)
                    .ToList();
        }

        public static List<string> GetInvalidPropertyNames(this Type type)
        {
            return
                type.GetProperties(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public)
                    .Where(i => i.GetCustomAttribute<IgnoreAttribute>() != null)
                    .Select(i => i.Name)
                    .ToList();
        }

        public static string GetName(this Type type)
        {
            string fqTableName = string.Empty;
            SchemaAttribute schemaAttribute = type.GetCustomAttribute<SchemaAttribute>();

            fqTableName = schemaAttribute == null
                ? $"[{type.Name}]"
                : $"[{schemaAttribute.Name}].[{type.Name}]";

            return fqTableName;
        }
    }
}
