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
    public class SmoLogin : IAlterable, ICreatable, IDmfFacet, IDroppable, IDropIfExists, IObjectPermission, ILoginOptions, 
        IRefreshable, IRenamable, IScriptable, ISfcValidate
    {
        #region FIELDS/CONSTANTS
        private Login _lg;
        private SmoServer _s;
        private SecurityIdentifier _secId;

        #endregion

        #region PROPERTIES
        public string AsymmetricKey { get => _lg.AsymmetricKey; set => _lg.AsymmetricKey = value; }
        public string Certificate { get => _lg.Certificate; set => _lg.Certificate = value; }
        public DateTime CreateDate => _lg.CreateDate;
        public string Credential { get => _lg.Credential; set => _lg.Credential = value; }
        public DateTime DateLastModified => _lg.DateLastModified;
        public string DefaultDatabase { get => _lg.DefaultDatabase; set => _lg.DefaultDatabase = value; }
        public bool DenyWindowsLogin { get => _lg.DenyWindowsLogin; set => _lg.DenyWindowsLogin = value; }
        public LoginEvents Events => _lg.Events;
        public bool HasAccess => _lg.HasAccess;
        public int ID => _lg.ID;
        public bool IsDisabled => _lg.IsDisabled;
        public bool IsLocked => _lg.IsLocked;
        public bool IsPasswordExpired => _lg.IsPasswordExpired;
        public bool IsSystemObject => _lg.IsSystemObject;
        public string Language { get => _lg.Language; set => _lg.Language = value; }
        public string LanguageAlias => _lg.LanguageAlias;
        public LoginType LoginType { get => _lg.LoginType; set => _lg.LoginType = value; }
        public bool MustChangePassword => _lg.MustChangePassword;
        public string Name { get => _lg.Name; set => _lg.Name = value; }
        public SmoServer Parent => _s;
        public bool PasswordExpirationEnabled { get => _lg.PasswordExpirationEnabled; set => _lg.PasswordExpirationEnabled = value; }
        public PasswordHashAlgorithm PasswordHashAlgorithm => _lg.PasswordHashAlgorithm;
        public bool PasswordPolicyEnforced { get => _lg.PasswordPolicyEnforced; set => _lg.PasswordPolicyEnforced = value; }
        public string Sid => _secId != null
            ? _secId.Value
            : null;
        public WindowsLoginAccessType WindowsLoginAccessType => _lg.WindowsLoginAccessType;

        #endregion

        #region CONSTRUCTORS
        private SmoLogin(Login login)
        {
            _lg = login;
            _s = new SmoServer(login.Parent);
            if (login.WindowsLoginAccessType != WindowsLoginAccessType.NonNTLogin)
                _secId = new SecurityIdentifier(login.Sid, 0);
        }

        #endregion

        #region METHODS
        public ExecutionManager GetExecutionManager() => _lg.ExecutionManager;
        public AbstractCollectionBase GetParentCollection() => _lg.ParentCollection;
        public Server GetParentObject() => _lg.Parent;
        public SqlPropertyCollection GetProperties() => _lg.Properties;
        public SecurityIdentifier GetSecurityIdentifier() => _secId;
        public ServerVersion GetServerVersion() => _lg.ServerVersion;
        public SqlSmoState GetState() => _lg.State;
        public object GetUserData() => _lg.UserData;
        public Urn GetUrn() => _lg.Urn;

        #endregion

        #region LOGIN METHODS
        public void AddCredential(string credName) => _lg.AddCredential(credName);
        public void AddToRole(string role) => _lg.AddToRole(role);
        public void Alter() => _lg.Alter();
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
        public List<object> Discover() => _lg.Discover();
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
        public void ExecuteWithModes(SqlExecutionModes modes, Action action) => _lg.ExecuteWithModes(modes, action);
        public string GetDatabaseUser(string dbName) => _lg.GetDatabaseUser(dbName);
        public override int GetHashCode() => _lg.GetHashCode();
        public T GetPropValueOptional<T>(string propName, T defaultValue) => _lg.GetPropValueOptional<T>(propName, defaultValue);
        public string GetSqlVersionName() => _lg.GetSqlServerVersionName();
        public IComparer<string> GetStringComparer() => _lg.GetStringComparer();
        public void Grant(ObjectPermissionSet permissions, string granteeName) => this._lg.Grant(permissions, granteeName);
        public void Grant(ObjectPermissionSet permissions, string[] granteeNames) => this._lg.Grant(permissions, granteeNames);
        public void Grant(ObjectPermissionSet permissions, string[] granteeNames, bool grantGrant) => this._lg.Grant(permissions, granteeNames, grantGrant);
        public void Grant(ObjectPermissionSet permissions, string granteeName, bool grantGrant) => this._lg.Grant(permissions, granteeName, grantGrant);
        public void Grant(ObjectPermissionSet permissions, string[] granteeNames, bool grantGrant, string asRole) => this._lg.Grant(permissions, granteeNames, grantGrant, asRole);
        public void Grant(ObjectPermissionSet permissions, string granteeName, bool grantGrant, string asRole) => this._lg.Grant(permissions, granteeName, grantGrant, asRole);
        public void InitChildCollection(Urn childType, bool forScripting) => _lg.InitChildCollection(childType, forScripting);
        public bool Initialize(bool allProps = false) => _lg.Initialize(allProps);
        public bool IsExpressSku() => _lg.IsExpressSku();
        public bool IsMember(string role) => _lg.IsMember(role);
        public bool IsSupportedProperty(string propName) => _lg.IsSupportedProperty(propName);
        public StringCollection ListMembers() => _lg.ListMembers();
        public void Refresh() => _lg.Refresh();
        public void Rename(string newName) => _lg.Rename(newName);
        public void Revoke(ObjectPermissionSet permissions, string granteeName) => this._lg.Revoke(permissions, granteeName);
        public void Revoke(ObjectPermissionSet permissions, string[] granteeNames) => this._lg.Revoke(permissions, granteeNames);
        public void Revoke(ObjectPermissionSet permissions, string[] granteeNames, bool revokeGrant, bool cascade) => this._lg.Revoke(permissions, granteeNames, revokeGrant, cascade);
        public void Revoke(ObjectPermissionSet permissions, string granteeName, bool revokeGrant, bool cascade, string asRole) => this._lg.Revoke(permissions, granteeName, revokeGrant, cascade, asRole);
        StringCollection IScriptable.Script() => _lg.Script();
        StringCollection IScriptable.Script(ScriptingOptions scriptingOptions) => _lg.Script(scriptingOptions);
        public void SetAccessToken(IRenewableToken token) => _lg.SetAccessToken(token);
        public void SetState(SqlSmoState state) => _lg.SetState(state);
        public override string ToString() => _lg.ToString();
        public void Touch() => _lg.Touch();
        ValidationState ISfcValidate.Validate(string methodName, params object[] arguments) => _lg.Validate(methodName, arguments);

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
