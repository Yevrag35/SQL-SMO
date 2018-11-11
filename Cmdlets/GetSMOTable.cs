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
        private Dynamic _dyn;
        private protected string[] _pns;

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public SMODatabase Database { get; set; }

        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "SpecificTables")]
        public string[] Name { get; set; }

        internal override string Activity => "Gathering Table Information";

        internal override string StatusFormat => "Retrieving table {0}/{1}...";

        internal override int Count => list.Count;

        internal protected List<Table> list = new List<Table>();

        internal override RuntimeDefinedParameterDictionary GenerateFor()
        {
            if (_dyn == null)
                _dyn = new Dynamic();

            if (_pns == null)
                _pns = SMOPropertyLoader.GetPropertyNames(typeof(SMOTable));

            _source = Dynamic.ToDictionary(_dyn.Generate("Properties", _pns, false));
            return _source;
        }

        internal protected const int ProgressId = 1;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (_pns == null)
                _pns = SMOPropertyLoader.GetPropertyNames(typeof(SMOTable));

            CheckSession();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            var realDB = this.Database.ShowOriginal() as Database;
            for (int i = 0; i < realDB.Tables.Count; i++)
            {
                var tbl = realDB.Tables[i];
                list.Add(tbl);
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
                    var tbl = list[i - 1];
                    var smot = (SMOTable)tbl;
                    smot.Load((string[])_source["Properties"].Value);
                    WriteObject(smot);
                }
                UpdateProgress(ProgressId);
            }
            else
            {
                foreach (var tbl in list.Where(x => Name.Contains(x.Name)))
                {
                    var smot = (SMOTable)tbl;
                    smot.Load((string[])_source["Properties"].Value);
                    WriteObject(smot);
                }
            }
        }
    }
}
