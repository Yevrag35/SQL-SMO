using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell.Cmdlets.Bases
{
    public abstract class JobAgentBaseCmdlet : BaseSqlCmdlet
    {
        #region PARAMETERS
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByPipelineInput", ValueFromPipeline = true, DontShow = true)]
        public Microsoft.SqlServer.Management.Smo.Agent.Job InputObject { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByJobName")]
        public string JobName { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (this.MyInvocation.BoundParameters.ContainsKey("JobName"))
            {
                SmoContext.Connection.JobServer.Jobs.Cast<Microsoft.SqlServer.Management.Smo.Agent.Job>();
            }
        }

        protected override void ProcessRecord()
        {

        }

        #endregion

        #region CMDLET METHODS


        #endregion
    }
}