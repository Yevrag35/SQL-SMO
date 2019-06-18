using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace MG.Sql.Smo
{
    public sealed class Password : IDisposable
    {
        private bool _isDisp = false;
        private SecureString _ss;

        private Password(SecureString securePass)
        {
            using (securePass)
            {
                _ss = securePass.Copy();
            }
        }
        private Password(string plainPass)
        {
            _ss = new SecureString();
            for (int i = 0; i < plainPass.Length; i++)
            {
                _ss.AppendChar(plainPass[i]);
            }
        }

        public void Dispose()
        {
            if (!_isDisp)
            {
                _ss.Dispose();
                GC.SuppressFinalize(this);
                _isDisp = true;
            }
        }
        public bool IsReadOnly() => _ss.IsReadOnly();
        public int Length => _ss.Length;
        public void MakeReadOnly() => _ss.MakeReadOnly();

        public static explicit operator Password(string plainStr) => new Password(plainStr);
        public static implicit operator Password(SecureString securePass) => new Password(securePass);
        public static implicit operator SecureString(Password pass)
        {
            if (!pass._isDisp)
            {
                var ss = pass._ss.Copy();
                pass.Dispose();
                return ss;
            }

            else
                throw new ObjectDisposedException("pass");
        }
    }
}
