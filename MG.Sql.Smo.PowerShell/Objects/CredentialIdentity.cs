using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell
{
    public class CredentialIdentity
    {
        #region FIELDS/CONSTANTS
        private dynamic _backing;

        #endregion

        #region PROPERTIES
        public bool IsNumericId { get; }
        public bool IsStringId { get; }

        #endregion

        #region CONSTRUCTORS
        private CredentialIdentity(string idStr)
        {
            this.IsStringId = true;
            _backing = idStr;
        }
        private CredentialIdentity(int idNum)
        {
            this.IsNumericId = true;
            _backing = idNum;
        }

        #endregion

        #region METHODS
        public static implicit operator CredentialIdentity(string s) => new CredentialIdentity(s);
        public static implicit operator CredentialIdentity(int i) => new CredentialIdentity(i);
        public static implicit operator CredentialIdentity(short i) => new CredentialIdentity(Convert.ToInt32(i));
        public static implicit operator CredentialIdentity(byte i) => new CredentialIdentity(Convert.ToInt32(i));
        public static implicit operator CredentialIdentity(long i) => new CredentialIdentity(Convert.ToInt32(i));

        public static explicit operator int(CredentialIdentity cid) => (int)cid._backing;
        public static explicit operator string(CredentialIdentity cid) => (string)cid._backing;

        #endregion
    }
}