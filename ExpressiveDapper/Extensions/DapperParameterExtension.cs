using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace ExpressiveDapper.Extensions
{
    public static class DapperParameterExtension
    {
        public static DynamicParameters ToDynamicParameters(this IDictionary<string, object> parameters)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();

            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }
            return dynamicParameters;
        }

        public static DynamicParameters ToDynamicParameters(this IDictionary<string, object> parameters,
            params IDictionary<string, object>[] additionalParmCollections)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();

            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            if (additionalParmCollections != null)
            {
                var flattenParms = additionalParmCollections.SelectMany(i => i);

                foreach (var parms in flattenParms)
                {
                    dynamicParameters.Add(parms.Key, parms.Value);
                }
            }
            return dynamicParameters;
        }
    }
}
