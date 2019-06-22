using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.New, "Credential", ConfirmImpact = ConfirmImpact.Low, SupportsShouldProcess = true, 
        DefaultParameterSetName = "IdentityAndPassword")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(Credential))]
    public class NewCredential : BaseCredentialCmdlet
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "IdentityAndPassword")]
        public string Identity { get; set; }

        [Parameter(Mandatory = true, Position = 2, ParameterSetName = "IdentityAndPassword")]
        public SecureString Password { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "ByCredential")]
        [Alias("PSCredential")]
        public AnyCredential Credential { get; set; }

        [Parameter(Mandatory = false)]
        public MappedClassType MappedClassType = MappedClassType.None;

        [Parameter(Mandatory = false)]
        public string ProviderName { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (ParameterSetName == "IdentityAndPassword")
                this.Credential = new AnyCredential(this.Identity, this.Password);
            
        }

        protected override void ProcessRecord()
        {
            Credential cred = this.FormatCredentialObject();
            if (base.ShouldProcess(_server.Name, "Creating credential '" + cred.Name + "'"))
            {
                try
                {
                    cred.Create(this.Credential.UserName, this.Credential.Password);
                }
                catch (FailedOperationException foe)
                {
                    base.ThrowInnerException(foe);
                }
                _server.Credentials.Refresh();

                base.WriteObject(cred);
            }
        }

        #endregion

        #region METHODS
        private Credential FormatCredentialObject()
        {
            var cred = new Credential(_server, this.Name)
            {
                MappedClassType = this.MappedClassType
            };
            if (!string.IsNullOrEmpty(this.ProviderName))
                cred.ProviderName = this.ProviderName;

            return cred;
        }

        #endregion
    }
}