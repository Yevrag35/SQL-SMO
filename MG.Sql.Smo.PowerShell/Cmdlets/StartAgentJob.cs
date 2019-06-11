using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsLifecycle.Start, "AgentJob", ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "ByPipelineInput", SupportsShouldProcess = true)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(void))]
    public class StartAgentJob : AgentJobModifyBaseCmdlet
    {
        private const string STEP_CAP = JOB_CAP + " at StepId {1}";
        private const string START = "Start";

        #region PARAMETERS

        [Parameter(Mandatory = false, Position = 1)]
        public object StartAt { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (!_input.IsEnabled)
            {
                if (this.Force)
                {
                    WriteWarning(_input.Name + " was previously disabled but will be enabled to start this command.");
                    _input.IsEnabled = true;
                    _input.Alter();
                }
                else
                    throw new InvalidOperationException("Cannot start a disabled job.  Use the \"-Force\" parameter if this was intended or enable the job.");
            }

            if (this.MyInvocation.BoundParameters.ContainsKey(START + "At"))
                this.StartJobAt(this.StartAt);

            else if (Force || base.ShouldProcess(string.Format(JOB_CAP, _input.Name), START))
                _input.Start();
        }

        #endregion

        #region CMDLET METHODS

        private void StartJobAt(object jobStep)
        {
            JobStep js = base.GetJobStep(jobStep);
            if (js != null && (this.Force || base.ShouldProcess(string.Format(STEP_CAP, _input.Name, js.ID), START)))
                _input.Start(js.Name);

            else if (js == null)
                throw new ArgumentException(jobStep + " is not a valid job step.");
        }

        #endregion
    }
}