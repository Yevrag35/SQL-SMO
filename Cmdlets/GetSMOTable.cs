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
    [Cmdlet(VerbsCommon.Get, "SMOTable", DefaultParameterSetName = "Specific")]
    [OutputType(typeof(SMOTable))]
    [OutputType(typeof(string[]))]
    [CmdletBinding(PositionalBinding = false)]
    public class GetSMOTable : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public SMODatabase Database { get; set; }

        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "Specific")]
        public string[] Name { get; set; }

        private bool _no;
        [Parameter(Mandatory = false, ParameterSetName = "All")]
        public SwitchParameter NamesOnly
        {
            get => _no;
            set => _no = value;
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            SharedCmdlet.CheckSession();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (!_no)
            {
                SMOTable[] smot;
                if (Name == null)
                {
                    smot = Database.GetTables();
                }
                else
                {
                    smot = Database.GetTables(Name);
                }
                WriteObject(smot, true);
            }
            else
            {
                string[] tbls = Database.Tables.OrderBy(x => x).ToArray();
                WriteObject(tbls, false);
            }
        }
    }
}
