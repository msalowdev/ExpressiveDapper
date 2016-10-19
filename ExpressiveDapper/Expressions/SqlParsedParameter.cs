using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressiveDapper.Expressions
{
    public class SqlParsedParameter
    {
        public string ParameterName { get; set; }
        public object Value { get; set; }
    }
}
