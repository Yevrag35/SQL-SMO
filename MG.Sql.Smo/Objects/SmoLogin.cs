using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Principal;

namespace MG.Sql.Smo
{
    public class SmoLogin : SqlUserBase, IAlterable, ICreatable, IDmfFacet, IDroppable, IDropIfExists, IObjectPermission, 
        IRefreshable, IRenamable, IScriptable, ISfcValidate
    {
        #region FIELDS/CONSTANTS
        private Login _lg;
        private SmoServer _s;
        private SecurityIdentifier _secId;
        private const string IS_LOCKED = "IsLocked";
        private const string IS_PASS_EXP = "IsPasswordExpired";
        private static readonly Type LOGIN_TYPE = typeof(Login);
        private const string MUST_CHANGE = "MustChangePassword";
        private const string PASS_EXP_EN = "PasswordExpirationEnabled";
        private const string PASS_POLICY_ENF = "PasswordPolicyEnforced";
        //private const string SECRET_METHOD = "GetPropValueOptionalAllowNull";
        //private static readonly MethodInfo SecretMethod = LOGIN_TYPE.GetMethod(SECRET_METHOD, FLAGS);
        
        //private bool? 

        #endregion

        #region PROPERTIES
        public override string AsymmetricKey { get => _lg.AsymmetricKey; set => _lg.AsymmetricKey = value; }
        public override string Certificate { get => _lg.Certificate; set => _lg.Certificate = value; }
        public override DateTime CreateDate => _lg.CreateDate;
        public string Credential { get => _lg.Credential; set => _lg.Credential = value; }
        public override DateTime DateLastModified => _lg.DateLastModified;
        public string DefaultDatabase { get => _lg.DefaultDatabase; set => _lg.DefaultDatabase = value; }
        public bool DenyWindowsLogin { get => _lg.DenyWindowsLogin; set => _lg.DenyWindowsLogin = value; }
        public LoginEvents Events => _lg.Events;
        public bool HasAccess => _lg.HasAccess;
        public override int ID => _lg.ID;
        public bool IsDisabled => _lg.IsDisabled;
        public bool? IsLocked { get; private set; }
        public bool? IsPasswordExpired { get; private set; }
        public override bool IsSystemObject => _lg.IsSystemObject;
        public string Language { get => _lg.Language; set => _lg.Language = value; }
        public string LanguageAlias => _lg.LanguageAlias;
        public override LoginType LoginType => _lg.LoginType;
        public bool? MustChangePassword { get; private set; }
        public override string Name { get => _lg.Name; set => _lg.Name = value; }
        public SmoServer Parent => _s;
        public bool? PasswordExpirationEnabled { get; private set; }
        public PasswordHashAlgorithm PasswordHashAlgorithm => _lg.PasswordHashAlgorithm;
        public bool? PasswordPolicyEnforced { get; private set; }
        public override string Sid => _secId != null
            ? _secId.Value
            : null;
        public WindowsLoginAccessType WindowsLoginAccessType => _lg.WindowsLoginAccessType;

        #endregion

        #region CONSTRUCTORS
        private SmoLogin(Login login)
            : base(login)
        {
            _lg = login;
            _s = new SmoServer(login.Parent);
            if (login.WindowsLoginAccessType != WindowsLoginAccessType.NonNTLogin)
                _secId = new SecurityIdentifier(login.Sid, 0);

            this.IsLocked = this.GetPropValueEvenIfNull(IS_LOCKED);
            this.IsPasswordExpired = this.GetPropValueEvenIfNull(IS_PASS_EXP);
            this.MustChangePassword = this.GetPropValueEvenIfNull(MUST_CHANGE);
            this.PasswordExpirationEnabled = this.GetPropValueEvenIfNull(PASS_EXP_EN);
            this.PasswordPolicyEnforced = this.GetPropValueEvenIfNull(PASS_POLICY_ENF);
        }

        public SmoLogin(Server server, string loginName, LoginType loginType)
            : this(new Login(server, loginName)
            {
                LoginType = loginType
            })
        { }

        #endregion

        #region METHODS
        public Server GetParentObject() => _lg.Parent;
        public object GetSid()
        {
            return _secId == null 
                ? _lg.Sid 
                : (object)_secId;
        }

        public void SetPasswordExpiration(bool enabled)
        {
            _lg.PasswordExpirationEnabled = enabled;
            this.Alter();
            _lg.Refresh();
            this.PasswordExpirationEnabled = _lg.PasswordExpirationEnabled;
        }
        public void SetPasswordExpiration(BinaryChoice choice) => this.SetPasswordExpiration(Convert.ToBoolean(choice));

        public void SetPasswordPolicy(bool enabled)
        {
            _lg.PasswordPolicyEnforced = enabled;
            this.Alter();
            _lg.Refresh();
            this.PasswordPolicyEnforced = _lg.PasswordPolicyEnforced;
        }
        public void SetPasswordPolicy(BinaryChoice choice) => this.SetPasswordPolicy(Convert.ToBoolean(choice));

        #endregion

        #region LOGIN METHODS
        public void AddCredential(string credName) => _lg.AddCredential(credName);
        public void AddToRole(string role) => _lg.AddToRole(role);
        public void Alter()
        {
            if (_lg.Parent.Databases.Contains(_lg.DefaultDatabase))
            {
                var db = _lg.Parent.Databases[_lg.DefaultDatabase];
                if (db.IsAccessible && db.Status == Microsoft.SqlServer.Management.Smo.DatabaseStatus.Normal)
                {
                    _lg.Alter();
                }
                else
                {
                    throw new InvalidOperationException("This login's default database is currently inaccessible.");
                }
            }
        }
        public void ChangePassword(Password newPass) => _lg.ChangePassword(newPass);
        public void ChangePassword(Password oldPass, Password newPass) => _lg.ChangePassword(oldPass, newPass);
        public void ChangePassword(Password newPass, bool unlock, bool mustChange) => _lg.ChangePassword(newPass, unlock, mustChange);
        public void Create() => _lg.Create();
        public void Create(Password password) => _lg.Create(password);
        public void Create(Password password, LoginCreateOptions options) => _lg.Create(password, options);
        public void Deny(ObjectPermissionSet permissions, string granteeName) => this._lg.Deny(permissions, granteeName);
        public void Deny(ObjectPermissionSet permissions, string[] granteeNames) => this._lg.Deny(permissions, granteeNames);
        public void Deny(ObjectPermissionSet permissions, string[] granteeNames, bool cascade) => this._lg.Deny(permissions, granteeNames, cascade);
        public void Deny(ObjectPermissionSet permissions, string granteeName, bool cascade) => this._lg.Deny(permissions, granteeName, cascade);
        public void Disable() => _lg.Disable();
        public void Drop() => _lg.Drop();
        public void DropCredential(string credName) => _lg.DropCredential(credName);
        void IDropIfExists.DropIfExists() => _lg.DropIfExists();
        public void Enable() => _lg.Enable();
        public DataTable EnumAgentProxyAccounts() => _lg.EnumAgentProxyAccounts();
        public StringCollection EnumCredentials() => _lg.EnumCredentials();
        public DatabaseMapping[] EnumDatabaseMappings() => _lg.EnumDatabaseMappings();
        public ObjectPermissionInfo[] EnumObjectPermissions() => _lg.EnumObjectPermissions();
        public ObjectPermissionInfo[] EnumObjectPermissions(string grantee) => _lg.EnumObjectPermissions(grantee);
        public ObjectPermissionInfo[] EnumObjectPermissions(ObjectPermissionSet set) => _lg.EnumObjectPermissions(set);
        public ObjectPermissionInfo[] EnumObjectPermissions(string grantee, ObjectPermissionSet set) => _lg.EnumObjectPermissions(grantee, set);
        public string GetDatabaseUser(string dbName) => _lg.GetDatabaseUser(dbName);
        public override int GetHashCode() => _lg.GetHashCode();
        public string GetSqlVersionName() => _lg.GetSqlServerVersionName();
        public void Grant(ObjectPermissionSet permissions, string granteeName) => this._lg.Grant(permissions, granteeName);
        public void Grant(ObjectPermissionSet permissions, string[] granteeNames) => this._lg.Grant(permissions, granteeNames);
        public void Grant(ObjectPermissionSet permissions, string[] granteeNames, bool grantGrant) => this._lg.Grant(permissions, granteeNames, grantGrant);
        public void Grant(ObjectPermissionSet permissions, string granteeName, bool grantGrant) => this._lg.Grant(permissions, granteeName, grantGrant);
        public void Grant(ObjectPermissionSet permissions, string[] granteeNames, bool grantGrant, string asRole) => this._lg.Grant(permissions, granteeNames, grantGrant, asRole);
        public void Grant(ObjectPermissionSet permissions, string granteeName, bool grantGrant, string asRole) => this._lg.Grant(permissions, granteeName, grantGrant, asRole);
        public bool IsMember(string role) => _lg.IsMember(role);
        public StringCollection ListMembers() => _lg.ListMembers();
        public void Rename(string newName) => _lg.Rename(newName);
        public void Revoke(ObjectPermissionSet permissions, string granteeName) => this._lg.Revoke(permissions, granteeName);
        public void Revoke(ObjectPermissionSet permissions, string[] granteeNames) => this._lg.Revoke(permissions, granteeNames);
        public void Revoke(ObjectPermissionSet permissions, string[] granteeNames, bool revokeGrant, bool cascade) => this._lg.Revoke(permissions, granteeNames, revokeGrant, cascade);
        public void Revoke(ObjectPermissionSet permissions, string granteeName, bool revokeGrant, bool cascade, string asRole) => this._lg.Revoke(permissions, granteeName, revokeGrant, cascade, asRole);
        StringCollection IScriptable.Script() => _lg.Script();
        StringCollection IScriptable.Script(ScriptingOptions scriptingOptions) => _lg.Script(scriptingOptions);

        #endregion

        public static implicit operator SmoLogin(Login login) => new SmoLogin(login);
        public static List<SmoLogin> GetLogins(Server server)
        {
            var list = new List<SmoLogin>(server.Logins.Count);
            for (int i = 0; i < server.Logins.Count; i++)
            {
                list.Add(server.Logins[i]);
            }
            return list;
        }
    }
}
