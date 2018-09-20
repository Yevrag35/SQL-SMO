using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace SQL.SMO.Databases
{
    public class SMOColumn : ISMOObject
    {
        private Column _c;

        public string Database => ((Table)_c.Parent).Parent.Name;
        public string Table => ((Table)_c.Parent).Name;
        public string Name { get { return _c.Name; } }
        public long ID { get { return _c.ID; } }
        public DataType DataType { get { return _c.DataType; } }
        public bool IsEncrypted { get { return _c.IsEncrypted; } }
        public bool Nullable { get { return _c.Nullable; } }
        public Type MSType => typeof(Column);

        internal SMOColumn(Column c) =>
            _c = c;

        public object ShowOriginal() => _c;
    }
}
