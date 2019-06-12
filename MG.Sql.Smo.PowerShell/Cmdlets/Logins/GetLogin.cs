using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "Login", ConfirmImpact = ConfirmImpact.None)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SmoLogin))]
    public class GetLogin : BaseSqlCmdlet
    {
        private Server _s;

        #region PARAMETERS
        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true)]
        [Alias("Name", "LoginID", "SID")]
        [SupportsWildcards]
        public LoginIdentity Identity { get; set; }

        [Parameter(Mandatory = false)]
        public LoginType[] Type { get; set; }

        [Parameter(Mandatory = false, DontShow = true)]
        public Server ServerObject { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            if (!this.MyInvocation.BoundParameters.ContainsKey("ServerObject"))
            {
                base.BeginProcessing();
                _s = SmoContext.Connection;
            }
            else
                _s = this.ServerObject;
        }

        protected override void ProcessRecord()
        {
            List<SmoLogin> logins = SmoLogin.GetLogins(_s);
            this.FilterByLoginNameOrSid(ref logins);
            this.FilterByLoginId(ref logins);
            this.FilterByType(ref logins);
            WriteObject(logins, true);
        }

        #endregion

        #region METHODS
        private void FilterByLoginNameOrSid(ref List<SmoLogin> logins)
        {
            if (this.Identity.IsLoginName || this.Identity.IsSecurityIdentitifer)
            {
                var wcp = new WildcardPattern((string)this.Identity, WildcardOptions.IgnoreCase);
                for (int i = logins.Count - 1; i >= 0; i--)
                {
                    SmoLogin smol = logins[i];
                    if (!wcp.IsMatch(smol.Name) && !wcp.IsMatch(smol.Sid))
                        logins.Remove(smol);
                }
            }
        }
        private void FilterByLoginId(ref List<SmoLogin> logins)
        {
            if (this.Identity.IsID)
            {
                for (int i = logins.Count - 1; i >= 0; i--)
                {
                    SmoLogin l = logins[i];
                    if (!l.ID.Equals((int)this.Identity))
                        logins.Remove(l);
                }
            }
        }

        private void FilterByType(ref List<SmoLogin> logins)
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("Type"))
            {
                for (int i = logins.Count - 1; i >= 0; i--)
                {
                    SmoLogin l = logins[i];
                    if (!this.Type.Contains(l.LoginType))
                        logins.Remove(l);
                }
            }
        }

        #endregion
    }
}

