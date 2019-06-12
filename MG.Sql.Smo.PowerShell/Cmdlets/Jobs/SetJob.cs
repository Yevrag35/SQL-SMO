using MG.Dynamic;
using MG.Swappable;
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
    [Cmdlet(VerbsCommon.Set, "Job", ConfirmImpact = ConfirmImpact.High)]
    [OutputType(typeof(void))]
    [CmdletBinding(PositionalBinding = false)]
    public class SetJob : JobModifyBaseCmdlet
    {
        private static readonly string[] SkipThese = new string[3]
        {
            NAME, "InputObject", "Force"
        };
        private bool yesToAll = false;
        private bool noToAll = false;

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

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (this.HasSetValues(this.MyInvocation.BoundParameters) && 
                (base.Force || base.ShouldContinue("Setting Properties on Job", "Set", ref yesToAll, ref noToAll)))
            {
                this.SetSmoJob(this.MyInvocation.BoundParameters, _input);
            }
        }

        #endregion

        #region BACKEND METHODS
        private bool HasSetValues(Dictionary<string, object> parameters)
        {
            IEqualityComparer<string> comparer = new SmoContext.CaseInsensitiveComparer();
            return parameters.Any(x => !SkipThese.Contains(x.Key, comparer));
        }

        private void SetSmoJob(Dictionary<string, object> parameters, SmoJob job)
        {
            var sd = new SwappableDictionary(parameters);
            if (sd.ContainsKey("NewName"))
                sd.Swap(sd.NewSwappable(NAME, "NewName"));

            if (sd.ContainsKey("Enabled"))
                sd.Swap(sd.NewSwappable("IsEnabled", "Enabled"));

            for (int i = sd.Count - 1; i >= 0; i--)
            {
                KeyValuePair<string, object> kvp = sd.ElementAt(i);
                if (SkipThese.Contains(kvp.Key))
                    sd.Remove(kvp.Key);
            }

            var propList = job.GetType().GetProperties(FLAGS).Where(x => x.CanWrite && sd.ContainsKey(x.Name)).ToList();

            base.ChangeValues(job, sd, propList);

            job.Alter();
        }

        #endregion
    }
}
