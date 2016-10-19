using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace ExpressiveDapper.Extensions
{
    public class FormattedUpdateProperties
    {
        public string UpdateStatement { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public FormattedUpdateProperties()
        {
            Parameters = new Dictionary<string, object>();
        }
    }
}
