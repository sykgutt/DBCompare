using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBCompare
{
    [Serializable]
    public class Data
    {
        public string Schema { get; set; }
        public string TableName { get; set; }
        public string FullName { get; set; }
        public string Columns { get; set; }
        public string Value { get; set; }
        public string ValueByColumns { get; set; }
    }
}
