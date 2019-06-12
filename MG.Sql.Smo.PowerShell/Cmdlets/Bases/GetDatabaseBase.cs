using MG.Dynamic;
using MG.Progress.PowerShell;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    public abstract class GetDatabaseBase : BaseSqlProgressCmdlet, IDynamicParameters
    {
        #region FIELDS/CONSTANTS
        protected private DynamicLibrary _dynLib;
        internal const string DBNAME = "Database" + BaseSqlCmdlet.NAME;

        #endregion

        #region CMDLET PROCESSING
        public object GetDynamicParameters()
        {
            if (SmoContext.IsSet && SmoContext.IsConnected && _dynLib == null)
            {
                _dynLib = new DynamicLibrary();
                IDynParam param = new DynamicParameter<Database>(DBNAME, SmoContext.Connection.Databases.Cast<Database>(), x => x.Name, "Name", true)
                {
                    Mandatory = false,
                    Position = 0,
                    SupportsWildcards = true
                };
                param.Aliases.Add("n");
                _dynLib.Add(param);
            }
            return _dynLib;
        }

        protected override void BeginProcessing() => base.BeginProcessing();

        #endregion

        #region CMDLET METHODS
        protected private IEnumerable<Database> RetrieveDatabases()
        {
            return _dynLib != null && _dynLib.ParameterHasValue(DBNAME)
                ? _dynLib.GetUnderlyingValues<Database>(DBNAME)
                : SmoContext.Connection.Databases.Cast<Database>();
        }

        #endregion
    }
}