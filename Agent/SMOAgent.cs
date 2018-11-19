using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using SQL.SMO.Framework;
using System;
using System.Collections.Generic;

namespace SQL.SMO
{
    public class SMOAgent : SMOPropertyLoader
    {
        private JobServer _js;

        internal static readonly string[] SkipThese = new string[3] { "JobServerType", "ServiceAccount", "State" };

        #region All Properties
        public string AgentDomainGroup { get; internal set; }
        public AgentLogLevels? AgentLogLevel { get; internal set; }
        public AgentMailType? AgentMailType { get; internal set; }
        public int? AgentShutdownWaitTime { get; internal set; }
        public string DatabaseMailProfile { get; internal set; }
        public string ErrorLogFile { get; internal set; }
        public string HostLoginName { get; internal set; }
        public int? IdleCpuDuration { get; internal set; }
        public int? IdleCpuPercentage { get; internal set; }
        public bool? IsCpuPollingEnabled { get; internal set; }
        public string LocalHostAlias { get; internal set; }
        public int? LoginTimeout { get; internal set; }
        public int? MaximumHistoryRows { get; internal set; }
        public int? MaximumJobHistoryRows { get; internal set; }
        public string MsxAccountCredentialName { get; internal set; }
        public string MsxAccountName { get; internal set; }
        public string MsxServerName { get; internal set; }
        public string NetSendRecipient { get; internal set; }
        public bool? ReplaceAlertTokensEnabled { get; internal set; }
        public bool? SaveInSentFolder { get; internal set; }
        
        public ServiceStartMode? ServiceStartMode { get; internal set; }
        public bool? SqlAgentAutoStart { get; internal set; }
        public string SqlAgentMailProfile { get; internal set; }
        public bool? SqlAgentRestart { get; internal set; }
        public bool? SqlServerRestart { get; internal set; }
        public bool? WriteOemErrorLog { get; internal set; }
        public Server Parent { get; internal set; }
        public JobCategoryCollection JobCategories { get; internal set; }
        public OperatorCategoryCollection OperatorCategories { get; internal set; }
        public AlertCategoryCollection AlertCategories { get; internal set; }
        public AlertSystem AlertSystem { get; internal set; }
        public AlertCollection Alerts { get; internal set; }
        public OperatorCollection Operators { get; internal set; }
        public TargetServerCollection TargetServers { get; internal set; }
        public TargetServerGroupCollection TargetServerGroups { get; internal set; }
        public SMOJobCollection Jobs { get; internal set; }
        public JobScheduleCollection SharedSchedules { get; internal set; }
        public ProxyAccountCollection ProxyAccounts { get; internal set; }
        public bool? SysAdminOnly { get; internal set; }
        public Urn Urn { get; internal set; }
        public SqlPropertyCollection Properties { get; internal set; }
        public DatabaseEngineType? DatabaseEngineType { get; internal set; }
        public DatabaseEngineEdition? DatabaseEngineEdition { get; internal set; }
        public ExecutionManager ExecutionManager { get; internal set; }
        public object UserData { get; internal set; }
        public JobServerType? JobServerType { get; internal set; }

        #endregion

        #region Default Properties
        public override string Name { get; internal set; }
        public string ServiceAccount { get; internal set; }
        public SqlSmoState? State { get; internal set; }
        public override Type OriginalType => _js.GetType();

        #endregion

        #region Constructors
        private SMOAgent(JobServer js)
        {
            Name = js.Name;
            JobServerType = js.JobServerType;
            ServiceAccount = js.ServiceAccount;
            State = js.State;
            ServiceStartMode = js.ServiceStartMode;
            _js = js;
        }

        #endregion

        #region Methods
        public override object ShowOriginal() => _js;

        public override object Load(params string[] propertyNames)
        {
            if (propertyNames == null)
                return null;

            LoadValue(_js, propertyNames);
            return this;
        }

        #endregion

        #region Operators
        public static explicit operator SMOAgent(JobServer js) => new SMOAgent(js);
        public static explicit operator JobServer(SMOAgent smoa) => smoa.ShowOriginal() as JobServer;

        #endregion
    }
}
