using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    public class BaseUserCmdlet : BaseSqlCmdlet
    {
        #region PARAMETERS
        [Parameter(Mandatory = false, DontShow = true)]
        public Server SqlServer { get; set; }

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


        #endregion
    }
}