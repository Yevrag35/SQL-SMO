using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace SQL.SMO.Databases
{
    public class SMOColumn : ISMOWrapper
    {
        private Column _c;
        private static readonly Type _type = typeof(Column);

        public string Database { get; }
        public string Table { get; }
        public string Name => _c.Name;
        public long ID => _c.ID;
        public DataType DataType => _c.DataType;
        public bool IsEncrypted => _c.IsEncrypted;
        public bool Nullable => _c.Nullable;
        public Type OriginalType => _type;

        internal SMOColumn(Column c)
        {
            _c = c;
            var par = (Table)_c.Parent;
            Database = par.Parent.Name;
            Table = par.Name;
        }

        public SMODatabase GetParentDatabase() => (SMODatabase)(((Table)_c.Parent).Parent as Database);
        public SMOTable GetParentTable() => (SMOTable)(_c.Parent as Table);


        public object ShowOriginal() => _c;
        public void Load(params string[] propertyNames) => throw new NotImplementedException();

        public static implicit operator Column(SMOColumn smoc) => smoc.ShowOriginal() as Column;
        public static explicit operator SMOColumn(Column col) => new SMOColumn(col);
    }
}
