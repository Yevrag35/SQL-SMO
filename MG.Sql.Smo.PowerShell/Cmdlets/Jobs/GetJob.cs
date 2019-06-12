using MG.Dynamic;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "Job", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "ByJobName")]
    [OutputType(typeof(SmoJob))]
    [CmdletBinding(PositionalBinding = false)]
    public class GetJob : BaseSqlCmdlet, IDynamicParameters
    {
        private DynamicLibrary _dynLib;
        private List<SmoJob> jobs;

        #region PARAMETERS
        [Parameter(Mandatory = false, ParameterSetName = "ByJobId")]
        public Guid JobId { get; set; }

        #endregion

        #region CMDLET PROCESSING
        public object GetDynamicParameters()
        {
            if (SmoContext.IsSet && SmoContext.IsConnected && _dynLib == null)
            {
                _dynLib = new DynamicLibrary();
                var dp = new DynamicParameter<Microsoft.SqlServer.Management.Smo.Agent.Job>(
                    "JobName", SmoContext.Connection.JobServer.Jobs.Cast<Microsoft.SqlServer.Management.Smo.Agent.Job>(), 
                    x => x.Name, "Name", true)
                {
                    Position = 0,
                    Mandatory = false
                };
                _dynLib.Add(dp);
            }
            return _dynLib;
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            jobs = new List<SmoJob>();
            if (this.MyInvocation.BoundParameters.ContainsKey("JobId"))
                jobs.Add(SmoContext.Connection.JobServer.Jobs.ItemById(this.JobId));

            else
                jobs.AddRange(_dynLib.GetUnderlyingValues<SmoJob>("JobName"));
        }

        protected override void ProcessRecord()
        {
            WriteObject(jobs, true);
        }

        #endregion
    }
}