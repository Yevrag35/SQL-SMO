using MG.Dynamic;
using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Set, "LoginPassword", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true, DefaultParameterSetName = "Default")]
    [CmdletBinding(PositionalBinding = false)]
    public class SetLoginPassword : BaseLoginCmdlet
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "FromPipelineInput", DontShow = true)]
        public SmoLogin InputObject { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "Default")]
        public string LoginName { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "FromPipelineInput")]
        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "Default")]
        [Alias("Credential", "Password")]
        public PassOrCreds PasswordOrCredential { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter MustChangePassword { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Unlock { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            if (_server == null && this.InputObject != null)
                _server = this.InputObject.Parent;

            if (!string.IsNullOrEmpty(this.LoginName) && base.GetLoginFromName(this.LoginName, out SmoLogin found))
                this.InputObject = found;

            if (this.InputObject == null)
                throw new ArgumentException("The specified login was not found.");

            if (this.Force || base.ShouldProcess(_server.Name, "Resetting password for " + this.InputObject.Name))
            {
                this.InputObject.ChangePassword(
                    (SecureString)this.PasswordOrCredential,
                    this.Unlock.ToBool(), 
                    this.MustChangePassword.ToBool()
                );
                this.InputObject.Refresh();
                _server.Refresh();
            }
        }

        #endregion
    }
}