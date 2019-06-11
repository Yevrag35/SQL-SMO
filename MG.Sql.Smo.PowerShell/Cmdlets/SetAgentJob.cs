using MG.Dynamic;
using MG.Sql.Smo.PowerShell.Backend;
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
    [Cmdlet(VerbsCommon.Set, "AgentJob", ConfirmImpact = ConfirmImpact.High)]
    [OutputType(typeof(void))]
    [CmdletBinding(PositionalBinding = false)]
    public class SetAgentJob : AgentJobModifyBaseCmdlet
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
                this.SetJob(this.MyInvocation.BoundParameters, _input);
            }
        }

        #endregion

        #region BACKEND METHODS
        private bool HasSetValues(Dictionary<string, object> parameters)
        {
            IEqualityComparer<string> comparer = new SmoContext.CaseInsensitiveComparer();
            return parameters.Any(x => !SkipThese.Contains(x.Key, comparer));
        }

        private void ResolveLists(ref SwappableDictionary swapDict, ref List<PropertyInfo> propList)
        {
            IEqualityComparer<string> comparer = new SmoContext.CaseInsensitiveComparer();
            for (int p = propList.Count - 1; p >= 0; p--)
            {
                PropertyInfo pi = propList[p];
                if (SkipThese.Contains(pi.Name))
                    propList.Remove(pi);

                else if (!swapDict.ContainsKey(pi.Name) && swapDict.ContainsKey(pi.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    swapDict.Swap(pi.Name, StringComparison.CurrentCultureIgnoreCase);
                }
                else if (!swapDict.ContainsKey(pi.Name, StringComparison.CurrentCultureIgnoreCase))
                    propList.Remove(pi);
            }
        }

        //private List<PropertyInfo> GetPropertiesFromKeys(Type jobType, ref List<string> keyList)
        //{
        //    var pis = jobType.GetProperties(FLAGS).ToList();
        //    for (int i = pis.Count - 1; i >= 0 ; i--)
        //    {
        //        PropertyInfo pi = pis[i];
        //        string key = keyList.Find(x => x.Equals(pi.Name, StringComparison.CurrentCultureIgnoreCase));
        //        if (string.IsNullOrEmpty(key))
        //            pis.Remove(pi);
        //    }
        //    //return jobType.GetProperties(FLAGS).Where(
        //    //    x => keyList.Exists(
        //    //        n => n.Equals(
        //    //            x.Name, StringComparison.CurrentCultureIgnoreCase))).ToList();
        //}

        private void SetJob(Dictionary<string, object> parameters, SmoJob job)
        {
            var sd = new SwappableDictionary(parameters);
            if (sd.ContainsKey("NewName"))
                sd.Swap(sd.NewSwappable(NAME, "NewName"));

            if (sd.ContainsKey("Enabled"))
                sd.Swap(sd.NewSwappable("IsEnabled", "Enabled"));

            var propList = job.GetType().GetProperties(FLAGS).Where(x => x.CanWrite && parameters.ContainsKey(x.Name)).ToList();

            base.ChangeValues(job, sd, propList);

            job.Alter();
        }

        #endregion
    }
}
