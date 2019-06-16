using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Management.Automation;
using System.Security;

namespace MG.Sql.Smo.PowerShell
{
    public abstract class BaseLoginCmdlet : BaseSqlCmdlet
    {
        protected private Server _server;

        #region PARAMETERS
        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty()]
        public string DefaultDatabase = "master";

        [Parameter(Mandatory = false)]
        public SwitchParameter Disable { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty()]
        public string Language = "us_english";

        [Parameter(Mandatory = false, DontShow = true)]
        public Server SqlServer { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (this.SqlServer == null)
            {
                base.BeginProcessing();
                _server = SmoContext.Connection;
            }
            else
                _server = this.SqlServer;
        }

        #endregion

        #region CMDLET METHODS
        protected private virtual SmoLogin CreateLogin(string login, LoginType type, string defaultDb, SecureString pass = null, bool? passExpEnabled = null, bool? passPolicyEnabled = null)
        {
            var sqlLogin = new Login(_server, login)
            {
                DefaultDatabase = defaultDb,
                Language = this.Language,
                LoginType = type
            };

            if (passExpEnabled.HasValue)
                sqlLogin.PasswordExpirationEnabled = passExpEnabled.Value;

            if (passPolicyEnabled.HasValue)
                sqlLogin.PasswordPolicyEnforced = passPolicyEnabled.Value;

            if (base.ShouldProcess(_server.Name, "New login for user/group \"" + sqlLogin.Name + "\""))
            {
                if (pass == null)
                    sqlLogin.Create();

                else
                    sqlLogin.Create(pass, LoginCreateOptions.None);

                sqlLogin.Refresh();
                if (this.Disable.ToBool())
                {
                    sqlLogin.Disable();
                    sqlLogin.Alter();
                }

                _server.Refresh();
                return sqlLogin;
            }
            else
                return null;
        }

        #endregion
    }
}