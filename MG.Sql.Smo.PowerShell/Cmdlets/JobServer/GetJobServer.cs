using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "JobServer", ConfirmImpact = ConfirmImpact.None)]
    [OutputType(typeof(JobServer))]
    public class GetJobServer : BaseSqlCmdlet
    {
        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            WriteObject(this.RetrieveJobServer());
        }

        #endregion

        #region CMDLET METHODS
        protected private JobServer RetrieveJobServer() => SmoContext.Connection.JobServer;

        #endregion
    }
}