
namespace ExpressiveDapper.SqlGeneration
{
    public class ParsedSqlProperty
    {
        public string FieldName { get; set; }
        public string ParameterName { get; set; }
        public object Value { get; set; }

        public bool IsWhereProperty { get; set; }

        public ParsedSqlProperty()
        {
            IsWhereProperty = false;
        }
    }
}
