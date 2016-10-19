
using Dapper;

namespace ExpressiveDapper.SqlGeneration
{
    public class SqlStatement
    {
        public string Statement { get; set; }
        public DynamicParameters Parameters { get; set; }
    }
}
