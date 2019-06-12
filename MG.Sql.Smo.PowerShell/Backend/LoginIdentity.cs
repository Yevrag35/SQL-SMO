using Microsoft.ActiveDirectory.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MG.Sql.Smo.PowerShell
{
    public class LoginIdentity
    {
        private dynamic _backing;
        private Type _backingType => _backing != null
            ? _backing.GetType()
            : null;

        public bool IsLoginName { get; }
        public bool IsID { get; }
        public bool IsSecurityIdentitifer { get; }

        private LoginIdentity(ADUser adUser)
        {
            _backing = adUser.SID.Value;
            this.IsSecurityIdentitifer = true;
        }
        private LoginIdentity(ADGroup adGroup)
        {
            _backing = adGroup.SID.Value;
            this.IsSecurityIdentitifer = true;
        }
        private LoginIdentity(ADComputer adComp)
        {
            _backing = adComp.SID.Value;
            this.IsSecurityIdentitifer = true;
        }
        private LoginIdentity(string strId)
        {
            _backing = strId;
            if (strId.StartsWith("S-"))
                this.IsSecurityIdentitifer = true;

            else
                this.IsLoginName = true;
        }
        private LoginIdentity(int loginId)
        {
            _backing = loginId;
            this.IsID = true;
        }
        private LoginIdentity(short loginId)
            : this(Convert.ToInt32(loginId)) { }
        private LoginIdentity(byte loginId)
            : this(Convert.ToInt32(loginId)) { }

        private T GetValue<T>() => (T)_backing;

        public static implicit operator LoginIdentity(string s) => new LoginIdentity(s);
        public static implicit operator LoginIdentity(int i) => new LoginIdentity(i);
        public static implicit operator LoginIdentity(short s) => new LoginIdentity(s);
        public static implicit operator LoginIdentity(byte b) => new LoginIdentity(b);
        public static implicit operator LoginIdentity(ADComputer adComp) => new LoginIdentity(adComp);
        public static implicit operator LoginIdentity(ADGroup adGroup) => new LoginIdentity(adGroup);
        public static implicit operator LoginIdentity(ADUser adUser) => new LoginIdentity(adUser);
        public static explicit operator int(LoginIdentity logId)
        {
            if (logId.IsID)
                return logId.GetValue<int>();

            else
                throw new InvalidCastException("Cannot convert a login name to a integer.");
        }
        public static explicit operator string(LoginIdentity logId) => Convert.ToString(logId._backing);
    }
}
