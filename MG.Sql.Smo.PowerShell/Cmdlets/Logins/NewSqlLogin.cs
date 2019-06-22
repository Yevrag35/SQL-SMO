using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Management.Automation;
using System.Security;

namespace MG.Sql.Smo.PowerShell.Cmdlets.Logins
{
    [Cmdlet(VerbsCommon.New, "SqlLogin", ConfirmImpact = ConfirmImpact.Low, SupportsShouldProcess = true, DefaultParameterSetName = "None")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SmoLogin))]
    public class NewSqlLogin : BaseNewLoginCmdlet
    {
        #region PARAMETERS
        [Parameter(Mandatory = true, Position = 0)]
        public string LoginName { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public SecureString Password { get; set; }

        [Parameter(Mandatory = false)]
        [Alias("expire", "passex")]
        public SwitchParameter PasswordExpirationEnabled { get; set; }

        [Parameter(Mandatory = false)]
        [Alias("passpolicy", "usepolicy")]
        public SwitchParameter PasswordPolicyEnabled { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            SmoLogin login = base.CreateLogin(this.LoginName, LoginType.SqlLogin, this.DefaultDatabase, this.Password,
                this.PasswordExpirationEnabled.ToBool(), this.PasswordPolicyEnabled.ToBool());

            if (login != null)
            {
                base.WriteObject(login);
            }
        }

        #endregion
    }
}