using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "JobCategory", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "ByJobCategoryName")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(JobCategory))]
    public class GetJobCategory : BaseSqlCmdlet
    {
        private JobServer _js;

        #region PARAMETERS
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "ByJobCategoryName")]
        [SupportsWildcards]
        public string Name { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "ByJobCategoryID")]
        public int[] Id { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "ByJobCategoryName")]
        public CategoryType[] Type { get; set; }

        [Parameter(Mandatory = false, DontShow = true)]
        public JobServer JobServer { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _js = !this.MyInvocation.BoundParameters.ContainsKey("JobServer")
                ? SmoContext.Connection.JobServer
                : this.JobServer;
        }

        protected override void ProcessRecord()
        {
            var list = new List<JobCategory>(_js.JobCategories.Cast<JobCategory>());
            if (!this.MyInvocation.BoundParameters.ContainsKey("Id"))
            {
                this.FilterByName(ref list);
                this.FilterByType(ref list);
            }
            else
            {
                this.FilterByID(ref list);
            }
            list.Sort(new JobCategoryComparer());
            base.WriteObject(list, true);
        }

        #endregion

        #region METHODS
        private void FilterByID(ref List<JobCategory> cats)
        {
            for (int i = cats.Count - 1; i >= 0; i--)
            {
                JobCategory cat = cats[i];
                if (!this.Id.Contains(cat.ID))
                    cats.Remove(cat);
            }
        }
        private void FilterByName(ref List<JobCategory> cats)
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("Name"))
            {
                var wcp = new WildcardPattern(this.Name, WildcardOptions.IgnoreCase);
                for (int i = cats.Count  - 1; i >= 0; i--)
                {
                    JobCategory cat = cats[i];
                    if (!wcp.IsMatch(cat.Name))
                        cats.Remove(cat);
                }
            }
        }
        private void FilterByType(ref List<JobCategory> cats)
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("Type"))
            {
                for (int i = cats.Count - 1; i >= 0; i--)
                {
                    JobCategory cat = cats[i];
                    if (!this.Type.Contains(cat.CategoryType))
                        cats.Remove(cat);
                }
            }
        }

        private class JobCategoryComparer : IComparer<JobCategory>
        {
            public int Compare(JobCategory x, JobCategory y)
            {
                int retInt = x.Parent.Name.CompareTo(y.Parent.Name);
                if (retInt == 0)
                {
                    retInt = x.ID.CompareTo(y.ID);
                }
                return retInt;
            }
        }

        #endregion
    }
}

