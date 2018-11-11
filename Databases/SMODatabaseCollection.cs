using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Collections.Generic;

namespace SQL.SMO.Databases
{
    public class SMODatabaseCollection : SMOCollection<SMODatabase>
    {
        public SMODatabaseCollection()
            : base()
        {
        }

        public SMODatabaseCollection(int capacity)
            : base(capacity)
        {
        }

        public SMODatabaseCollection(IEnumerable<SMODatabase> dbs)
            : base(dbs)
        {
        }

        public SMODatabaseCollection(SMODatabase oneDB)
            : base(oneDB)
        {
        }

        public void Add(Database msDB) =>
            base.Add((SMODatabase)msDB);

        public static explicit operator SMODatabaseCollection(DatabaseCollection dbc)
        {
            var newDBC = new SMODatabaseCollection(dbc.Count);
            for (int d = 0; d < dbc.Count; d++)
            {
                newDBC.Add(dbc[d]);
            }
            return newDBC;
        }
    }
}
