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
    [Cmdlet(VerbsCommon.Set, "Login", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true, DefaultParameterSetName = "Default")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SmoLogin))]
    public class SetLogin : BaseLoginCmdlet, IDynamicParameters
    {
        #region FIELDS/CONSTANTS
        private DynamicLibrary _dynLib;
        private string _defDb;

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "FromPipelineInput", DontShow = true)]
        public SmoLogin InputObject { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "Default")]
        public string LoginName { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice? AccountStatus { get; set; }

        [Parameter(Mandatory = false)]
        public LoginStatus? LoginStatus { get; set; }

        [Parameter(Mandatory = false)]
        public SqlRole[] AddRole { get; set; }

        [Parameter(Mandatory = false)]
        public SqlRole[] RemoveRole { get; set; }

        [Parameter(Mandatory = false)]
        public string Language { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice? PasswordExpiration { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice? PasswordPolicy { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter PassThru { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        #endregion

        #region DYNAMIC PARAMETERS
        public object GetDynamicParameters()
        {
            _dynLib = new DynamicLibrary();
            Server s = this.SqlServer == null && SmoContext.IsSet && SmoContext.IsConnected 
                ? SmoContext.Connection
                : this.SqlServer;

            var dp = new DynamicParameter<string>("DefaultDatabase", typeof(string), s.Databases.Cast<Database>().Select(x => x.Name));
            _dynLib.Add(dp);
            return _dynLib;
        }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (_dynLib != null && _dynLib.ParameterHasValue("DefaultDatabase"))
                _defDb = _dynLib.GetParameterValue<string>("DefaultDatabase");
        }

        protected override void ProcessRecord()
        {
            if (_server == null && this.InputObject != null)
                _server = this.InputObject.Parent;

            if (_server == null)
                throw new SmoContextNotSetException();

            if (!string.IsNullOrEmpty(this.LoginName) && base.GetLoginFromName(this.LoginName, out SmoLogin found))
                this.InputObject = found;

            if (this.InputObject == null)
                throw new ArgumentException("The specified login was not found.");

            if (this.Force || base.ShouldProcess(this.InputObject.Name + " on " + _server.Name, "Set"))
            {
                this.Set();
                if (this.PassThru)
                    base.WriteObject(this.InputObject);
            }
        }

        #endregion

        #region METHODS

        private void Set()
        {
            if (this.AccountStatus.HasValue && this.AccountStatus.Value == BinaryChoice.Disabled)
                this.InputObject.Disable();

            else if (this.AccountStatus.HasValue && this.AccountStatus.Value == BinaryChoice.Enabled)
                this.InputObject.Enable();

            if (!string.IsNullOrEmpty(_defDb))
                this.InputObject.DefaultDatabase = _defDb;

            if (this.LoginStatus.HasValue)
                this.InputObject.DenyWindowsLogin = Convert.ToBoolean(this.LoginStatus.Value);

            if (!string.IsNullOrEmpty(this.Language))
                this.InputObject.Language = this.Language;

            if (this.PasswordExpiration.HasValue)
                this.InputObject.SetPasswordExpiration(this.PasswordExpiration.Value);

            if (this.PasswordPolicy.HasValue)
                this.InputObject.SetPasswordPolicy(this.PasswordPolicy.Value);

            if (this.AddRole != null && this.AddRole.Length > 0)
            {
                for (int a = 0; a < this.AddRole.Length; a++)
                {
                    _server.Roles[this.AddRole[a].ToString()].AddMember(this.InputObject.Name);
                }
            }
            if (this.RemoveRole != null && this.RemoveRole.Length > 0)
            {
                for (int r = 0; r < this.RemoveRole.Length; r++)
                {
                    _server.Roles[this.RemoveRole[r].ToString()].DropMember(this.InputObject.Name);
                }
            }
            this.InputObject.Alter();
            this.InputObject.Refresh();
            _server.Refresh();
        }

        #endregion
    }
}