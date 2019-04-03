using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.SqlServer.Management.Smo.Mail;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG.Sql
{
    public class SmoServer
    {
        public ServerActiveDirectory ActiveDirectory { get; internal set; }
        public AffinityInfo AffinityInfo { get; internal set; }
        public AuditLevel? AuditLevel { get; set; }
        public AuditCollection Audits { get; internal set; }
        public AvailabilityGroupCollection AvailabilityGroups { get; internal set; }
        public BackupDeviceCollection BackupDevices { get; internal set; }
        public string BackupDirectory { get; set; }
        public string BrowserServiceAccount { get; internal set; }
        public ServiceStartMode? BrowserStartMode { get; internal set; }
        public Version BuildClrVersion { get; internal set; }
        public string BuildClrVersionString { get; internal set; }
        public int? BuildNumber { get; internal set; }
        public string ClusterName { get; internal set; }
        public ClusterQuorumState? ClusterQuorumState { get; internal set; }
        public ClusterQuorumType? ClusterQuorumType { get; internal set; }
        public string Collation { get; internal set; }
        public int? CollationID { get; internal set; }
        public int? ComparisonStyle { get; internal set; }
        public string ComputerNamePhysicalNetBIOS { get; internal set; }
        public Configuration Configuration { get; internal set; }
        public ServerConnection ConnectionContext { get; internal set; }
        public CredentialCollection Credentials { get; internal set; }
        public CryptographicProviderCollection CryptographicProviders { get; internal set; }
        public DatabaseEngineEdition? DatabaseEngineEdition { get; internal set; }
        public DatabaseEngineType? DatabaseEngineType { get; internal set; }
        public DatabaseCollection Databases { get; internal set; }
        public AvailabilityGroupClusterType? DefaultAvailabilityGroupClusterType { get; internal set; }
        public string DefaultFile { get; set; }
        public string DefaultLog { get; set; }
        public bool? DefaultTextMode { get; set; }
        public string DomainInstanceName { get; internal set; }
        public string DomainName { get; internal set; }
        public string Edition { get; internal set; }
        public EndpointCollection Endpoints { get; internal set; }
        public Edition? EngineEdition { get; internal set; }
        public string ErrorLogPath { get; internal set; }
        public ServerEvents Events { get; internal set; }
        public ExecutionManager ExecutionManager { get; internal set; }
        public FileStreamEffectiveLevel? FilestreamLevel { get; internal set; }
        public string FilestreamShareName { get; internal set; }
        public FullTextService FullTextService { get; internal set; }
        public HadrManagerStatus? HadrManagerStatus { get; internal set; }
        public string HostDistribution { get; internal set; }
        public string HostPlatform { get; internal set; }
        public string HostRelease { get; internal set; }
        public string HostServicePackLevel { get; internal set; }
        public int? HostSku { get; internal set; }
        public Information Information { get; internal set; }
        public string InstallDataDirectory { get; internal set; }
        public string InstallSharedDirectory { get; internal set; }
        public string InstanceName { get; internal set; }
        public bool? IsAvailabilityReplicaSeedingModeSupported { get; internal set; }
        public bool? IsCaseSensitive { get; internal set; }
        public bool? IsClustered { get; internal set; }
        public bool? IsConfigurationOnlyAvailabilityReplicaSupported { get; internal set; }
        public bool? IsCrossPlatformAvailabilityGroupSupported { get; internal set; }
        public bool? IsDesignMode { get; internal set; }
        public bool? IsFullTextInstalled { get; internal set; }
        public bool? IsHadrEnabled { get; internal set; }
        public bool? IsMemberOfWsfcCluster { get; internal set; }
        public bool? IsPolyBaseInstalled { get; internal set; }
        public bool? IsReadOnlyListWithLoadBalancingSupported { get; internal set; }
        public bool? IsSingleUser { get; internal set; }
        public bool? IsXTPSupported { get; internal set; }
        public JobServer JobServer { get; internal set; }
        public string Language { get; internal set; }
        public LanguageCollection Languages { get; internal set; }
        public LinkedServerCollection LinkedServers { get; internal set; }
        public ServerLoginMode? LoginMode { get; set; }
        public LoginCollection Logins { get; internal set; }
        public SqlMail Mail { get; internal set; }
        public string MailProfile { get; set; }
        public string MasterDBLogPath { get; internal set; }
        public string MasterDBPath { get; internal set; }
        public byte MaxPrecision { get; internal set; }
        public string Name { get; internal set; }
        public bool? NamedPipesEnabled { get; internal set; }
        public string NetName { get; internal set; }
        public int? NumberOfLogFiles { get; set; }
        public OleDbProviderSettingsCollection OleDbProviderSettings { get; internal set; }
        public string OSVersion { get; internal set; }
        public string PathSeparator { get; internal set; }
        public PerfMonMode? PerfMonMode { get; set; }
        public int? PhysicalMemory { get; internal set; }
        public long PhysicalMemoryUsageInKB { get; internal set; }
        public string Platform { get; internal set; }
        public int? Processors { get; internal set; }
        public int? ProcessorUsage { get; internal set; }
        public string Product { get; internal set; }
        public string ProductLevel { get; internal set; }
        public SqlPropertyCollection Properties { get; internal set; }
        public ServerProxyAccount ProxyAccount { get; internal set; }
        public ResourceGovernor ResourceGovernor { get; internal set; }
        public DateTime? ResourceLastUpdateDateTime { get; internal set; }
        public Version ResourceVersion { get; internal set; }
        public string ResourceVersionString { get; internal set; }
        public ServerRoleCollection Roles { get; internal set; }
        public string RootDirectory { get; internal set; }
        public ServerAuditSpecificationCollection ServerAuditSpecifications { get; internal set; }
        public DatabaseEngineType? ServerType { get; internal set; }
        public string ServiceAccount { get; internal set; }
        public string ServiceInstanceId { get; internal set; }
        public ServiceMasterKey ServiceMasterKey { get; internal set; }
        public string ServiceName { get; internal set; }
        public ServiceStartMode? ServiceStartMode { get; internal set; }
        public Settings Settings { get; internal set; }
        public SmartAdmin SmartAdmin { get; internal set; }
        public short? SqlCharSet { get; internal set; }
        public string SqlCharSetName { get; internal set; }
        public string SqlDomainGroup { get; internal set; }
        public short? SqlSortOrder { get; internal set; }
        public string SqlSortOrderName { get; internal set; }
        public SqlSmoState? State { get; internal set; }
        public ServerStatus? Status { get; internal set; }
        public AvailabilityGroupClusterType[] SupportedAvailabilityGroupClusterTypes { get; internal set; }
        public SystemDataTypeCollection SystemDataTypes { get; internal set; }
        public SystemMessageCollection SystemMessages { get; internal set; }
        public int? TapeLoadWaitTime { get; set; }
        public bool? TcpEnabled { get; internal set; }
        public ServerDdlTriggerCollection Triggers { get; internal set; }
        public Urn Urn { get; internal set; }
        public object UserData { get; internal set; }
        public UserDefinedMessageCollection UserDefinedMessages { get; internal set; }
        public UserOptions UserOptions { get; internal set; }
        public Version Version { get; internal set; }
        public int? VersionMajor { get; internal set; }
        public int? VersionMinor { get; internal set; }
        public string VersionString { get; internal set; }

        public SmoServer() { }

        private SmoServer(Server server) => this.Translate(server);

        private void Translate(Server server)
        {
            PropertyInfo[] allProps = this.GetType().GetProperties(
                (BindingFlags)52).Where(
                    x => x.CanWrite).ToArray();

            foreach (Property prop in server.Properties)
            {
                for (int i = 0; i < allProps.Length; i++)
                {
                    PropertyInfo pi = allProps[i];
                    if (prop.Name.Equals(pi.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        MethodInfo genMeth = CastMethod.MakeGenericMethod(pi.PropertyType);
                        object castedObj = genMeth.Invoke(this, new object[1] { prop.Value });
                        pi.SetValue(this, castedObj);
                    }
                }
            }
        }

        private readonly MethodInfo CastMethod = typeof(SmoServer).GetMethod("Cast", BindingFlags.NonPublic | BindingFlags.Instance);

        private T Cast<T>(dynamic o) => (T)o;

        public static explicit operator SmoServer(Server server) => new SmoServer(server);
    }
}
