using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Remove, "Login", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true, DefaultParameterSetName = "Default")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(void))]
    public class RemoveLogin : BaseLoginCmdlet
    {
        private List<SmoLogin> _logins;

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "FromPipelineInput")]
        public SmoLogin InputObject { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "Default")]
        public string[] LoginName { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _logins = new List<SmoLogin>();
        }

        protected override void ProcessRecord()
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("LoginName"))
            {
                for (int i = 0; i < this.LoginName.Length; i++)
                {
                    if (this.GetLoginFromName(this.LoginName[i], out SmoLogin tempLogin))
                        _logins.Add(tempLogin);
                }
            }
            else if (this.MyInvocation.BoundParameters.ContainsKey("InputObject"))
                _logins.Add(this.InputObject);
        }

        protected override void EndProcessing()
        {
            for (int l = 0; l < _logins.Count; l++)
            {
                if (this.Force || base.ShouldProcess(this.InputObject.Name + " on " + _server.Name, "Remove"))
                {
                    var activeProcs = SqlProcessCollection.GetProcesses(this.InputObject.Name, _server);
                    if (activeProcs.Count > 0)
                    {
                        activeProcs.KillAll();
                    }

                    this.InputObject.Drop();
                }
            }
            _server.Refresh();
        }

        #endregion
    }
}