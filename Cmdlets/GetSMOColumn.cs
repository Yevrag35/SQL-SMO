using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Databases;
using SQL.SMO.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace SQL.SMO.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "SMOColumn")]
    [OutputType(typeof(SMOColumn))]
    public class GetSMOColumn : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public SMOTable Table { get; set; }

        [Parameter(Mandatory = false, Position = 0)]
        public string[] Name { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            SharedCmdlet.CheckSession();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            SMOColumn[] smoc;
            if (Name == null)
            {
                smoc = Table.GetColumns();
            }
            else
            {
                smoc = Table.GetColumns(Name);
            }
            WriteObject(smoc, false);
        }
    }
}
