using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Set, "SmoAgentJob", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    public class SetSmoAgentJob : GetSmoAgentJob
    {
        private List<Microsoft.SqlServer.Management.Smo.Agent.Job> list;
        private static readonly string[] SkipThese = new string[3]
        {
            NAME, "InputObject", "Force"
        };

        #region PARAMETERS
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByPipelineInput", ValueFromPipeline = true, DontShow = true)]
        public Microsoft.SqlServer.Management.Smo.Agent.Job InputObject { get; set; }

        [Parameter(Mandatory = false)]
        public string Category { get; set; }

        [Parameter(Mandatory = false)]
        public byte CategoryType { get; set; }

        [Parameter(Mandatory = false)]
        public CompletionAction DeleteLevel { get; set; }

        [Parameter(Mandatory = false)]
        public string Description { get; set; }

        [Parameter(Mandatory = false)]
        public CompletionAction EmailLevel { get; set; }

        [Parameter(Mandatory = false)]
        public CompletionAction EventLogLevel { get; set; }

        [Parameter(Mandatory = false)]
        public bool Enabled { get; set; }

        [Parameter(Mandatory = false)]
        public string NewName { get; set; }

        [Parameter(Mandatory = false)]
        public CompletionAction NetSendLevel { get; set; }

        [Parameter(Mandatory = false)]
        public string OperatorToEmail { get; set; }

        [Parameter(Mandatory = false)]
        public string OperatorToNetSend { get; set; }

        [Parameter(Mandatory = false)]
        public string OperatorToPage { get; set; }

        [Parameter(Mandatory = false)]
        public string OwnerLoginName { get; set; }

        [Parameter(Mandatory = false)]
        public CompletionAction PageLevel { get; set; }

        [Parameter(Mandatory = false)]
        public int StartStepId { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }


        #endregion
        private IEnumerable<PropertyInfo> Properties;

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            list = new List<Microsoft.SqlServer.Management.Smo.Agent.Job>();
            Properties = this.GetJobProperties();
        }

        protected override void ProcessRecord()
        {
            if (this.ParameterSet)
                list.AddRange(base.GetJobsByName(GetChosenValues<string>(pName, rtDict)));

            else if (this.ParameterSetName == "ByPipelineInput")
                list.Add(InputObject);

            else
                list.AddRange(base.GetAllJobs().Cast<Microsoft.SqlServer.Management.Smo.Agent.Job>());
        }

        protected override void EndProcessing()
        {
            for (int i = 0; i < list.Count; i++)
            {
                var job = list[i];
                if (Force || ShouldProcess("Job - " + job.Name, "Set"))
                {
                    this.SetJob(this.MyInvocation.BoundParameters, job);
                }
            }
        }

        #endregion

        #region CMDLET METHODS
        private void SetJob(Dictionary<string, object> dict, Microsoft.SqlServer.Management.Smo.Agent.Job job)
        {
            foreach (KeyValuePair<string, object> entry in dict.Where(x => !SkipThese.Contains(x.Key)))
            {
                PropertyInfo pi = Properties.Single(x => x.Name.Contains(entry.Key));

                pi.SetValue(job, entry.Value);
            }

            job.Alter();
        }

        private IEnumerable<PropertyInfo> GetJobProperties()
        {
            return typeof(Microsoft.SqlServer.Management.Smo.Agent.Job).GetProperties(FLAGS).Where(
                x => x.CanWrite);
        }

        #endregion
    }
}