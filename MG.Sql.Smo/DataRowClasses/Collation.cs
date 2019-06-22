using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data;

namespace MG.Sql.Smo
{
    public class Collation : DataRowClass
    {
        public int CodePage { get; private set; }
        public CollationVersion CollationVersion { get; private set; }
        public int ComparisonStyle { get; private set; }
        public string Description { get; private set; }
        public int LocaleID { get; private set; }
        public string Name { get; private set;}
        public string Urn { get; private set; }

        public Collation(DataRow dataRow)
            : base(dataRow) { }
    }
}
