using MG.Sql.Smo.Exceptions;
using MG.Dynamic;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell.Cmdlets.Users
{
    [Cmdlet(VerbsCommon.Add, "DbUser", ConfirmImpact = ConfirmImpact.Low, SupportsShouldProcess = true, DefaultParameterSetName = "ByUserName")]
    public class AddDbUser : BaseUserCmdlet, IDynamicParameters
    {
        private DynamicLibrary _dynLib;
        private IEnumerable<Database> _dbs;
        private List<SqlUserBase> _users;

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "BySmoUserBase")]
        public SqlUserBase UserObject { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByUserName")]
        public string[] UserName { get; set; }

        #endregion

        #region DYNAMIC PARAMETERS
        public object GetDynamicParameters()
        {
            if (_dynLib == null && (this.SqlServer != null || (SmoContext.IsSet && SmoContext.IsConnected)))
            {
                Server s = this.SqlServer != null
                    ? this.SqlServer
                    : SmoContext.Connection;

                _dynLib = new DynamicLibrary();

                var dp = new DynamicParameter<Database>("Database", s.Databases.Cast<Database>(), x => x.Name, "Name", true)
                {
                    Mandatory = true
                };
                _dynLib.Add(dp);
            }
            return _dynLib;
        }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _dbs = _dynLib.GetUnderlyingValues<Database>("Database");
            _users = new List<SqlUserBase>();
        }

        protected override void ProcessRecord()
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("UserObject"))
                _users.Add(this.UserObject);

            else
            {
                for (int i = 0; i < this.UserName.Length; i++)
                {

                }
            }
        }

        #endregion

        #region CMDLET METHODS
        private SqlUserBase GetUser(string userName)
        {
            if (_server.Logins.Contains(userName))
            {
                return (SqlUserBase)_server.Logins[userName];
            }
        }

        #endregion
    }
}