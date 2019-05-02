using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "SmoAgentServer", ConfirmImpact = ConfirmImpact.None)]
    [OutputType(typeof(JobServer))]
    public class GetSmoAgentServer : BaseSqlCmdlet
    {
        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            WriteObject(this.GetJobServer());
        }

        #endregion

        #region CMDLET METHODS
        protected private JobServer GetJobServer() => SmoContext.Connection.JobServer;

        #endregion
    }
}