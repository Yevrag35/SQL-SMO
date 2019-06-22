using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;

namespace MG.Sql.Smo
{
    public class SmoUser : SqlUserBase, IAlterable, ICreatable, IDmfFacet, IDropIfExists, IDroppable, IObjectPermission, 
        IRefreshable, IRenamable, IScriptable, ISfcValidate
    {
        #region FIELDS/CONSTANTS
        private SecurityIdentifier _secId;
        private User _u;

        #endregion

        #region PROPERTIES
        public override string AsymmetricKey { get => _u.AsymmetricKey; set => _u.AsymmetricKey = value; }
        public AuthenticationType AuthenticationType => _u.AuthenticationType;
        public override string Certificate { get => _u.Certificate; set => _u.Certificate = value; }
        public override DateTime CreateDate => _u.CreateDate;
        public override DateTime DateLastModified => _u.DateLastModified;
        public DefaultLanguage DefaultLanguage => _u.DefaultLanguage;
        public string DefaultSchema { get => _u.DefaultSchema; set => _u.DefaultSchema = value; }
        public UserEvents Events => _u.Events;
        public ExtendedPropertyCollection ExtendedProperties => _u.ExtendedProperties;
        public bool HasDBAccess => _u.HasDBAccess;
        public override int ID => _u.ID;
        public override bool IsSystemObject => _u.IsSystemObject;
        public string Login => _u.Login;
        public override LoginType LoginType => _u.LoginType;
        public override string Name { get => _u.Name; set => _u.Name = value; }
        public Database Parent { get => _u.Parent; set => _u.Parent = value; }
        public override string Sid => _secId != null
            ? _secId.Value
            : null;

        public UserType UserType { get => _u.UserType; set => _u.UserType = value; }

        #endregion

        #region CONSTRUCTORS
        private SmoUser(User user)
            : base(user)
        {
            if (user.LoginType != LoginType.SqlLogin)
                _secId = new SecurityIdentifier(user.Sid, 0);

            _u = user;
        }
        public SmoUser(Database db, string userName)
            : this(new User(db, userName)) { }

        #endregion

        #region PUBLIC METHODS
        public object GetSid()
        {
            return _secId == null
                ? _u.Sid
                : (object)_secId;
        }

        public static List<SmoUser> GetUsersFromDatabase(Database db)
        {
            var list = new List<SmoUser>(db.Users.Count);
            for (int i = 0; i < db.Users.Count; i++)
            {
                list.Add(db.Users[i]);
            }
            return list;
        }

        #endregion

        #region OPERATORS/CASTS
        public static implicit operator SmoUser(User user) => new SmoUser(user);
        public static explicit operator User(SmoUser smoUser) => smoUser._u;

        #endregion

        #region USER METHODS
        public void AddToRole(string role) => _u.AddToRole(role);
        public void Alter() => _u.Alter();
        public void ChangePassword(Password newPass) => _u.ChangePassword(newPass);
        public void ChangePassword(Password oldPass, Password newPass) => _u.ChangePassword(oldPass, newPass);
        public void Create() => _u.Create();
        public void Create(Password password) => _u.Create(password);
        public void Deny(ObjectPermissionSet permissions, string[] granteeNames) => _u.Deny(permissions, granteeNames);
        public void Deny(ObjectPermissionSet permissions, string granteeName) => _u.Deny(permissions, granteeName);
        public void Deny(ObjectPermissionSet permissions, string[] granteeNames, bool cascade) => _u.Deny(permissions, granteeNames, cascade);
        public void Deny(ObjectPermissionSet permissions, string granteeName, bool cascade) => _u.Deny(permissions, granteeName, cascade);
        public void Drop() => _u.Drop();
        public void DropIfExists() => _u.DropIfExists();
        public ObjectPermissionInfo[] EnumObjectPermissions() => _u.EnumObjectPermissions();
        public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName) => _u.EnumObjectPermissions(granteeName);
        public ObjectPermissionInfo[] EnumObjectPermissions(ObjectPermissionSet permissions) => _u.EnumObjectPermissions(permissions);
        public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName, ObjectPermissionSet permissions) => _u.EnumObjectPermissions(granteeName, permissions);
        public Urn[] EnumOwnedObjects() => _u.EnumOwnedObjects();
        public StringCollection EnumRoles() => _u.EnumRoles();
        public void Grant(ObjectPermissionSet permissions, string[] granteeNames) => _u.Grant(permissions, granteeNames);
        public void Grant(ObjectPermissionSet permissions, string granteeName) => _u.Grant(permissions, granteeName);
        public void Grant(ObjectPermissionSet permissions, string[] granteeNames, bool grantGrant) => _u.Grant(permissions, granteeNames, grantGrant);
        public void Grant(ObjectPermissionSet permissions, string granteeName, bool grantGrant) => _u.Grant(permissions, granteeName, grantGrant);
        public void Grant(ObjectPermissionSet permissions, string[] granteeNames, bool grantGrant, string asRole) => _u.Grant(permissions, granteeNames, grantGrant, asRole);
        public void Grant(ObjectPermissionSet permissions, string granteeName, bool grantGrant, string asRole) => _u.Grant(permissions, granteeName, grantGrant, asRole);
        public bool IsMember(string role) => _u.IsMember(role);
        public void MakeContained(bool copyLoginName, bool disableLogin) => _u.MakeContained(copyLoginName, disableLogin);
        public void Revoke(ObjectPermissionSet permissions, string[] granteeNames) => _u.Revoke(permissions, granteeNames);
        public void Revoke(ObjectPermissionSet permissions, string granteeName) => _u.Revoke(permissions, granteeName);
        public void Revoke(ObjectPermissionSet permissions, string[] granteeNames, bool revokeGrant, bool cascade) => _u.Revoke(permissions, granteeNames, revokeGrant, cascade);
        public void Revoke(ObjectPermissionSet permissions, string granteeName, bool revokeGrant, bool cascade, string asRole) => _u.Revoke(permissions, granteeName, revokeGrant, cascade, asRole);
        public void Rename(string newname) => _u.Rename(newname);
        public StringCollection Script() => _u.Script();
        public StringCollection Script(ScriptingOptions scriptingOptions) => _u.Script(scriptingOptions);

        #endregion
    }
}