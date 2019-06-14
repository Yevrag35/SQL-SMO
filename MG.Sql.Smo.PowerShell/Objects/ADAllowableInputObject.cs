using Microsoft.ActiveDirectory.Management;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace MG.Sql.Smo.PowerShell.Backend
{
    public class ADAllowableInputObject
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PROPERTIES
        public bool FromADObject { get; internal set; }
        public bool IsADAccount { get; internal set; }
        public bool IsADGroup { get; internal set; }
        public bool IsNTLogin { get; internal set; }
        public bool IsSqlLogin { get; internal set; }
        public string LoginName { get; }
        public string NetBiosName { get; set; }

        #endregion

        #region CONSTRUCTORS
        private ADAllowableInputObject(ADComputer adComp)
        {
            this.LoginName = adComp.SamAccountName;
            this.IsADAccount = true;
            this.FromADObject = true;
        }
        private ADAllowableInputObject(ADUser adUser)
        {
            this.LoginName = adUser.SamAccountName;
            this.IsADAccount = true;
            this.FromADObject = true;
        }
        private ADAllowableInputObject(ADGroup adGroup)
        {
            this.LoginName = adGroup.SamAccountName;
            this.IsADGroup = true;
            this.FromADObject = true;
        }
        private ADAllowableInputObject(string login) => this.LoginName = login;

        #endregion

        #region PUBLIC METHODS


        #endregion

        #region BACKEND/PRIVATE METHODS


        #endregion
    }
}