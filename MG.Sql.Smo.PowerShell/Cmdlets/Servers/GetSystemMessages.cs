using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "SystemMessages", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SystemMessage))]
    public class GetServerSystemMessages : HighMemoryCmdlet
    {
        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            Server srv = SmoContext.Connection;
            if (this.Force || ShouldProcess(srv.Name, "Get System Messages"))
                base.WriteObject(srv.SystemMessages, true);
        }

        #endregion
    }
}