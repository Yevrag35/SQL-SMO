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
    public class GetJob : BaseServerSqlCmdlet, IDynamicParameters
    {
        private DynamicLibrary _dynLib;
        private List<SmoJob> jobs;

        #region PARAMETERS
        [Parameter(Mandatory = false, ParameterSetName = "ByJobId")]
        public Guid JobId { get; set; }

        #endregion

        #region DYNAMIC PARAMETERS
        public object GetDynamicParameters()
        {
            if (SmoContext.IsSet && SmoContext.IsConnected && _dynLib == null)
            {
                if (SmoContext.GetNullOrEmpty(SmoContext.Jobs))
                    SmoContext.SetJobs(SmoContext.Connection.JobServer.Jobs);

                _dynLib = new DynamicLibrary();
                var dp = new DynamicParameter<Microsoft.SqlServer.Management.Smo.Agent.Job>("JobName", SmoContext.Jobs, x => x.Name, "Name", true)
                {
                    Position = 0,
                    Mandatory = false,
                    ParameterSetName = "ByJobName"
                };
                _dynLib.Add(dp);
            }
            else if (_dynLib == null)
            {
                _dynLib = new DynamicLibrary();
                _dynLib.Add("JobName", new RuntimeDefinedParameter("JobName", typeof(string[]), new Collection<Attribute>
                {
                    new ParameterAttribute
                    {
                        Mandatory = false,
                        Position = 0,
                        ParameterSetName = "ByJobName"
                    }
                }));
            }
            return _dynLib;
        }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            jobs = new List<SmoJob>();
            if (!this.JobId.Equals(Guid.Empty))
                jobs.Add(SmoContext.Connection.JobServer.Jobs.ItemById(this.JobId));

            else if (_dynLib != null && _dynLib.ParameterHasValue("JobName"))
                jobs.AddRange(_dynLib.GetUnderlyingValues<SmoJob>("JobName"));

            else if (this.SqlServer == null && _dynLib != null)
                jobs.AddRange(_dynLib.GetBackingItems<SmoJob>("JobName"));
        }

        protected override void ProcessRecord()
        {
            if (jobs != null && jobs.Count > 0)
            {
                base.WriteObject(jobs, true);
            }
            else if (this.SqlServer != null)
            {
                var jobCol = this.SqlServer.JobServer.Jobs;
                if (_dynLib != null && _dynLib.ParameterHasValue("JobName"))
                {
                    string[] names = _dynLib.GetParameterValues<string>("JobName");
                    for (int i = 0; i < jobCol.Count; i++)
                    {
                        Microsoft.SqlServer.Management.Smo.Agent.Job job = jobCol[i];
                        if (names.Contains(job.Name, new SmoContext.CaseInsensitiveComparer()))
                        {
                            SmoJob smoj = job;
                            base.WriteObject(smoj);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < jobCol.Count; i++)
                    {
                        SmoJob smoj = jobCol[i];
                        base.WriteObject(smoj);
                    }
                }
            }
            else
                throw new ArgumentException("SqlServer can't be null if you don't connect to an instance.");
        }

        #endregion
    }
}