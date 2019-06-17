using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Management.Automation;
using System.Security;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "", ConfirmImpact = ConfirmImpact.None)]
    public abstract class BaseLoginCmdlet : BaseSqlCmdlet
    {
        protected private Server _server;

        #region PARAMETERS
        [Parameter(Mandatory = false, DontShow = true)]
        public virtual Server SqlServer { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (this.SqlServer == null)
            {
                base.BeginProcessing();
                _server = SmoContext.Connection;
            }
            else
                _server = this.SqlServer;
        }

        #endregion

        #region METHODS
        protected private bool GetLoginFromName(string loginName, out SmoLogin outLogin)
        {
            bool contains = _server.Logins.Contains(loginName);
            outLogin = null;
            if (contains)
                outLogin = _server.Logins[loginName];

            return contains;
        }

        #endregion
    }
}