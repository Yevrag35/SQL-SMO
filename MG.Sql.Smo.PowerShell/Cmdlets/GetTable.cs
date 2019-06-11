using MG.Dynamic;
using MG.Progress.PowerShell;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Table", ConfirmImpact = ConfirmImpact.None)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(Table))]
    public class GetTable : BaseSqlProgressCmdlet
    {
        private static readonly string[] Aliases = new string[2] { "tab", "tbname" };

        //private DynamicLibrary _dynLib;
        protected private List<Table> _tables;
        //private List<string> _names;
        //private string[] chosenNames = null;
        //private bool OneDB = false;
        protected override string Activity => "SQL Table Retrieval";
        protected private const string TABNAME = "Table" + BaseSqlCmdlet.NAME;
        protected override ICollection<string> Items => _tables.Select(x => x.Name).ToArray();
        protected override int TotalCount => _tables.Count;

        #region PARAMETERS
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public Database Database { get; set; }

        [Parameter(Mandatory = false, Position = 0)]
        [SupportsWildcards]
        public string[] TableName { get; set; }

        #endregion

        #region DYNAMIC PARAMETERS
        //public object GetDynamicParameters()
        //{
        //    if (_dynLib == null)
        //        _dynLib = new DynamicLibrary();

        //    _dynLib.Clear();
        //    if (Database != null)
        //    {
        //        var dp = new DynamicParameter<Table>(TABNAME, Database.Tables.Cast<Table>(), x => x.Name, NAME, true)
        //        {
        //            Mandatory = false,
        //            ParameterSetName = "__AllParameterSets"
        //        };
        //        dp.Aliases.AddRange(Aliases);
        //        _dynLib.Add(dp);
        //    }
        //    else
        //    {
        //        _dynLib.Add(TABNAME, new RuntimeDefinedParameter(TABNAME, typeof(string[]), new Collection<Attribute>
        //                {
        //                    new AliasAttribute(Aliases),
        //                    new ParameterAttribute
        //                    {
        //                        Mandatory = false,
        //                        ParameterSetName = "__AllParameterSets"
        //                    }
        //                }));
        //    }
        //    return _dynLib;
        //}

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _tables = new List<Table>();
        }

        protected override void ProcessRecord()
        {
            IEnumerable<Table> tbs = null;
            if (this.MyInvocation.BoundParameters.ContainsKey("Database"))
                tbs = this.Database.Tables.Cast<Table>();

            else
            {
                var list = new List<Table>();
                foreach (Database db in SmoContext.Connection.Databases)
                {
                    list.AddRange(db.Tables.Cast<Table>());
                }
                tbs = list;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey(TABNAME))
                _tables.AddRange(this.GetTablesFromWildcard(TableName, tbs));

            else
                _tables.AddRange(tbs);
        }

        protected override void EndProcessing()
        {
            for (int i = 0; i < _tables.Count; i++)
            {
                base.UpdateProgress(0, i);
                WriteObject(_tables[i]);
            }
        }

        #endregion

        #region BACKEND METHODS
        protected private virtual IEnumerable<Table> GetTablesFromName(string[] names)
        {
            IEnumerable<Table> tables = this.Database.Tables.Cast<Table>();
            return names != null && names.Length > 0
                ? tables.Where(x => names.Any(s => s.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase)))
                : tables;
        }

        protected private virtual IEnumerable<Table> GetTablesFromWildcard(string[] names, IEnumerable<Table> reference)
        {
            if (names != null && names.Length > 0)
            {
                var tbs = new List<Table>();
                for (int i = 0; i < names.Length; i++)
                {
                    var wcp = new WildcardPattern(names[i], WildcardOptions.IgnoreCase);
                    tbs.AddRange(reference.Where(x => wcp.IsMatch(x.Name)));
                }
                return tbs.Distinct(new TableEquality());
            }
            else
                return null;
        }
        #endregion
    }
}
