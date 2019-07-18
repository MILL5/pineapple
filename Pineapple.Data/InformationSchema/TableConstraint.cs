using System;
using System.Collections.Generic;
using System.Text;

namespace Pineapple.Data.InformationSchema
{
    public class TableConstraint
    {
        public string TableCatalog { get; set; }
        public string TableSchema { get; set; }

        public string TableName { get; set; }

        public string ConstraintType { get; set; }
        public string ConstraintName { get; set; }
    }
}
