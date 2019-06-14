using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    public abstract class BaseLoginCmdlet : BaseSqlCmdlet
    {
        protected private Server _server;

        #region PARAMETERS
        [Parameter(Mandatory = false, DontShow = true)]
        public Server ServerObject { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (this.ServerObject == null)
            {
                base.BeginProcessing();
                _server = SmoContext.Connection;
            }
            else
                _server = this.ServerObject;
        }

        protected override void ProcessRecord()
        {

        }

        #endregion

        #region CMDLET METHODS


        #endregion
    }
}