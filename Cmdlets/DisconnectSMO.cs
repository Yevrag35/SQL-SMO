using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Management.Automation;

namespace SQL.SMO.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Disconnects from the current SMO context.</para>
    /// <para type="description">Disconnects and disposes of the SQL Management Object in the PowerShell session's context, after which it runs the garbage collector.</para>
    /// </summary>
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
