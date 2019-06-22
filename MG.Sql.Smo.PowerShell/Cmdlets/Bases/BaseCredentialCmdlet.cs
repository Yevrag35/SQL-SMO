using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace MG.Sql.Smo.PowerShell
{
    public abstract class BaseCredentialCmdlet : BaseServerSqlCmdlet
    {
        #region PARAMETERS


        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        #endregion

        #region METHODS
        protected private List<Credential> GetCredentials() => _server.Credentials.Cast<Credential>().ToList();

        #endregion
    }
}