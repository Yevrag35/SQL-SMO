using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Rename, "Login", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true, DefaultParameterSetName = "Default")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SmoLogin))]
    public class RenameLogin : BaseLoginCmdlet
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "FromPipelineInput", DontShow = true)]
        public SmoLogin InputObject { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "Default")]
        public string LoginName { get; set; }

        [Parameter(Mandatory = true)]
        public string NewName { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter PassThru { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            if (_server == null && this.InputObject != null)
                _server = this.InputObject.Parent;

            if (_server == null)
                throw new SmoContextNotSetException();

            if (this.InputObject.LoginType != LoginType.SqlLogin)
                throw new ArgumentException("Only SQL logins can be renamed.");

            string msg = string.Format("Renaming '{0}' to '{1}'", this.InputObject.Name, this.NewName);
            if (this.Force || base.ShouldProcess(_server.Name, msg))
            {
                this.InputObject.Rename(this.NewName);
                this.InputObject.Alter();
                this.InputObject.Refresh();
                _server.Refresh();

                if (this.PassThru)
                    base.WriteObject(this.InputObject);
            }
        }

        #endregion
    }
}