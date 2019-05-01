using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace MG.Sql.Smo
{
    public class SmoConfiguration
    {
        private const string GET_ITEM = "get_Item";
        private const string LOWER_NAME = "name";
        private readonly Configuration _config;

        #region PROPERTIES
        public bool? AdHocDistributedQueriesEnabled { get; private set; }
        public int? Affinity64IOMask { get; private set; }
        public int? Affinity64Mask { get; private set; }
        public int? AffinityIOMask { get; private set; }
        public int? AffinityMask { get; private set; }
        public bool? AgentXPsEnabled { get; private set; }
        public bool? AllowUpdates { get; private set; }
        public int? AweEnabled { get; private set; }
        public int? BlockedProcessThreshold { get; private set; }
        public BinaryChoice? C2AuditMode { get; private set; }
        public bool? CommonCriteriaComplianceEnabled { get; private set; }
        public bool? ContainmentEnabled { get; private set; }
        public int? CostThresholdForParallelism { get; private set; }
        public BinaryChoice? CrossDBOwnershipChaining { get; private set; }
        public int? CursorThreshold { get; private set; }
        public bool? DatabaseMailEnabled { get; private set; }
        public BinaryChoice? DefaultBackupCompression { get; private set; }
        public int? DefaultFullTextLanguage { get; private set; }
        public int? DefaultLanguage { get; private set; }
        public bool? DefaultTraceEnabled { get; private set; }
        public bool? DisallowResultsFromTriggers { get; private set; }
        public bool? ExtensibleKeyManagementEnabled { get; private set; }
        public int? FilestreamAccessLevel { get; private set; }
        public int? FillFactor { get; private set; }
        public int? FullTextCrawlBandwidthMax { get; private set; }
        public int? FullTextCrawlBandwidthMin { get; private set; }
        public int? FullTextCrawlRangeMax { get; private set; }
        public int? FullTextNotifyBandwidthMax { get; private set; }
        public int? FullTextNotifyBandwidthMin { get; private set; }
        public int? IndexCreateMemory { get; private set; }
        public int? InDoubtTransactionResolution { get; private set; }
        public bool? IsSqlClrEnabled { get; private set; }
        public BinaryChoice? LightweightPooling { get; private set; }
        public int? Locks { get; private set; }
        public int? MaxDegreeOfParallelism { get; private set; }
        public int? MaxServerMemory { get; private set; }
        public int? MaxWorkerThreads { get; private set; }
        public int? MediaRetention { get; private set; }
        public int? MinMemoryPerQuery { get; private set; }
        public int? MinServerMemory { get; private set; }
        public BinaryChoice? NestedTriggers { get; private set; }
        public int? NetworkPacketSize { get; private set; }
        public bool? OleAutomationProceduresEnabled { get; private set; }
        public int? OpenObjects { get; private set; }
        public BinaryChoice? OptimizeAdhocWorkloads { get; private set; }
        public BinaryChoice? PrecomputeRank { get; private set; }
        public BinaryChoice? PriorityBoost { get; private set; }
        public int? ProtocolHandlerTimeout { get; private set; }
        public int? QueryGovernorCostLimit { get; private set; }
        public int? QueryWait { get; private set; }
        public int? RecoveryInterval { get; private set; }
        public BinaryChoice? RemoteAccess { get; private set; }
        public bool? RemoteDacConnectionsEnabled { get; private set; }
        public bool? RemoteDataArchiveEnabled { get; private set; }
        public int? RemoteLoginTimeout { get; private set; }
        public BinaryChoice? RemoteProcTrans { get; private set; }
        public int? RemoteQueryTimeout { get; private set; }
        public int? ReplicationMaxTextSize { get; private set; }
        public bool? ReplicationXPsEnabled { get; private set; }
        public BinaryChoice? ScanForStartupProcedures { get; private set; }
        public bool? ServerTriggerRecursionEnabled { get; private set; }
        public BinaryChoice? SetWorkingSetSize { get; private set; }
        public BinaryChoice? ShowAdvancedOptions { get; private set; }
        public bool? SmoAndDmoXPsEnabled { get; private set; }
        public bool? SqlMailXPsEnabled { get; private set; }
        public BinaryChoice? TransformNoiseWords { get; private set; }
        public int? TwoDigitYearCutoff { get; private set; }
        public int? UserConnections { get; private set; }
        public bool? UserInstancesEnabled { get; private set; }
        public int? UserInstanceTimeout { get; private set; }
        public bool? WebXPsEnabled { get; private set; }
        public bool? XPCmdShellEnabled { get; private set; }

        #endregion

        #region CONSTRUCTORS
        public SmoConfiguration(Configuration config)
        {
            _config = config;
            this.MatchProperties(config.Properties);
        }

        #endregion

        #region METHODS
        private void MatchProperties(ConfigPropertyCollection col)
        {
            Type colType = col.GetType();
            MethodInfo indexer = colType.GetMethods().Single(x => x.Name == GET_ITEM &&
                x.GetParameters().Any(p => p.Name.Equals(LOWER_NAME, StringComparison.CurrentCultureIgnoreCase) &&
                    p.ParameterType.Equals(typeof(string))));

            IEnumerable<PropertyInfo> allProps = this.GetType().GetProperties(
                BindingFlags.Instance | BindingFlags.Public).Where(
                    x => x.CanWrite);

            foreach (PropertyInfo pi in allProps)
            {
                object val = null;
                try
                {
                    val = indexer.Invoke(col, new object[1] { pi.Name });
                }
                catch (TargetInvocationException) { }
                if (val != null && val is ConfigProperty cp)
                {
                    if (pi.PropertyType.Equals(typeof(bool?)))
                    {
                        bool yesno = Convert.ToBoolean(cp.RunValue);
                        pi.SetValue(this, yesno);
                    }
                    else if (pi.PropertyType.Equals(typeof(BinaryChoice?)))
                    {
                        pi.SetValue(this, (BinaryChoice)cp.RunValue);
                    }
                    else
                        pi.SetValue(this, cp.RunValue);
                }
            }
        }

        public static void SetPropertyValue(Configuration config, string propertyName, object value)
        {
            Type colType = config.Properties.GetType();
            MethodInfo indexer = colType.GetMethods().Single(x => x.Name == GET_ITEM &&
                x.GetParameters().Any(p => p.Name.Equals(LOWER_NAME, StringComparison.CurrentCultureIgnoreCase) &&
                    p.ParameterType.Equals(typeof(string))));

            if (value != null)
            {
                int setObj;
                if (value is BinaryChoice bc)
                    setObj = Convert.ToInt32(bc);

                else if (value is bool b)
                    setObj = Convert.ToInt32(b);

                else
                    setObj = (int)value;

                var cp = (ConfigProperty)indexer.Invoke(config.Properties, new object[1] { propertyName });
                cp.ConfigValue = setObj;
            }
        }

        #endregion
    }
}