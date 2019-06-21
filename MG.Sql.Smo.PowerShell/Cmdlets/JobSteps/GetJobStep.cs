using MG.Sql.Smo.PowerShell.Extensions;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "JobStep", ConfirmImpact = ConfirmImpact.None)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(JobStep))]
    public class GetJobStep : BaseSqlCmdlet
    {
        #region FIELDS/CONSTANTS
        //private List<SmoJob> _jobs;

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public SmoJob Job { get; set; }

        [Parameter(Mandatory = false, Position = 0)]
        [SupportsWildcards]
        public NumStrIdentity Identity { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            
        }

        #endregion

        #region METHODS
        protected private JobStep GetJobStep(NumStrIdentity input)
        {
            JobStep jobStep = null;
            if (input.IsNumeric && this.Job.JobSteps.TryFind(x => x.ID.Equals((int)input), out JobStep jStep))
            {
                jobStep = jStep;
            }

            else if (input.IsString)
            {
                var newCol = this.Job.JobSteps.WildcardMatch("Name", (string)input);
            }

            //if (input is int jobStepId && _input.JobSteps.TryFind(x => x.ID.Equals(jobStepId), out JobStep jsId))
            //    jobStep = jsId;

            //else if (input is string jobStepName && _input.JobSteps.TryFind(
            //    x => x.Name.Equals(jobStepName, StringComparison.CurrentCultureIgnoreCase), out JobStep jsName))
            //    jobStep = jsName;

            //return jobStep;
        }

        #endregion
    }
}