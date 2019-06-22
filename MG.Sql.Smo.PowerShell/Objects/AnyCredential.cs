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
    public class AnyCredential
    {
        #region FIELDS/CONSTANTS
        private readonly PSCredential _psCreds;

        #endregion

        #region PROPERTIES
        public string UserName => _psCreds.UserName;
        public SecureString Password => _psCreds.Password;

        #endregion

        #region CONSTRUCTORS
        public AnyCredential(string userName, SecureString pass)
        {
            _psCreds = new PSCredential(userName, pass);
            _psCreds.Password.MakeReadOnly();
        }

        private AnyCredential(NetworkCredential netCreds)
        {
            _psCreds = new PSCredential(netCreds.UserName, netCreds.SecurePassword.Copy());
            _psCreds.Password.MakeReadOnly();
        }

        private AnyCredential(PSCredential psCreds)
        {
            _psCreds = psCreds;
            _psCreds.Password.MakeReadOnly();
        }

        private AnyCredential(SqlCredential sqlCreds)
        {
            _psCreds = new PSCredential(sqlCreds.UserId, sqlCreds.Password.Copy());
            _psCreds.Password.MakeReadOnly();
        }

        #endregion

        #region METHODS
        public static implicit operator AnyCredential(NetworkCredential netCreds) => new AnyCredential(netCreds);
        public static implicit operator AnyCredential(PSCredential psCreds) => new AnyCredential(psCreds);
        public static implicit operator AnyCredential(SqlCredential sqlCreds) => new AnyCredential(sqlCreds);

        public static explicit operator PSCredential(AnyCredential aync) => aync._psCreds;
        public static explicit operator NetworkCredential(AnyCredential aync) => aync._psCreds.GetNetworkCredential();

        #endregion
    }
}