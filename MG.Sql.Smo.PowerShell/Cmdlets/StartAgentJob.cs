using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsLifecycle.Start, "AgentJob", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "ByPipelineInput")]
    public class StartAgentJob : BaseSqlCmdlet
    {
        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByPipelineInput")]
        public Microsoft.SqlServer.Management.Smo.Agent.Job Job { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByJobName")]
        public string JobName { get; set; }

        [Parameter(Mandatory = false, Position = 1)]
        public object StartAt { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            if (!this.MyInvocation.BoundParameters.ContainsKey("StartAt"))
                this.Job.Start();

            else
                this.StartJobAt(this.StartAt);
        }

        #endregion

        #region CMDLET METHODS
        private void StartJobAt(object jobStep)
        {
            if (jobStep is int jobStepNo)
            {
                var js = this.Job.JobSteps.ItemById(jobStepNo);
                this.Job.Start(js.Name);
            }
            else if (jobStep is string jobName)
                this.Job.Start(jobName);
            
            else
                throw new ArgumentException(jobStep + " is not a valid job step.");
            
        }

        #endregion
    }
}