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
        public string Name { get { return _tbl.Name; } }
        public long RowCount { get { return _tbl.RowCount; } }
        public string Owner { get { return _tbl.Owner; } }
        public long ID { get { return _tbl.ID; } }
        public string FileGroup { get { return _tbl.FileGroup; } }
        public double DataSpaceUsed { get { return _tbl.DataSpaceUsed; } }
        public bool ChangeTrackingEnabled { get { return _tbl.ChangeTrackingEnabled; } }
        public bool AnsiNullsStatus { get { return _tbl.AnsiNullsStatus; } }
        public bool HasIndex { get { return _tbl.HasIndex; } }
        public double IndexSpaceUsed { get { return _tbl.IndexSpaceUsed; } }
        public bool Replicated { get { return _tbl.Replicated; } }
        public bool TrackColumnsUpdatedEnabled { get { return _tbl.TrackColumnsUpdatedEnabled; } }
        public TableEvents Events { get { return _tbl.Events; } }
        public IndexCollection Indexes { get { return _tbl.Indexes; } }
        public int MaxDegreeOfParallelism { get { return _tbl.MaximumDegreeOfParallelism; } }
        public ForeignKeyCollection ForeignKeys { get { return _tbl.ForeignKeys; } }
        public string Schema { get { return _tbl.Schema; } }
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
            SMOColumn[] smoc = new SMOColumn[_tbl.Columns.Count];
            for (int i = 0; i < _tbl.Columns.Count; i++)
            {
                Column c = _tbl.Columns[i];
                smoc[i] = new SMOColumn(c);
            }
            return smoc;
        }
        
        internal SMOColumn[] GetColumns(string[] colNames)
        {
            SMOColumn[] smoc = new SMOColumn[colNames.Length];
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
