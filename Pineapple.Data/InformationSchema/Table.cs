using System;
using System.Collections.Generic;
using System.Text;

namespace Pineapple.Data.InformationSchema
{
    public class Table
    {
        public string TableCatalog { get; set; }
        public string TableSchema { get; set; }
        public string TableName { get; set; }
    }
}
