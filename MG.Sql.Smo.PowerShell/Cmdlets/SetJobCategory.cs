using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Set, "JobCategory", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true, DefaultParameterSetName = "ByPipelineInput")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(void))]
    public class SetJobCategory : BaseForceSqlCmdlet
    {
        private JobServer _js;

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByPipelineInput")]
        public JobCategory InputObject { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByJobCategoryIdentifier")]
        public object Identity { get; set; }

        [Parameter(Mandatory = false)]
        public string NewName { get; set; }

        [Parameter(Mandatory = false)]
        public CategoryType NewType { get; set; }

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
            var jobCat = this.GetJobCategory(this.Identity);
            if (base.Force || base.ShouldProcess(jobCat.Name, "Modifying properties"))
            {
                if (this.MyInvocation.BoundParameters.ContainsKey("NewType") && jobCat.CategoryType != this.NewType)
                    jobCat.CategoryType = this.NewType;

                else if (this.MyInvocation.BoundParameters.ContainsKey("NewType"))
                    base.WriteWarning(jobCat.Name + " is already of type " + this.NewType.ToString() + ".");

                if (this.MyInvocation.BoundParameters.ContainsKey("NewName") && !jobCat.Name.Equals(this.NewName, StringComparison.CurrentCulture))
                    jobCat.Rename(this.NewName);

                else if (this.MyInvocation.BoundParameters.ContainsKey("NewName"))
                    base.WriteWarning("The new name specified is the same as the existing name (" + jobCat.Name + ").");
            }
        }

        #endregion

        #region METHODS
        private JobCategory GetJobCategory(object identity)
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("InputObject"))
                return this.InputObject;

            else if (identity is ValueType vt)
                return _js.JobCategories.ItemById(Convert.ToInt32(vt));

            else if (identity is string catName)
                return _js.JobCategories[catName];

            else
                throw new ArgumentException("No job category with an identity of '" + Convert.ToString(identity) + "' could be found.");
        }

        #endregion
    }
}
