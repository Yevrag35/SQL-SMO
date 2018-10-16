using MG.Attributes;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace SQL.SMO.Databases
{
    public class SMODatabase : AttributeResolver, ISMOObject
    {
        private Database _db;
        private string[] _tblNames;
        private readonly CompatTable _compat;

        public string Name => _db.Name;
        public bool AutoShrink => _db.AutoShrink;
        public string Collation => _db.Collation;
        public string Compatibility => GetNameAttribute(_compat);
        public SqlSmoState State => _db.State;
        public DatabaseStatus Status => _db.Status;
        public RecoveryModel RecoveryModel => _db.RecoveryModel;
        public LogReuseWaitStatus LogReuseWaitStatus => _db.LogReuseWaitStatus;
        public Type MSType => typeof(Database);
        public string[] Tables
        {
            get
            {
                if (_tblNames == null)
                {
                    GetTableNames();
                }
                return _tblNames;
            }
        }

        internal SMODatabase(Database db)
        {
            _db = db;
            _compat = GetEnumFromValue<CompatTable>(_db.CompatibilityLevel, typeof(CompatAttribute));
        }

        internal SMOTable[] GetTables()
        {
            SMOTable[] smot = new SMOTable[_db.Tables.Count];
            for (int i = 0; i < _db.Tables.Count; i++)
            {
                Table t = _db.Tables[i];
                smot[i] = new SMOTable(t);
            }
            return smot;
        }

        internal SMOTable[] GetTables(string[] tableNames)
        {
            var smot = new SMOTable[tableNames.Length];
            IEnumerable<Table> tbls = _db.Tables.OfType<Table>();
            Table t = null;
            for (int i = 0; i < tableNames.Length; i++)
            {
                string s = tableNames[i];
                try
                {
                    t = tbls.Single(x =>
                        string.Equals(x.Name, s, StringComparison.OrdinalIgnoreCase)
                    );
                }
                catch
                {
                    throw new SmoException("Table '" + s + "' was not found!");
                }
                smot[i] = new SMOTable(t);
            }
            return smot;
        }

        internal protected void GetTableNames()
        {
            _tblNames = new string[_db.Tables.Count];
            for (int i = 0; i < _db.Tables.Count; i++)
            {
                Table tbl = _db.Tables[i];
                _tblNames[i] = tbl.Name;
            }
        }

        public object ShowOriginal() => _db;
    }
}
