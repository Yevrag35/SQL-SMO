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
    [Cmdlet(VerbsCommon.Set, "SmoAgentServer", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true, 
        DefaultParameterSetName = "Individual")]
    [OutputType(typeof(void))]
    public class SetSmoAgentServer : GetSmoAgentServer
    {
        #region PARAMETERS

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public AgentLogLevels AgentLogLevel { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public AgentMailType AgentMailType { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public int AgentShutdownWaitTime { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public BinaryChoice CpuPolling { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public string DatabaseMailProfile { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public string ErrorLogFile { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public int IdleCpuDuration { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public int IdleCpuPercentage { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public string LocalHostAlias { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public int LoginTimeout { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public int MaximumHistoryRows { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public int MaximumJobHistoryRows { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public string NetSendRecipient { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public BinaryChoice ReplaceAlertTokens { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public bool SaveInSentFolder { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public BinaryChoice SqlAgentAutoStart { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public string SqlAgentMailProfile { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public BinaryChoice SqlAgentRestart { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public BinaryChoice SqlServerRestart { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public BinaryChoice WriteOemErrorLog { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            if (Force || ShouldProcess("JobServer", "Set"))
                this.SetJobServer(this.MyInvocation.BoundParameters);
        }

        #endregion

        #region CMDLET METHODS
        private void SetJobServer(Dictionary<string, object> dict)
        {
            IEnumerable<PropertyInfo> writProps = typeof(JobServer).GetProperties(FLAGS).Where(
                x => x.CanWrite);

            JobServer js = base.GetJobServer();

            foreach (KeyValuePair<string, object> entry in dict.Where(x => !x.Key.Equals("Force")))
            {
                PropertyInfo pi = writProps.Single(x => x.Name.Contains(entry.Key));

                object val = entry.Value is BinaryChoice bin
                    ? Convert.ToBoolean(bin)
                    : entry.Value;
                pi.SetValue(js, val);
            }

            js.Alter();
        }

        #endregion
    }
}