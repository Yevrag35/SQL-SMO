using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "ServerConfig", ConfirmImpact = ConfirmImpact.None)]
    [OutputType(typeof(SmoConfiguration))]
    public class GetServerConfig : BaseSqlCmdlet
    {
        #region PARAMETERS


        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord() => WriteObject(this.GetConfig(), false);

        #endregion

        #region CMDLET METHODS
        protected private SmoConfiguration GetConfig() => new SmoConfiguration(SmoContext.Connection.Configuration);

        #endregion
    }
}