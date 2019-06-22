using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Management.Automation;
using System.Security;

namespace MG.Sql.Smo.PowerShell
{
    public abstract class BaseLoginCmdlet : BaseSqlCmdlet
    {
        #region PARAMETERS
        [Parameter(Mandatory = false, DontShow = true)]
        public virtual Server SqlServer { get; set; }

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

        #region METHODS
        

        #endregion
    }
}