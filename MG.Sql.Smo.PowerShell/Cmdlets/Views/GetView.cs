using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell.Cmdlets.Views
{
    [Cmdlet(VerbsCommon.Get, "View", ConfirmImpact = ConfirmImpact.None)]
    public class GetView : BaseSqlCmdlet
    {
        private MgSmoCollection<View> _views;

        #region PARAMETERS
        [Parameter(Mandatory = false, Position = 0)]
        [SupportsWildcards]
        public string[] ViewName { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public Database Database { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _views = new MgSmoCollection<View>();
        }

        protected override void ProcessRecord()
        {
            if (this.Database != null && this.MyInvocation.PipelinePosition >= 2)
            {
                if (this.HasNames())
                {
                    WildcardPattern[] patterns = this.PatternsFromNames(this.ViewName);
                    this.FilterByName(ref _views, patterns, this.Database);
                }
                else
                {
                    _views.AddRange(this.Database.Views);
                }
            }
            else if (this.Database != null && this.MyInvocation.PipelinePosition < 2)
            {
                NoEnd = true;
                if (this.HasNames())
                {
                    WildcardPattern[] patterns = base.PatternsFromNames(this.ViewName);
                    for (int i = 0; i < this.Database.Views.Count; i++)
                    {
                        View v = this.Database.Views[i];
                        if (base.NameMatchesPatterns(v.Name, patterns))
                            base.WriteObject(v);
                    }
                }
            }
            else
            {
                if (SmoContext.Databases == null)
                {
                    SmoContext.SetDatabases(_server.Databases);
                }

                _views.AddRange(SmoContext.Databases.SelectMany(x => x.Views.Cast<View>()));
            }
        }

        protected override void EndProcessing()
        {
            if (!NoEnd)
            {
                base.WriteObject(_views, true);
            }
        }

        #endregion

        #region CMDLET METHODS
        private void FilterByName(ref MgSmoCollection<View> views, WildcardPattern[] pats, Database db)
        {
            for (int i = 0; i < db.Views.Count; i++)
            {
                View v = db.Views[i];
                if (base.NameMatchesPatterns(v.Name, pats))
                    views.Add(v);
            }
        }

        private bool HasNames() => this.ViewName != null && this.ViewName.Length > 0;

        #endregion
    }
}