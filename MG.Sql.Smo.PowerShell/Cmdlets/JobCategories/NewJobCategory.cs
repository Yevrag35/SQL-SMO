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
    [Cmdlet(VerbsCommon.New, "JobCategory", ConfirmImpact = ConfirmImpact.Low, SupportsShouldProcess = true)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(JobCategory))]
    public class NewJobCategory : BaseSqlCmdlet
    {
        private JobServer _js;

        #region PARAMETERS
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public CategoryType Type { get; set; }

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

            if (_js.JobCategories.Contains(this.Name))
                throw new JobCategoryAlreadyExistsException(this.Name);
        }

        protected override void ProcessRecord()
        {
            if (base.ShouldProcess(_js.Name, "Adding new job category - '" + this.Name + "'"))
            {
                var jobCat = new JobCategory(_js, this.Name)
                {
                    CategoryType = this.Type
                };

                try
                {
                    jobCat.Create();
                }
                catch (FailedOperationException foe)
                {
                    base.ThrowInnerException(foe);
                }
                _js.JobCategories.Refresh();
                WriteObject(jobCat);
            }
        }

        #endregion

        #region METHODS


        #endregion
    }
}
