using MG.Sql.Smo.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace MG.Sql.Smo.PowerShell.Cmdlets.Processes
{
    [Cmdlet(VerbsLifecycle.Stop, "Process", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true, DefaultParameterSetName = "ByProcessId")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(void))]
    public class StopProcess : BaseServerSqlCmdlet
    {
        #region FIELDS/CONSTANTS
        private List<int> _ids;

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "FromPipelineInput", DontShow = true)]
        public SqlProcess InputObject { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByProcessId")]
        public int[] ProcessId { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (_server == null)
                throw new SmoContextNotSetException();

            _ids = new List<int>();
        }

        protected override void ProcessRecord()
        {
            if (this.InputObject != null)
                _ids.Add(this.InputObject.Spid);

            else
                _ids.AddRange(this.ProcessId);
        }

        protected override void EndProcessing()
        {
            for (int i = 0; i < _ids.Count; i++)
            {
                int procId = _ids[i];
                if (this.Force || base.ShouldProcess(_server.Name, "Kill ProcessId: " + procId))
                {
                    try
                    {
                        _server.KillProcess(procId);
                    }
                    catch (Exception e)
                    {
                        base.ThrowInnerException(e);
                    }
                }
                    
            }
        }

        #endregion

        #region METHODS


        #endregion
    }
}