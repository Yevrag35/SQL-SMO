using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Management.Automation;

namespace SQL.SMO.Cmdlets
{
    [Cmdlet(VerbsCommunications.Disconnect, "SMO")]
    public class DisconnectSMO : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (Context.IsSet)
            {
                WriteVerbose("Disconnecting from " + ((Server)Context.Connection).Name + "...");
                Context.Disconnect(Context.IsConnected);
                WriteVerbose("Calling GC to free up memory...");
                GC.Collect();
            }
        }
    }
}
