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
    [Cmdlet(VerbsCommon.Get, "SmoTable", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "ByPipelineInput")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(Table))]
    public class GetSmoTable : BaseSqlProgressCmdlet, IDynamicParameters
    {
        private static readonly string[] Aliases = new string[2] { "tab", "tbname" };

        private DynamicLibrary _dynLib;
        protected private List<Table> _tables;
        private List<string> _names;
        private string[] chosenNames = null;
        private bool OneDB = false;
        protected override string Activity => "SQL Table Retrieval";
        protected private const string TABNAME = "Table" + NAME;
        protected override ICollection<string> Items => _tables.Select(x => x.Name).ToArray();
        protected override int TotalCount => _tables.Count;

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByPipelineInput")]
        public Database Database { get; set; }

        #endregion

        #region DYNAMIC PARAMETERS
        public object GetDynamicParameters()
        {
            _dynLib = new DynamicLibrary();
            if (Database != null)
            {
                _dynLib = new DynamicLibrary();
                var dp = new DynamicParameter<Table>(TABNAME, Database.Tables.Cast<Table>(), x => x.Name, NAME, true)
                {
                    Mandatory = false,
                    Position = 0
                };
                dp.Aliases.AddRange(Aliases);
                _dynLib.Add(dp);
            }
            else
            {
                _dynLib = new DynamicLibrary
                {
                    { TABNAME, new RuntimeDefinedParameter(TABNAME, typeof(string[]), new Collection<Attribute>
                        {
                            new AliasAttribute(Aliases),
                            new ParameterAttribute
                            {
                                Mandatory = false,
                                Position = 0,
                            }
                        })
                    }
                };
            }
            return _dynLib;
        }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _tables = new List<Table>();
            if (this.MyInvocation.BoundParameters.ContainsKey(TABNAME))
            {
                if (_dynLib.ParameterIsValidateSet(TABNAME))
                {
                    OneDB = true;
                    _tables.AddRange(_dynLib.GetUnderlyingValues<Table>(TABNAME));
                }
                else
                    chosenNames = _dynLib.GetParameterValues<string>(TABNAME);

            }
        }

        protected override void ProcessRecord()
        {
            if (!OneDB)
                _tables.AddRange(this.GetTablesFromName(chosenNames));
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



        #endregion
    }
}
