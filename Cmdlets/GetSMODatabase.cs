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
    [Cmdlet(VerbsCommon.Get, "SMODatabase")]
    public class GetSMODatabase : SharedCmdlet
    {
        private Dynamic _dyn;

        internal override RuntimeDefinedParameterDictionary GenerateFor()
        {
            if (_dyn == null) { _dyn = new Dynamic(); }
            if (Context.DBNames == null)
            {
                Context.GetDatabaseNames();
            }
            _source = _dyn.Generate(dName, Context.DBNames, false);
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
            DatabaseCollection dbCol = ((Server)Context.Connection).Databases;
            string[] chosen = _source[dName].Value as string[];
            if (chosen == null)
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
    }
}
