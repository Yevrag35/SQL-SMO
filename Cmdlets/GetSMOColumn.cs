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
        private protected Dynamic _dyn;
        private protected string[] _pns;

        internal override RuntimeDefinedParameterDictionary GenerateFor()
        {
            if (_dyn == null)
                _dyn = new Dynamic();

            if (_pns == null)
                _pns = SMOPropertyLoader.GetPropertyNames(typeof(SMOColumn));

            _source = Dynamic.ToDictionary(_dyn.Generate("Properties", _pns, false));
            return _source;
        }

        internal override string Activity => "Gathering Column Information";

        internal override string StatusFormat => "Retrieving column {0}/{1}...";

        internal protected List<Column> list = new List<Column>();

        internal override int Count => list.Count;

        internal protected const int ProgressId = 2;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (_pns == null)
                _pns = SMOPropertyLoader.GetPropertyNames(typeof(SMOColumn));

            CheckSession();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();


            var realTab = this.Table.ShowOriginal() as Table;
            for (int i = 0; i < realTab.Columns.Count; i++)
            {
                var col = realTab.Columns[i];
                list.Add(col);
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            if (Name == null)
            {
                for (int i = 1; i <= Count; i++)
                {
                    UpdateProgress(ProgressId, i);
                    var col = list[i - 1];
                    var smoc = (SMOColumn)col;
                    smoc.Load((string[])_source["Properties"].Value);
                    WriteObject(smoc);
                }
                UpdateProgress(ProgressId);
            }
            else
            {
                foreach (var col in list.Where(x => Name.Contains(x.Name)))
                {
                    var smoc = (SMOColumn)col;
                    smoc.Load((string[])_source["Properties"].Value);
                    WriteObject(smoc);
                }
            }
        }

        //protected override void EndProcessing()
        //{
        //    base.EndProcessing();
        //    if (!skip)
        //    {
        //        for (int i = 1; i <= Count; i++)
        //        {
        //            UpdateProgress(ProgressId, i);
        //            var col = list[i - 1];
        //            WriteObject(col);
        //        }
        //        UpdateProgress(ProgressId);
        //    }
        //}
    }
}
