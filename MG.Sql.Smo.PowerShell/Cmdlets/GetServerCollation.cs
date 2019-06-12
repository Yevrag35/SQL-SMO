using MG.Dynamic;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "ServerCollation", ConfirmImpact = ConfirmImpact.None)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(Collation))]
    public class GetServerCollation : BaseSqlCmdlet, IDynamicParameters
    {
        private DynamicLibrary _dynLib;

        #region PARAMETERS
        [Parameter(Mandatory = false, Position = 0)]
        [SupportsWildcards]
        public string Name { get; set; }

        [Parameter(Mandatory = false)]
        public int[] LocaleID { get; set; }

        [Parameter(Mandatory = false)]
        public CollationVersion[] Version { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateSet("Name", "LocaleID", "Version")]
        public string SortBy { get; set; }

        #endregion

        #region CMDLET PROCESSING
        public object GetDynamicParameters()
        {
            if (!string.IsNullOrEmpty(this.SortBy) && this.SortBy == "Name")
            {
                _dynLib = new DynamicLibrary();
                _dynLib.Add("Ascending", new RuntimeDefinedParameter("Ascending", typeof(SwitchParameter), new Collection<Attribute>
                {
                    new ParameterAttribute
                    {
                        Mandatory = false
                    }
                }));
                return _dynLib;
            }
            else
                return null;
        }

        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            var cols = CollationCollection.GetCollations(SmoContext.Connection);
            this.FilterByName(ref cols);
            this.FilterByLocaleId(ref cols);
            this.FilterByVersion(ref cols);
            
            if (this.MyInvocation.BoundParameters.ContainsKey("SortBy"))
                this.Sort(ref cols);

            WriteObject(cols, true);
        }

        #endregion

        #region CMDLET METHODS
        private void FilterByName(ref CollationCollection cols)
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("Name"))
            {
                var wcp = new WildcardPattern(this.Name, WildcardOptions.IgnoreCase);
                for (int i = cols.Count - 1; i >= 0; i--)
                {
                    Collation c = cols[i];
                    if (!wcp.IsMatch(c.Name))
                        cols.Remove(c);
                }
            }
        }
        private void FilterByLocaleId(ref CollationCollection cols)
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("LocaleID"))
            {
                for (int i = cols.Count - 1; i >= 0; i--)
                {
                    Collation c = cols[i];
                    if (!this.LocaleID.Contains(c.LocaleID))
                        cols.Remove(c);
                }
            }
        }
        private void FilterByVersion(ref CollationCollection cols)
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("Version"))
            {
                for (int i = cols.Count - 1; i >= 0; i--)
                {
                    Collation c = cols[i];
                    if (!this.Version.Contains(c.CollationVersion))
                        cols.Remove(c);
                }
            }
        }

        private void Sort(ref CollationCollection cols)
        {
            switch (this.SortBy)
            {
                case "Name":
                {
                    cols.SortByName(_dynLib.ParameterHasValue("Ascending"));
                    break;
                }
                case "LocaleID":
                {
                    cols.SortByLocaleID();
                    break;
                }
                case "Version":
                {
                    cols.SortByVersion();
                    break;
                }
            }
        }

        #endregion
    }
}
