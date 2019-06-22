using MG.Dynamic;
using MG.Sql.Smo.Exceptions;
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
        protected private int _count;
        protected private List<Table> _tables;
        private MgSmoCollection<Database> _dbs;
        //private List<string> _names;
        //private string[] chosenNames = null;
        //private bool OneDB = false;
        protected override string Activity => "SQL Table Retrieval";
        protected override string StatusFormat => "Processing Database {0}/{1}...";
        protected private const string TABNAME = "Table" + BaseSqlCmdlet.NAME;
        protected override ICollection<string> Items => _tables.Select(x => x.Name).ToArray();
        protected override int TotalCount => _count;

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

            if (this.Database != null)
                _server = this.Database.Parent;

            else if (SmoContext.IsSet && SmoContext.IsConnected)
                _server = SmoContext.Connection;

            else
                throw new SmoContextNotSetException();

            _dbs = new MgSmoCollection<Database>();
        }

        protected override void ProcessRecord()
        {
            if (this.Database != null)
                _dbs.Add(this.Database);

            else if (SmoContext.Databases != null && SmoContext.Databases.Count > 0)
                _dbs.AddRange(SmoContext.Databases);

            else
            {
                SmoContext.SetDatabases(_server.Databases);
                _dbs.AddRange(SmoContext.Databases);
            }
        }

        protected override void EndProcessing()
        {
            _count = _dbs.Count;
            var tbs = new List<Table>(_count);
            if (this.TableName != null && this.TableName.Length > 0)
            {
                WildcardPattern[] pats = this.GetPatterns(this.TableName);
                    
                for (int d = 0; d < _dbs.Count; d++)
                {
                    this.UpdateProgress(0, d);
                    Database db = _dbs[d];
                    for (int t = 0; t < db.Tables.Count; t++)
                    {
                        this.UpdateSubProgress(t, db.Tables.Count);
                        Table tab = db.Tables[t];

                        if (this.MatchesPatterns(tab.Name, pats))
                            base.WriteObject(tab);
                    }
                    this.UpdateProgress(1);
                }
                this.UpdateProgress(0);
            }
            else
            {
                for (int d = 0; d < _dbs.Count; d++)
                {
                    this.UpdateProgress(0, d);

                    Database db = _dbs[d];
                    for (int t = 0; t < db.Tables.Count; t++)
                    {
                        this.UpdateSubProgress(t, db.Tables.Count);
                        base.WriteObject(db.Tables[t]);
                    }
                    this.UpdateProgress(1);
                }
                this.UpdateProgress(0);
            }
        }

        #endregion

        #region BACKEND METHODS

        private WildcardPattern[] GetPatterns(string[] names)
        {
            var pats = new WildcardPattern[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                pats[i] = new WildcardPattern(names[i], WildcardOptions.IgnoreCase);
            }
            return pats;
        }
        private bool MatchesPatterns(string str, WildcardPattern[] wcps)
        {
            bool result = false;

            for (int i = 0; i < wcps.Length; i++)
            {
                if (wcps[i].IsMatch(str))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        protected override void UpdateProgress(int id, int on)
        {
            string status = string.Format(this.StatusFormat, on, _count);
            var pr = new ProgressRecord(id, this.Activity, status);
            this.WriteTheProgress(pr, on, _count);
        }
        protected private void UpdateSubProgress(int on, int total)
        {
            string status = string.Format("Retrieving Table {0}/{1}...", on, total);
            var pr = new ProgressRecord(1, "Gathering Sql Tables", status)
            {
                ParentActivityId = 0
            };
            this.WriteTheProgress(pr, on, total);
        }

        protected private void WriteTheProgress(ProgressRecord pr, int on, int count)
        {
            double num = Math.Round(on / (double)count * HUNDRED, this.RoundDigits, this.MidpointRounding);
            pr.PercentComplete = Convert.ToInt32(num);
            base.WriteProgress(pr);
        }

        #endregion
    }
}
