using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "UserDefinedMessages", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(UserDefinedMessage))]
    public class GetUserDefinedMessages : HighMemoryCmdlet
    {
        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            Server srv = SmoContext.Connection;
            if (this.Force || ShouldProcess(srv.Name, "Get System Messages"))
                base.WriteObject(srv.UserDefinedMessages, true);
        }

        #endregion
    }
}