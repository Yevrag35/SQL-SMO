using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MG.Sql.Smo
{
    public class SqlProcess : DataRowClass
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PROPERTIES
        public short BlockingSpid { get; private set; }
        public string Command { get; private set; }
        public int Cpu { get; private set; }
        public string Database { get; private set; }
        public short ExecutionContextID { get; private set; }
        public string Host { get; private set; }
        public bool IsSystem { get; private set; }
        public string Login { get; private set; }
        public int MemUsage { get; private set; }
        public string Name { get; private set; }
        public string Program { get; private set; }
        public int Spid { get; private set; }
        public ProcessStatus? Status { get; private set; }
        public string Urn { get; private set; }

        #endregion

        #region CONSTRUCTORS
        public SqlProcess(DataRow dataRow)
            : base(dataRow) { }

        #endregion

        #region PUBLIC METHODS

        internal void Kill(Server server) => server.KillProcess(this.Spid);

        #endregion

        #region BACKEND/PRIVATE METHODS


        #endregion
    }
}