using System.Collections.Generic;
using Dapper;

namespace ExpressiveDapper.Extensions
{
    public class FormattedInsertProperties
    {
        public string InsertStatement { get; set; }
        public string ParameterStatement { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public FormattedInsertProperties()
        {
            Parameters = new Dictionary<string, object>();
        }
    }
}
