using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Reflection;
using System.Security;

namespace MG.Sql.Smo.PowerShell
{
    public class PassOrCreds : IDisposable
    {
        #region FIELDS/CONSTANTS
        private bool _isDisp = false;
        private SecureString _pass;

        #endregion

        #region PROPERTIES
        public bool IsCredential { get; }
        public bool IsPasswordOnly { get; }

        #endregion

        #region CONSTRUCTORS
        private PassOrCreds(NetworkCredential netCreds)
        {
            this.IsCredential = true;
            _pass = netCreds.SecurePassword.Copy();
            _pass.MakeReadOnly();
        }
        private PassOrCreds(PSCredential psCreds)
        {
            this.IsCredential = true;
            _pass = psCreds.Password.Copy();
            _pass.MakeReadOnly();
        }
        private PassOrCreds(SecureString ss)
        {
            this.IsPasswordOnly = true;
            _pass = ss.Copy();
            _pass.MakeReadOnly();
        }
        private PassOrCreds(SqlCredential sqlCreds)
        {
            this.IsCredential = true;
            _pass = sqlCreds.Password.Copy();
            _pass.MakeReadOnly();
        }

        #endregion

        #region METHODS
        public void Dispose()
        {
            if (!_isDisp)
            {
                _pass.Dispose();
                GC.SuppressFinalize(_pass);
                GC.SuppressFinalize(this);
                _isDisp = true;
            }
        }

        #endregion

        #region CASTS/OPERATORS
        public static implicit operator PassOrCreds(NetworkCredential netCreds) => new PassOrCreds(netCreds);
        public static implicit operator PassOrCreds(PSCredential psCreds) => new PassOrCreds(psCreds);
        public static implicit operator PassOrCreds(SecureString ss) => new PassOrCreds(ss);
        public static implicit operator PassOrCreds(SqlCredential sqlCreds) => new PassOrCreds(sqlCreds);
        public static explicit operator SecureString(PassOrCreds poc) => poc._pass;

        #endregion
    }
}