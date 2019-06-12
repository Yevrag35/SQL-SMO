using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "ServerErrorLog", ConfirmImpact = ConfirmImpact.None)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(ErrorLog))]
    public class GetServerErrorLog : BaseSqlCmdlet
    {
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord() => WriteObject(ErrorLog.GetErrorLogs(SmoContext.Connection), true);
    }
}
