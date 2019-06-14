using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell.Cmdlets.Logins
{
    [Cmdlet(VerbsCommon.New, "WindowsLogin", ConfirmImpact = ConfirmImpact.Low, SupportsShouldProcess = true)]
    [OutputType(typeof(SmoLogin))]
    public class NewWindowsLogin : BaseLoginCmdlet
    {
        private string netBiosName;

        #region PARAMETERS
        //[Parameter(Mandatory = )]

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
        }

        #endregion

        #region CMDLET METHODS


        #endregion
    }
}