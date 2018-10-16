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
    [Cmdlet(VerbsCommon.Get, "SMOTable", DefaultParameterSetName = "SpecificTables")]
    [OutputType(typeof(SMOTable))]
    [CmdletBinding(PositionalBinding = false)]
    public class GetSMOTable : ProgressCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public SMODatabase Database { get; set; }

        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "SpecificTables")]
        public string[] Name { get; set; }

        private bool _no;
        [Parameter(Mandatory = true, ParameterSetName = "AllTables")]
        public SwitchParameter NamesOnly
        {
            get => _no;
            set => _no = value;
        }

        internal override string Activity => "Gathering Table Information";

        internal override string StatusFormat => "Retrieving table {0}/{1}...";

        internal protected List<SMOTable> list = new List<SMOTable>();

        internal override int Count => list.Count;

        internal protected const int ProgressId = 1;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            SharedCmdlet.CheckSession();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (_no)
            {
                string[] tbls = Database.Tables.OrderBy(x => x).ToArray();
                WriteObject(tbls, false);
            }
            else if (Name != null)
            {
                list.AddRange(Database.GetTables(Name));
            }
            else
            {
                list.AddRange(Database.GetTables());
            }
        }
        protected override void EndProcessing()
        {
            base.EndProcessing();
            if (!_no)
            {
                for (int i = 1; i <= Count; i++)
                {
                    UpdateProgress(ProgressId, i);
                    var tbl = list[i - 1];
                    WriteObject(tbl);
                }
                UpdateProgress(ProgressId);
            }
        }
    }
}
