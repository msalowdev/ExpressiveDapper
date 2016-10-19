using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dapper;
using ExpressiveDapper.Extensions;
using ExpressiveDapper.Interfaces;
using ExpressiveDapper.TableAttribute;

namespace ExpressiveDapper.SqlGeneration
{
    public static class TypeParser
    {
        //Todo: add support for stashing type to field maps.
        public static string BuildSqlForSelect(Type type)
        {
            var tableName = type.GetName();
            List<PropertyInfo> tableProperties = type.GetSqlPropertyInfo();

            return
         string.Join(",",
         tableProperties.Select(i => $"{tableName}.[{i.Name}]"));
        }

        public static FormattedInsertProperties BuildSqlForInsert(ITable table)
        {
            FormattedInsertProperties formattedProperties = new FormattedInsertProperties();
            var type = table.GetType();
            List<PropertyInfo> tableProperties = type.GetSqlPropertyInfo();
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            foreach (var tableProperty in tableProperties)
            {
                parameters.Add($"@{tableProperty.Name}_parm", tableProperty.GetValue(table));
            }
            formattedProperties.InsertStatement = string.Join(",",
                tableProperties.Select(i => $"[{i.Name}]"));

            formattedProperties.ParameterStatement = string.Join(",", parameters.Keys);

            formattedProperties.Parameters = parameters;

            return formattedProperties;
        }
        public static FormattedUpdateProperties BuildSqlForUpdate(ITable table)
        {

            FormattedUpdateProperties formattedProperties = new FormattedUpdateProperties();

            var type = table.GetType();
            //Can't save primarky keys.
            List<PropertyInfo> tableProperties = type.GetSqlPropertyInfo().Where(i => i.GetCustomAttribute<PrimaryKeyAttribute>() == null).ToList();
            
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            List<Tuple<string, string>> fieldParameterMap = new List<Tuple<string, string>>();

            foreach (var tableProperty in tableProperties)
            {
                var parameterName = $"@{tableProperty.Name}_parm";
                parameters.Add(parameterName, tableProperty.GetValue(table));
                fieldParameterMap.Add(new Tuple<string, string>(tableProperty.Name, parameterName));
            }


            formattedProperties.UpdateStatement = string.Join(",",
                fieldParameterMap.Select(i => $"[{i.Item1}]={i.Item2}"));

            formattedProperties.Parameters = parameters;
            return formattedProperties;
        }
        public static FormattedUpdateProperties ParseDynamicForUpdate(dynamic objectToUpdate)
        {
            FormattedUpdateProperties formattedProperties = new FormattedUpdateProperties();

            Type tableType = objectToUpdate.GetType();
            List<PropertyInfo> tableProperties = tableType.GetSqlPropertyInfo();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            List<Tuple<string, string>> fieldParameterMap = new List<Tuple<string, string>>();

            foreach (var tableProperty in tableProperties)
            {
                var parameterName = $"@{tableProperty.Name}_parm";
                parameters.Add(parameterName, tableProperty.GetValue(objectToUpdate));
                fieldParameterMap.Add(new Tuple<string, string>(tableProperty.Name, parameterName));
            }


            formattedProperties.UpdateStatement = string.Join(",",
                fieldParameterMap.Select(i => $"[{i.Item1}]={i.Item2}"));

            formattedProperties.Parameters = parameters;
            return formattedProperties;
        }
    }
}
