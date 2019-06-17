using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Set, "ServerConfig", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    public class SetServerConfig : BaseSqlCmdlet
    {
        private static readonly string[] SkipThese = new string[5]
        {
            "Force", "Verbose", "WhatIf", "Debug", "ErrorAction"
        };

        #region PARAMETERS

        [Parameter(Mandatory = false)]
        public bool AdHocDistributedQueriesEnabled { get; set; }

        [Parameter(Mandatory = false)]
        public int Affinity64IOMask { get; set; }

        [Parameter(Mandatory = false)]
        public int Affinity64Mask { get; set; }

        [Parameter(Mandatory = false)]
        public int AffinityIOMask { get; set; }

        [Parameter(Mandatory = false)]
        public int AffinityMask { get; set; }

        [Parameter(Mandatory = false)]
        public bool AgentXPsEnabled { get; set; }

        [Parameter(Mandatory = false)]
        public bool AweEnabled { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, 86400)]
        public int BlockedProcessThreshold { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice C2AuditMode { get; set; }

        [Parameter(Mandatory = false)]
        public bool CommonCriteriaComplianceEnabled { get; set; }

        [Parameter(Mandatory = false)]
        public bool ContainmentEnabled { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, 32767)]
        public int CostThresholdForParallelism { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice CrossDBOwnershipChaining { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(-1, 2147483647)]
        public int CursorThreshold { get; set; }

        [Parameter(Mandatory = false)]
        public bool DatabaseMailEnabled { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice DefaultBackupCompression { get; set; }

        [Parameter(Mandatory = false)]
        public int DefaultFullTextLanguage { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, 9999)]
        public int DefaultLanguage { get; set; }

        [Parameter(Mandatory = false)]
        public bool DefaultTraceEnabled { get; set; }

        [Parameter(Mandatory = false)]
        public bool DisallowResultsFromTriggers { get; set; }

        [Parameter(Mandatory = false)]
        public bool ExtensibleKeyManagementEnabled { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, 2)]
        public int FilestreamAccessLevel { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, 100)]
        public int FillFactor { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, 32767)]
        public int FullTextCrawlBandwidthMax { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, 32767)]
        public int FullTextCrawlBandwidthMin { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, 256)]
        public int FullTextCrawlRangeMax { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, 32767)]
        public int FullTextNotifyBandwidthMax { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, 32767)]
        public int FullTextNotifyBandwidthMin { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(704, 2147483647)]
        public int IndexCreateMemory { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, 2)]
        public int InDoubtTransactionResolution { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice LightweightPooling { get; set; }

        [Parameter(Mandatory = false)]
        public int Locks { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, 32767)]
        public int MaxDegreeOfParallelism { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(128, 2147483647)]
        public int MaxServerMemory { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, 65535)]
        public int MaxWorkerThreads { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, 365)]
        public int MediaRetention { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(512, 2147483647)]
        public int MinMemoryPerQuery { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice NestedTriggers { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(512, 32767)]
        public int NetworkPacketSize { get; set; }

        [Parameter(Mandatory = false)]
        public bool OleAutomationProceduresEnabled { get; set; }

        [Parameter(Mandatory = false)]
        public int OpenObjects { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice OptimizeAdHocWorkloads { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice PrecomputeRank { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice PriorityBoost { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(1, 3600)]
        public int ProtocolHandlerTimeout { get; set; }

        [Parameter(Mandatory = false)]
        public int QueryGovernorCostLimit { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(-1, 2147483647)]
        public int QueryWait { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, 32767)]
        public int RecoveryInterval { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice RemoteAccess { get; set; }

        [Parameter(Mandatory = false)]
        public bool RemoteDacConnectionsEnabled { get; set; }

        [Parameter(Mandatory = false)]
        public bool RemoteDataArchiveEnabled { get; set; }

        [Parameter(Mandatory = false)]
        public int RemoteLoginTimeout { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice RemoteProcTrans { get; set; }

        [Parameter(Mandatory = false)]
        public int RemoteQueryTimeout { get; set; }

        [Parameter(Mandatory = false)]
        public int ReplicationMaxTextSize { get; set; }

        [Parameter(Mandatory = false)]
        public bool ReplicationXPsEnabled { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice ScanForStartupProcedures { get; set; }

        [Parameter(Mandatory = false)]
        public bool ServerTriggerRecursionEnabled { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice SetWorkingSetSize { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice ShowAdvancedOptions { get; set; }

        [Parameter(Mandatory = false)]
        public bool SmoAndDmoXPsEnabled { get; set; }

        [Parameter(Mandatory = false)]
        public bool SqlMailXPsEnabled { get; set; }

        [Parameter(Mandatory = false)]
        public BinaryChoice TransformNoiseWords { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(1753, 9999)]
        public int TwoYearDigitCutoff { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, 32767)]
        public int UserConnections { get; set; }

        //[Parameter(Mandatory = false)]
        //public bool UserInstancesEnabled { get; set; }

        //[Parameter(Mandatory = false)]
        //public int UserInstanceTimeout { get; set; }

        //[Parameter(Mandatory = false)]
        //public bool WebXPsEnabled { get; set; }

        [Parameter(Mandatory = false)]
        public bool XPCmdShellEnabled { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            foreach (KeyValuePair<string, object> entry in this.MyInvocation.BoundParameters.Where(x => !SkipThese.Contains(x.Key)))
            {
                SmoConfiguration.SetPropertyValue(SmoContext.Connection.Configuration, entry.Key, entry.Value);
            }
            try
            {
                SmoContext.Connection.Configuration.Alter();
            }
            catch (Exception e)
            {
                base.ThrowInnerException(e);
            }
        }
    }
}
