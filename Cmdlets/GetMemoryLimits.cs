using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Config;
using SQL.SMO.Framework;
using System;
using System.Management.Automation;

namespace SQL.SMO.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "SQLMemoryLimit")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(MemoryProperty))]
    public class GetSQLMemoryLimit : PSCmdlet
    {
        [Parameter(Mandatory = false, Position = 0)]
        public MeasurementUnit InUnitsOf = MeasurementUnit.MB;

        [Parameter(Mandatory = false, ValueFromPipeline = true, DontShow = true)]
        public Server SMO { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (!Context.IsSet && SMO == null)
            {
                throw new SMOContextNotSetException();
            }
            else if (Context.IsSet && SMO == null && !Context.IsConnected)
            {
                Context.Disconnect(true);
                GC.Collect();
                throw new SMOContextNotSetException();
            }
            else if (SMO == null)
            {
                SMO = (Server)Context.Connection;
            }

            MemoryProperty memProp = new MemoryProperty(
                SMO.Configuration.MinServerMemory,
                SMO.Configuration.MaxServerMemory,
                InUnitsOf
            );
            WriteObject(memProp, false);
        }
    }
}
