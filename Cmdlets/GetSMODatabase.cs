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
    [Cmdlet(VerbsCommon.Get, "SMODatabase", DefaultParameterSetName = "SpecificDBs")]
    [OutputType(typeof(SMODatabase))]
    public class GetSMODatabase : SharedCmdlet
    {
        private Dynamic _dyn;

        private protected bool _no;
        [Parameter(Mandatory = true, ParameterSetName = "NamesOnly")]
        public SwitchParameter NamesOnly
        {
            get => _no;
            set => _no = value;
        }

        internal override RuntimeDefinedParameterDictionary GenerateFor()
        {
            if (_dyn == null) { _dyn = new Dynamic(); }
            if (Context.DBNames == null)
            {
                Context.GetDatabaseNames();
            }
            _source = _dyn.Generate(dName, Context.DBNames, false, "SpecificDBs");
            return _source;
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            CheckSession();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (!_no)
            {
                DatabaseCollection dbCol = ((Server)Context.Connection).Databases;
                if (!(_source[dName].Value is string[] chosen))
                {
                    chosen = Context.DBNames;
                }
                for (int i = 0; i < chosen.Length; i++)
                {
                    string d = chosen[i];
                    Database db = dbCol.OfType<Database>().Single(x => x.Name == d);
                    WriteObject(new SMODatabase(db));
                }
            }
            else
            {
                if (Context.DBNames == null)
                    Context.GetDatabaseNames();

                WriteObject(Context.DBNames, true);
            }
        }
    }
}
