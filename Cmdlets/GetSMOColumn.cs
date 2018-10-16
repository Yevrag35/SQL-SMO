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
    [Cmdlet(VerbsCommon.Get, "SMOColumn", DefaultParameterSetName = "SpecificColumns")]
    [OutputType(typeof(SMOColumn))]
    public class GetSMOColumn : ProgressCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public SMOTable Table { get; set; }

        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "SpecificColumns")]
        public string[] Name { get; set; }

        private bool _no;
        [Parameter(Mandatory = true, ParameterSetName = "NamesOnly")]
        public SwitchParameter NamesOnly
        {
            get => _no;
            set => _no = value;
        }

        private protected bool skip = true;

        internal override string Activity => "Gathering Column Information";

        internal override string StatusFormat => "Retrieving column {0}/{1}...";

        internal protected List<SMOColumn> list = new List<SMOColumn>();

        internal override int Count => list.Count;

        internal protected const int ProgressId = 2;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            SharedCmdlet.CheckSession();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (Name != null)
                WriteObject(Table.GetColumns(Name));

            else if (_no)
                WriteObject(Table.Columns);

            else
            {
                list.AddRange(Table.GetColumns());
                skip = false;
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            if (!skip)
            {
                for (int i = 1; i <= Count; i++)
                {
                    UpdateProgress(ProgressId, i);
                    var col = list[i - 1];
                    WriteObject(col);
                }
                UpdateProgress(ProgressId);
            }
        }
    }
}
