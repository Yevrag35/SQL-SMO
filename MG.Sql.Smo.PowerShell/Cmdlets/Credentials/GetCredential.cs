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
    [Cmdlet(VerbsCommon.Get, "Credential", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "ByName")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(Credential))]
    public class GetCredential : BaseCredentialCmdlet
    {
        #region PARAMETERS
        [Parameter(Mandatory = true, ParameterSetName = "ByIdentity")]
        [SupportsWildcards]
        [Alias("Id")]
        public CredentialIdentity[] Identity { get; set; }

        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "ByName")]
        public string[] Name { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            var creds = base.GetCredentials();
            this.Filter(ref creds);
            base.WriteObject(creds, true);
        }

        #endregion

        #region METHODS
        private void Filter(ref List<Credential> list)
        {
            if (this.Name != null && this.Name.Length > 0)
            {
                for (int a = 0; a < this.Name.Length; a++)
                {
                    var wcp = new WildcardPattern(this.Name[a], WildcardOptions.IgnoreCase);
                    list.RemoveAll(x => !wcp.IsMatch(x.Name));
                }
            }
            else if (this.Identity != null && this.Identity.Length > 0)
            {
                for (int i = 0; i < this.Identity.Length; i++)
                {
                    var cid = this.Identity[i];
                    if (cid.IsStringId)
                    {
                        var wcp = new WildcardPattern((string)cid, WildcardOptions.IgnoreCase);
                        list.RemoveAll(x => !wcp.IsMatch(x.Identity));
                    }
                    else
                    {
                        int id = (int)cid;
                        list.RemoveAll(x => !x.ID.Equals(id));
                    }
                }
            }
        }

        #endregion
    }
}