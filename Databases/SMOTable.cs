using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace SQL.SMO.Databases
{
    public class SMOTable : ISMOObject
    {
        private Table _tbl;
        private string[] _cols;

        public SqlSmoObject Database => _tbl.Parent;
        public string Name => _tbl.Name;
        public long RowCount => _tbl.RowCount;
        public string Owner => _tbl.Owner;
        public long ID => _tbl.ID;
        public string FileGroup => _tbl.FileGroup;
        public double DataSpaceUsed => _tbl.DataSpaceUsed;
        public bool ChangeTrackingEnabled => _tbl.ChangeTrackingEnabled;
        public bool AnsiNullsStatus => _tbl.AnsiNullsStatus;
        public bool HasIndex => _tbl.HasIndex;
        public double IndexSpaceUsed => _tbl.IndexSpaceUsed;
        public bool Replicated => _tbl.Replicated;
        public bool TrackColumnsUpdatedEnabled => _tbl.TrackColumnsUpdatedEnabled;
        public TableEvents Events => _tbl.Events;
        public IndexCollection Indexes => _tbl.Indexes;
        public int MaxDegreeOfParallelism => _tbl.MaximumDegreeOfParallelism;
        public ForeignKeyCollection ForeignKeys => _tbl.ForeignKeys;
        public string Schema => _tbl.Schema;
        public Type MSType => typeof(Table);
        public string[] Columns
        {
            get
            {
                if (_cols == null)
                {
                    GetColumnNames();
                }
                return _cols;
            }
        }

        internal SMOTable(Table t) =>
            _tbl = t;

        internal SMOColumn[] GetColumns()
        {
            var smoc = new SMOColumn[_tbl.Columns.Count];
            for (int i = 0; i < _tbl.Columns.Count; i++)
            {
                Column c = _tbl.Columns[i];
                smoc[i] = new SMOColumn(c);
            }
            return smoc;
        }
        
        internal SMOColumn[] GetColumns(string[] colNames)
        {
            var smoc = new SMOColumn[colNames.Length];
            IEnumerable<Column> cols = _tbl.Columns.OfType<Column>();
            for (int i = 0; i < colNames.Length; i ++)
            {
                string s = colNames[i];
                Column c = cols.Single(x => x.Name == s);
                if (c == null)
                {
                    throw new SmoException("Column '" + s + "' was not found!");
                }
                smoc[i] = new SMOColumn(c);
            }
            return smoc;
        }

        internal protected void GetColumnNames()
        {
            _cols = new string[_tbl.Columns.Count];
            for (int i = 0; i < _tbl.Columns.Count; i++)
            {
                _cols[i] = _tbl.Columns[i].Name;
            }
        }

        public object ShowOriginal() => _tbl;

    }
}
