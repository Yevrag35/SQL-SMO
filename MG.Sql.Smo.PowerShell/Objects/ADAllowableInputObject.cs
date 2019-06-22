using Microsoft.ActiveDirectory.Management;
using Microsoft.PowerShell.Commands;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace MG.Sql.Smo.PowerShell
{
    public class ADAllowableInputObject
    {
        #region FIELDS/CONSTANTS
        private string DOMAIN_FORMAT = "{0}\\{1}";
        private static readonly string THIS_COMP = Environment.GetEnvironmentVariable("COMPUTERNAME");

        #endregion

        #region PROPERTIES
        public bool FromADObject { get; internal set; }
        public LoginType? Type { get; internal set; }
        public string DistinguishedName { get; internal set; }
        public bool IsLocal { get; internal set; }
        public string LoginName { get; internal set; }
        public string NetBiosName { get; set; }

        #endregion

        #region CONSTRUCTORS
        private ADAllowableInputObject(ADComputer adComp)
        {
            this.LoginName = adComp.SamAccountName;
            //this.IsADAccount = true;
            this.FromADObject = true;
            this.Type = LoginType.WindowsUser;
            this.DistinguishedName = adComp.DistinguishedName;
        }
        private ADAllowableInputObject(ADUser adUser)
        {
            this.LoginName = adUser.SamAccountName;
            this.Type = LoginType.WindowsUser;
            this.FromADObject = true;
            this.DistinguishedName = adUser.DistinguishedName;
        }
        private ADAllowableInputObject(ADGroup adGroup)
        {
            this.LoginName = adGroup.SamAccountName;
            this.Type = LoginType.WindowsGroup;
            this.FromADObject = true;
            this.DistinguishedName = adGroup.DistinguishedName;
        }
        private ADAllowableInputObject(LocalUser user)
        {
            this.LoginName = user.Name;
            this.IsLocal = true;
            this.Type = LoginType.WindowsUser;
        }
        private ADAllowableInputObject(LocalGroup group)
        {
            this.LoginName = group.Name;
            this.IsLocal = true;
            this.Type = LoginType.WindowsGroup;
        }
        private ADAllowableInputObject(string login) => this.LoginName = login;

        #endregion

        #region PUBLIC METHODS
        public static implicit operator ADAllowableInputObject(ADComputer comp) => new ADAllowableInputObject(comp);
        public static implicit operator ADAllowableInputObject(ADUser user) => new ADAllowableInputObject(user);
        public static implicit operator ADAllowableInputObject(ADGroup group) => new ADAllowableInputObject(group);
        public static implicit operator ADAllowableInputObject(LocalUser locUsr) => new ADAllowableInputObject(locUsr);
        public static implicit operator ADAllowableInputObject(LocalGroup locGrp) => new ADAllowableInputObject(locGrp);
        public static implicit operator ADAllowableInputObject(string loginName) => new ADAllowableInputObject(loginName);

        public string AsLoginName(Server server)
        {
            string asName = !string.IsNullOrEmpty(this.NetBiosName) 
                ? string.Format(DOMAIN_FORMAT, this.NetBiosName, this.LoginName)
                : this.LoginName;

            if (this.IsLocal && !asName.Contains(@"\"))
                asName = string.Format("{0}\\{1}", server.NetName, asName);

            return asName;
        }

        #endregion

        #region BACKEND/PRIVATE METHODS


        #endregion
    }
}