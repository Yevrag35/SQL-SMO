using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Collections.Generic;

namespace SQL.SMO.Databases
{
    public class SMOColumnCollection : SMOCollection<SMOColumn>
    {
        public SMOColumnCollection()
            : base()
        {
        }

        public SMOColumnCollection(int capacity)
            : base(capacity)
        {
        }

        public SMOColumnCollection(IEnumerable<SMOColumn> tabs)
            : base(tabs)
        {
        }

        public SMOColumnCollection(SMOColumn oneCol)
            : base(oneCol)
        {
        }

        public void Add(Column msCol) =>
            base.Add((SMOColumn)msCol);

        public static explicit operator SMOColumnCollection(ColumnCollection colCol)
        {
            var newColCol = new SMOColumnCollection(colCol.Count);
            for (int d = 0; d < colCol.Count; d++)
            {
                newColCol.Add(colCol[d]);
            }
            return newColCol;
        }
    }
}
