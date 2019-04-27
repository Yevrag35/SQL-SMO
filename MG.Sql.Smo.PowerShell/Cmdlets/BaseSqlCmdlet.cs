using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Cmdlets
{
    public abstract class BaseSqlCmdlet : PSCmdlet
    {
        protected override void BeginProcessing()
        {
            if (!SMOContext.IsSet || !SMOContext.IsConnected)
                throw new SMOContextNotSetException();
        }
    }
}