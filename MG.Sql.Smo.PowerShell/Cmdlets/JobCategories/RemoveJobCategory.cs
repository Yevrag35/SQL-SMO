using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Remove, "JobCategory", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true, DefaultParameterSetName = "ByPipelineInput")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(void))]
    public class RemoveJobCategory : BaseForceSqlCmdlet
    {
        private JobServer _js;
        private List<JobCategory> _list;

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByPipelineInput")]
        public JobCategory InputObject { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByJobCategoryName")]
        public string[] Name { get; set; }

        [Parameter(Mandatory = false, DontShow = true)]
        public JobServer JobServer { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _list = new List<JobCategory>();
            _js = !this.MyInvocation.BoundParameters.ContainsKey("JobServer")
                ? SmoContext.Connection.JobServer
                : this.JobServer;
        }

        protected override void ProcessRecord() => this.GetJobCategory(this.Name);

        protected override void EndProcessing()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                var cat = _list[i];
                if (base.Force || base.ShouldProcess(_js.Name, "Removing job category - '" + cat.Name + "'"))
                    cat.Drop();
            }
            _js.Refresh();
        }

        #endregion

        #region METHODS
        private void GetJobCategory(params string[] catNames)
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("InputObject"))
                _list.Add(this.InputObject);

            else if (catNames != null && catNames.Length > 0)
            {
                for (int i = 0; i < catNames.Length; i++)
                {
                    _list.Add(_js.JobCategories[catNames[i]]);
                }
            }
        }

        #endregion
    }
}
