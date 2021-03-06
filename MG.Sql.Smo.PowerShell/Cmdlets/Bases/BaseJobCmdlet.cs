﻿using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    public abstract class JobModifyBaseCmdlet : BaseForceSqlCmdlet
    {
        protected private SmoJob _input;
        internal const string JOBNAME = "JobName";
        protected private const string JOBID = "JobId";
        protected private const string JOB_CAP = "Job - {0}";
        protected private MgSmoCollection<Microsoft.SqlServer.Management.Smo.Agent.Job> _jobs;

        #region PARAMETERS
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByPipelineInput", ValueFromPipeline = true, DontShow = true)]
        public SmoJob InputObject { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "By" + JOBNAME)]
        public string JobName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "By" + JOBID)]
        public Guid JobId { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            

            if (this.MyInvocation.BoundParameters.ContainsKey(JOBNAME))
            {
                if (this.SqlServer == null && SmoContext.GetNullOrEmpty(SmoContext.Jobs))
                    SmoContext.SetJobs(SmoContext.Connection.JobServer.Jobs);

                else if (this.SqlServer != null)
                    SmoContext.SetJobs(this.SqlServer.JobServer.Jobs);

                _input = this.GetJobFromName(this.JobName);
            }
                

            else if (this.MyInvocation.BoundParameters.ContainsKey(JOBID))
                _input = this.GetJobFromId(this.JobId, _server);
        }
        protected override void ProcessRecord()
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("InputObject"))
                _input = this.InputObject;
        }

        #endregion

        #region CMDLET METHODS
        protected private SmoJob GetJobFromName(string name)
        {
            SmoJob retJob = null;
            for (int i = 0; i < SmoContext.Jobs.Count; i++)
            {
                var job = SmoContext.Jobs[i];
                if (job.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    retJob = job;
                    break;
                }
            }
            return retJob;
        }
        protected private SmoJob GetJobFromId(Guid jobId, Server server)
        {
            return server.JobServer.Jobs.ItemById(jobId);
        }

        protected private JobStep GetJobStep(object input)
        {
            JobStep jobStep = null;
            if (input is int jobStepId && _input.JobSteps.TryFind(x => x.ID.Equals(jobStepId), out JobStep jsId))
                jobStep = jsId;

            else if (input is string jobStepName && _input.JobSteps.TryFind(
                x => x.Name.Equals(jobStepName, StringComparison.CurrentCultureIgnoreCase), out JobStep jsName))
                jobStep = jsName;

            return jobStep;
        }

        #endregion
    }
}