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
        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "FromPipelineInput")]
        public SmoLogin InputObject { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "Default")]
        public string LoginName { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            if (!string.IsNullOrEmpty(this.LoginName) && this.GetLoginFromName(this.LoginName, out SmoLogin found))
                this.InputObject = found;

            if (this.Force || base.ShouldProcess(this.InputObject.Name + " on " + _server.Name, "Remove"))
            {
                var activeProcs = SqlProcessCollection.GetProcesses(this.InputObject.Name, _server);
                if (activeProcs.Count > 0)
                {
                    activeProcs.KillAll();
                }

                this.InputObject.Drop();
                _server.Refresh();
            }
        }

        #endregion

        #region CMDLET METHODS
        private bool GetLoginFromName(string loginName, out SmoLogin outLogin)
        {
            bool contains = _server.Logins.Contains(loginName);
            outLogin = null;
            if (contains)
                outLogin = _server.Logins[loginName];

            return contains;
        }

        #endregion
    }
}