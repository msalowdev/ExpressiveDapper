using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace ExpressiveDapper.Expressions
{
    public class SqlParsedExpression
    {
        public string SqlStatement { get;  }

        public Dictionary<string, object> Parameters { get;}

        public SqlParsedExpression(string sql, Dictionary<string, object> parameters)
        {
            Parameters = parameters;
            SqlStatement = sql;
        }
    }
}
