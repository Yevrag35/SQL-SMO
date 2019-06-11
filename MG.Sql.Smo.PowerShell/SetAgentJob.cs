using MG.Dynamic;
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
    [Cmdlet(VerbsCommon.Set, "AgentJob", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    [OutputType(typeof(void))]
    [CmdletBinding(PositionalBinding = false)]
    public class SetAgentJob : JobAgentBaseCmdlet
    {
        private static readonly string[] SkipThese = new string[3]
        {
            NAME, "InputObject", "Force"
        };
        private bool yesToAll = false;
        private bool noToAll = false;
        private ShouldProcessReason _reason;

        #region PARAMETERS

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

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (Force || ShouldContinue("Setting Properties on Job", "Set", ref yesToAll, ref noToAll))
            {
                this.SetJob(this.MyInvocation.BoundParameters, _input);
            }
        }

        #endregion

        #region BACKEND METHODS
        private void SetJob(Dictionary<string, object> dict, SmoJob job)
        {
            var props = job.GetType().GetProperties(FLAGS).Where(x => x.CanWrite);
            foreach (KeyValuePair<string, object> entry in dict.Where(x => !SkipThese.Contains(x.Key)))
            {
                PropertyInfo pi = props.Single(x => x.Name.Contains(entry.Key));
                pi.SetValue(job, entry.Value);
            }

            job.Alter();
        }

        #endregion
    }
}
