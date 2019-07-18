using System;
using System.Collections.Generic;
using System.Text;

namespace Pineapple.Data.InformationSchema
{
    public class Column
    {
        public string TableCatalog { get; set; }
        public string TableSchema { get; set; }

        public string TableName { get; set; }

        public string ColumnName { get; set; }
        public string DataType { get; set; }
    }
}
