using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsLifecycle.Stop, "Job", DefaultParameterSetName = "ByPipelineInput")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(void))]
    public class StopJob : JobModifyBaseCmdlet
    {
        private const string STOP = "Stop";
        private bool yesToAll = false;
        private bool noToAll = false;

        #region PARAMETERS

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (_input.CurrentRunStatus != JobExecutionStatus.Idle)
            {
                if (this.Force || base.ShouldContinue(string.Format("Stopping " + JOB_CAP, _input.Name), STOP, ref yesToAll, ref noToAll))
                {
                    WriteVerbose(string.Format("Stopping " + JOB_CAP, _input.Name));
                    _input.Stop();
                }
            }
        }

        #endregion

        #region CMDLET METHODS


        #endregion
    }
}