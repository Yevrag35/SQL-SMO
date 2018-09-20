using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Management.Automation;

namespace SQL.SMO.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "SMOConnection")]
    [OutputType(typeof(SMOConnection))]
    public class GetSMOConnection : PSCmdlet
    {
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            SharedCmdlet.CheckSession();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            SMOConnection conn = new SMOConnection((Server)Context.Connection);
            WriteObject(conn, false);
        }
    }
}
