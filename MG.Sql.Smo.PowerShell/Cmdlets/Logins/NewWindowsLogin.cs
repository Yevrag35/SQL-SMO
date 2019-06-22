using Microsoft.PowerShell.Commands;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell.Cmdlets.Logins
{
    [Cmdlet(VerbsCommon.New, "WindowsLogin", ConfirmImpact = ConfirmImpact.Low, SupportsShouldProcess = true, DefaultParameterSetName = "None")]
    [OutputType(typeof(SmoLogin))]
    [CmdletBinding(PositionalBinding = false)]
    public class NewWindowsLogin : BaseNewLoginCmdlet
    {
        private bool oneTimeNB = false;
        private string netBiosName;

        #region PARAMETERS
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public ADAllowableInputObject LoginName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "LocalUser")]
        public SwitchParameter IsLocalUser { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "LocalGroup")]
        public SwitchParameter IsLocalGroup { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            if (this.MyInvocation.PipelinePosition > 1)
                oneTimeNB = true;

            if (!this.LoginName.FromADObject && !this.IsLocalUser && !this.IsLocalGroup)
                this.ConditionOne();

            else if (this.IsLocalUser)
            {
                this.LoginName.Type = LoginType.WindowsUser;
                this.LoginName.IsLocal = true;
                this.LoginName.NetBiosName = _server.NetName;
            }

            else if (this.IsLocalGroup)
            {
                this.LoginName.Type = LoginType.WindowsGroup;
                this.LoginName.IsLocal = true;
                this.LoginName.NetBiosName = _server.NetName;
            }

            if (!this.LoginName.Type.HasValue)
                throw new ArgumentException("Could not determine what kind of Windows user this was.");

            string login = this.GetLoginName(this.LoginName);
            SmoLogin smol = base.CreateLogin(login, this.LoginName.Type.Value, this.DefaultDatabase);
            if (smol != null)
            {
                WriteObject(smol);
            }
        }

        #endregion

        #region CMDLET METHODS
        private void ConditionOne()
        {
            if (this.LoginName.LoginName.Contains(@"\"))
            {
                string[] strs = this.LoginName.LoginName.Split(new string[1] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
                this.LoginName.LoginName = strs.Last();
                this.LoginName.NetBiosName = strs.First().ToUpper();
            }

            string cls = SmoContext.FindADObjectClassFromSamAccountName(this.LoginName.LoginName, out string dn);
            this.LoginName.Type = SmoContext.FindADLoginFromObjectClass(cls);
            this.LoginName.FromADObject = true;
            if (!string.IsNullOrEmpty(dn))
                this.LoginName.DistinguishedName = dn;
        }

        private string GetLoginName(ADAllowableInputObject id)
        {
            if (id.FromADObject && string.IsNullOrEmpty(id.NetBiosName))
            {
                if (!oneTimeNB || string.IsNullOrEmpty(netBiosName))
                    netBiosName = SmoContext.GetNetBiosFromDn(id.DistinguishedName);

                id.NetBiosName = netBiosName;
            }

            return id.AsLoginName(_server);
        }

        #endregion
    }
}