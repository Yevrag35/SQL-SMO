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
    public abstract class BaseDatabaseCmdlet : BaseSqlProgressCmdlet, IDynamicParameters
    {
        #region FIELDS/CONSTANTS
        protected private DynamicLibrary _dynLib;
        internal const string DBNAME = "Database" + BaseSqlCmdlet.NAME;
        protected private MgSmoCollection<Database> _dbs;

        #endregion

        [Parameter(Mandatory = false, DontShow = true)]
        public virtual Server SqlServer { get; set; }

        #region DYNAMIC PARAMETERS
        public object GetDynamicParameters()
        {
            if (_dynLib == null && SmoContext.IsSet && SmoContext.IsConnected)
            {
                if (SmoContext.GetNullOrEmpty(SmoContext.Databases))
                    SmoContext.SetDatabases(SmoContext.Connection.Databases);

                _dynLib = new DynamicLibrary();
                IDynParam param = new DynamicParameter<Database>(DBNAME, SmoContext.Databases, x => x.Name, "Name", true)
                {
                    Mandatory = false,
                    Position = 0,
                };
                param.Aliases.Add("n");
                _dynLib.Add(param);
            }
            else if (_dynLib == null)
            {
                _dynLib = new DynamicLibrary
                {
                    {
                        DBNAME,
                        new RuntimeDefinedParameter(DBNAME, typeof(string[]), new Collection<Attribute>
                {
                    new ParameterAttribute
                    {
                        Mandatory = false,
                        Position = 0
                    }
                })
                    }
                };
            }
            return _dynLib;
        }

        #endregion

        #region CMDLET PROCESSING

        protected override void BeginProcessing()
        {
            if (this.SqlServer == null)
            {
                base.BeginProcessing();
                _server = SmoContext.Connection;
            }
            else
                _server = this.SqlServer;
        }

        #endregion

        #region CMDLET METHODS
        protected private IEnumerable<Database> RetrieveDatabases()
        {
            return _dynLib != null && _dynLib.ParameterHasValue(DBNAME)
                ? _dynLib.GetUnderlyingValues<Database>(DBNAME)
                : _dynLib.GetBackingItems<Database>(DBNAME);
        }

        #endregion
    }
}