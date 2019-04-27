using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommunications.Disconnect, "SmoServer", ConfirmImpact = ConfirmImpact.None)]
    public class DisconnectSmoServer : PSCmdlet
    {
        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord() => SMOContext.Disconnect();

        #endregion

        #region CMDLET METHODS


        #endregion
    }
}