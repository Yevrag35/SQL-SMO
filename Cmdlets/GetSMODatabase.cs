using SQL.SMO.Databases;
using SQL.SMO.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace SQL.SMO.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "SMODatabase", DefaultParameterSetName = "DynamicParameter")]
    [OutputType(typeof(SMODatabase))]
    public class GetSMODatabase : SharedCmdlet
    {
        private Dynamic _dyn;
        private protected string[] _pns;

        internal override RuntimeDefinedParameterDictionary GenerateFor()
        {
            if (_dyn == null)
                _dyn = new Dynamic();

            if (Context.DBNames == null)
                Context.GetDatabaseNames();

            if (_pns == null)
                _pns = SMOPropertyLoader.GetPropertyNames(typeof(SMODatabase));

            var param1 = _dyn.Generate(dName, Context.DBNames, false);
            var param2 = _dyn.Generate("Properties", _pns, false, 1);
            _source = Dynamic.ToDictionary(param1, param2);

            return _source;
        }

        private protected string[] GetChosenDatabases()
        {
            var chosen = _source[dName].Value;
            if (chosen == null)
                return Context.DBNames;

            else
            {
                var chosenNames = ((IEnumerable)chosen).Cast<string>().ToArray();
                var list = new List<string>();
                for (int i = 0; i < Context.DBNames.Length; i++)
                {
                    var name = Context.DBNames[i];
                    for (int n = 0; n < chosenNames.Length; n++)
                    {
                        var cname = chosenNames[n];
                        if (string.Equals(cname, name, StringComparison.InvariantCultureIgnoreCase))
                            list.Add(name);
                    }
                }
                return list.ToArray();
            }
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (Context.DBNames == null)
                Context.GetDatabaseNames();

            if (_pns == null)
                _pns = SMOPropertyLoader.GetPropertyNames(typeof(SMODatabase));

            CheckSession();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            var dbCol = (SMODatabaseCollection)Context.Connection.Databases;
            dbCol.LoadProperties("Name");
            string[] dbNames = GetChosenDatabases();
            for (int i = dbCol.Count - 1; i >= 0; i--)
            {
                var db = dbCol[i];
                if (!dbNames.Contains(db.Name))
                    dbCol.Remove(db);
            }
            if (_source["Properties"].Value is string[] props)
            {
                dbCol.LoadProperties(props);
            }
            WriteObject(dbCol, true);
        }
    }
}
