using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Collections.Generic;

namespace SQL.SMO.Databases
{
    public class SMOTableCollection : SMOCollection<SMOTable>
    {
        public SMOTableCollection()
            : base()
        {
        }

        public SMOTableCollection(int capacity)
            : base(capacity)
        {
        }

        public SMOTableCollection(IEnumerable<SMOTable> tabs)
            : base(tabs)
        {
        }

        public SMOTableCollection(SMOTable oneTab)
            : base(oneTab)
        {
        }

        public void Add(Table msTab) =>
            base.Add((SMOTable)msTab);

        public static explicit operator SMOTableCollection(TableCollection tabCol)
        {
            var newTabCol = new SMOTableCollection(tabCol.Count);
            for (int d = 0; d < tabCol.Count; d++)
            {
                newTabCol.Add(tabCol[d]);
            }
            newTabCol.MakeReadOnly();
            return newTabCol;
        }
    }
}
