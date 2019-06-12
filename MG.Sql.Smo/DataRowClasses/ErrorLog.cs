using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data;

namespace MG.Sql.Smo
{
    public class ErrorLog : DataRowClass
    {
        public int ArchiveNo { get; private set; }
        public DateTime CreateDate { get; private set; }
        public string Name { get; private set; }
        public long Size { get; private set; }
        public string Urn { get; private set; }

        public ErrorLog(DataRow dataRow)
            : base(dataRow) { }

        public static DataItemCollection<ErrorLog> GetErrorLogs(Server server) => new DataItemCollection<ErrorLog>(server.EnumErrorLogs());
    }
}
