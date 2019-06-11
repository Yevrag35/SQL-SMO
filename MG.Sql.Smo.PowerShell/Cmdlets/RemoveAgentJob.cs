using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Remove, "AgentJob", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true, DefaultParameterSetName = "ByPipelineInput")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(void))]
    public class RemoveAgentJob : AgentJobModifyBaseCmdlet
    {
        private JobServer _js;

        #region PARAMETERS
        [Parameter(Mandatory = false, DontShow = true)]
        public JobServer JobServer { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _js = !this.MyInvocation.BoundParameters.ContainsKey("JobServer") 
                ? SmoContext.Connection.JobServer 
                : this.JobServer;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

        }

        #endregion

        #region CMDLET METHODS
        //private void RemoveJob(SmoJob job, string where, bool isGroup)
        //{
        //    string format = "Job - {0} on {1}";
        //    if (this.Force || base.ShouldProcess(string.Format(format, job.Name, where), "Remove"))
        //    {
        //        if (!isGroup)
        //        {
        //            job.RemoveFromTargetServer(where);
        //        }
        //        else
        //        {
        //            job.RemoveFromTargetServerGroup(where);
        //        }
        //    }
        //}

        #endregion
    }
}