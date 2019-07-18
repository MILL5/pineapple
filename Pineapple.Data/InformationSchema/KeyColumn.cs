using System.Text;

namespace Pineapple.Data.InformationSchema

{
    public class KeyColumn
    {
        public string TableCatalog { get; set; }
        public string TableSchema { get; set; }

        public string TableName { get; set; }

        public string ColumnName { get; set; }
        public int Position { get; set; }
        public string ConstraintName { get; set; }
    }
}