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
        private Server _sql;
        private const BindingFlags PUB_INST = BindingFlags.Public | BindingFlags.Instance;
        private static readonly PropertyInfo[] thisProps = typeof(SmoServer).GetProperties(PUB_INST);
        private static readonly PropertyInfo[] origProps = typeof(Server).GetProperties(PUB_INST);

        #region PUBLIC PROPERTIES
        [Obsolete]
        public ServerActiveDirectory ActiveDirectory { get; private set; }
        public AffinityInfo AffinityInfo { get; private set; }
        public AuditLevel AuditLevel
        {
            get => _sql.AuditLevel;
            set => _sql.AuditLevel = value;
        }
        public AuditCollection Audits { get; private set; }
        public AvailabilityGroupCollection AvailabilityGroups { get; private set; }
        public BackupDeviceCollection BackupDevices { get; private set; }
        public string BackupDirectory
        {
            get => _sql.BackupDirectory;
            set => _sql.BackupDirectory = value;
        }
        public string BrowserServiceAccount => _sql.BrowserServiceAccount;
        public ServiceStartMode BrowserStartMode => _sql.BrowserStartMode;
        public Version BuildClrVersion => _sql.BuildClrVersion;
        //public string BuildClrVersionString { get; private set; }
        public int BuildNumber => _sql.BuildNumber;
        public string ClusterName => _sql.ClusterName;
        public ClusterQuorumState ClusterQuorumState => _sql.ClusterQuorumState;
        public ClusterQuorumType ClusterQuorumType => _sql.ClusterQuorumType;
        public string Collation => _sql.Collation;
        public int CollationID => _sql.CollationID;
        public int ComparisonStyle => _sql.ComparisonStyle;
        public string ComputerNamePhysicalNetBIOS => _sql.ComputerNamePhysicalNetBIOS;
        public Configuration Configuration { get; private set; }
        public ServerConnection ConnectionContext { get; private set; }
        public CredentialCollection Credentials { get; private set; }
        public CryptographicProviderCollection CryptographicProviders { get; private set; }
        public DatabaseEngineEdition DatabaseEngineEdition => _sql.DatabaseEngineEdition;
        public DatabaseEngineType DatabaseEngineType => _sql.DatabaseEngineType;
        public DatabaseCollection Databases { get; private set; }
        public AvailabilityGroupClusterType DefaultAvailabilityGroupClusterType => _sql.DefaultAvailabilityGroupClusterType;
        public string DefaultFile
        {
            get => _sql.DefaultFile;
            set => _sql.DefaultFile = value;
        }
        public string DefaultLog
        {
            get => _sql.DefaultLog;
            set => _sql.DefaultLog = value;
        }
        public bool DefaultTextMode
        {
            get => _sql.DefaultTextMode;
            set => _sql.DefaultTextMode = value;
        }
        public string Edition => _sql.Edition;
        public EndpointCollection Endpoints { get; private set; }
        public Edition EngineEdition => _sql.EngineEdition;
        public string ErrorLogPath => _sql.ErrorLogPath;
        public ServerEvents Events { get; private set; }
        public ExecutionManager ExecutionManager { get; private set; }
        public FileStreamEffectiveLevel FilestreamLevel => _sql.FilestreamLevel;
        public string FilestreamShareName => _sql.FilestreamShareName;
        public FullTextService FullTextService { get; private set; }
        public HadrManagerStatus HadrManagerStatus => _sql.HadrManagerStatus;
        public string HostDistribution { get; private set; }
        public string HostPlatform => _sql.HostPlatform;
        public string HostRelease { get; private set; }
        public string HostServicePackLevel { get; private set; }
        public int? HostSku { get; private set; }
        public Information Information { get; private set; }
        public string InstallDataDirectory => _sql.InstallDataDirectory;
        public string InstallSharedDirectory => _sql.InstallSharedDirectory;
        public string InstanceName => _sql.InstanceName;
        public bool IsAvailabilityReplicaSeedingModeSupported => _sql.IsAvailabilityReplicaSeedingModeSupported;
        public bool IsCaseSensitive => _sql.IsCaseSensitive;
        public bool IsClustered => _sql.IsClustered;
        public bool IsConfigurationOnlyAvailabilityReplicaSupported => _sql.IsConfigurationOnlyAvailabilityReplicaSupported;
        public bool IsCrossPlatformAvailabilityGroupSupported => _sql.IsCrossPlatformAvailabilityGroupSupported;
        public bool IsFullTextInstalled => _sql.IsFullTextInstalled;
        public bool IsHadrEnabled => _sql.IsHadrEnabled;
        public bool IsMemberOfWsfcCluster => _sql.IsMemberOfWsfcCluster;
        public bool IsPolyBaseInstalled => _sql.IsPolyBaseInstalled;
        public bool IsReadOnlyListWithLoadBalancingSupported => _sql.IsReadOnlyListWithLoadBalancingSupported;
        public bool IsSingleUser => _sql.IsSingleUser;
        public bool IsXTPSupported => _sql.IsXTPSupported;
        public JobServer JobServer { get; private set; }
        public string Language => _sql.Language;
        public LanguageCollection Languages { get; private set; }
        public LinkedServerCollection LinkedServers { get; private set; }
        public ServerLoginMode LoginMode
        {
            get => _sql.LoginMode;
            set => _sql.LoginMode = value;
        }
        public LoginCollection Logins { get; private set; }
        public SqlMail Mail { get; private set; }
        public string MailProfile
        {
            get => _sql.MailProfile;
            set => _sql.MailProfile = value;
        }
        public string MasterDBLogPath => _sql.MasterDBLogPath;
        public string MasterDBPath => _sql.MasterDBPath;
        public byte MaxPrecision => _sql.MaxPrecision;
        public string Name => _sql.Name;
        public bool NamedPipesEnabled => _sql.NamedPipesEnabled;
        public string NetName => _sql.NetName;
        public int NumberOfLogFiles
        {
            get => _sql.NumberOfLogFiles;
            set => _sql.NumberOfLogFiles = value;
        }
        public OleDbProviderSettingsCollection OleDbProviderSettings { get; private set; }
        public string OSVersion => _sql.OSVersion;
        public string PathSeparator => _sql.PathSeparator;
        public PerfMonMode PerfMonMode
        {
            get => _sql.PerfMonMode;
            set => _sql.PerfMonMode = value;
        }
        public int PhysicalMemory => _sql.PhysicalMemory;
        public long PhysicalMemoryUsageInKB => _sql.PhysicalMemoryUsageInKB;
        public string Platform => _sql.Platform;
        public int Processors => _sql.Processors;
        public int ProcessorUsage => _sql.ProcessorUsage;
        public string Product => _sql.Product;
        public string ProductLevel => _sql.ProductLevel;
        public SqlPropertyCollection Properties { get; private set; }
        public ServerProxyAccount ProxyAccount { get; private set; }
        public ResourceGovernor ResourceGovernor { get; private set; }
        public DateTime ResourceLastUpdateDateTime { get; private set; }
        public Version ResourceVersion => _sql.ResourceVersion;
        //public string ResourceVersionString { get; private set; }
        public ServerRoleCollection Roles { get; private set; }
        public string RootDirectory => _sql.RootDirectory;
        public ServerAuditSpecificationCollection ServerAuditSpecifications { get; private set; }
        public DatabaseEngineType ServerType => _sql.ServerType;
        public string ServiceAccount => _sql.ServiceAccount;
        public string ServiceInstanceId => _sql.ServiceInstanceId;
        public ServiceMasterKey ServiceMasterKey { get; private set; }
        public string ServiceName => _sql.ServiceName;
        public ServiceStartMode ServiceStartMode => _sql.ServiceStartMode;
        public Settings Settings { get; private set; }
        public SmartAdmin SmartAdmin { get; private set; }
        public short SqlCharSet => _sql.SqlCharSet;
        public string SqlCharSetName => _sql.SqlCharSetName;
        public string SqlDomainGroup => _sql.SqlDomainGroup;
        public short SqlSortOrder => _sql.SqlSortOrder;
        public string SqlSortOrderName => _sql.SqlSortOrderName;
        public SqlSmoState State => _sql.State;
        public ServerStatus Status => _sql.Status;
        public AvailabilityGroupClusterType[] SupportedAvailabilityGroupClusterTypes => _sql.SupportedAvailabilityGroupClusterTypes;
        public SystemDataTypeCollection SystemDataTypes { get; private set; }
        public SystemMessageCollection SystemMessages { get; private set; }
        public int TapeLoadWaitTime
        {
            get => _sql.TapeLoadWaitTime;
            set => _sql.TapeLoadWaitTime = value;
        }
        public bool TcpEnabled => _sql.TcpEnabled;
        public ServerDdlTriggerCollection Triggers { get; private set; }
        public Urn Urn { get; private set; }
        public object UserData { get; private set; }
        public UserDefinedMessageCollection UserDefinedMessages { get; private set; }
        public UserOptions UserOptions { get; private set; }
        public Version Version => _sql.Version;
        //public int? VersionMajor { get; private set; }
        //public int? VersionMinor { get; private set; }
        //public string VersionString { get; private set; }

        #endregion

        public SmoServer() { }

        public SmoServer(Server server)
        {
            _sql = server;
            //this.AuditLevel = _sql.AuditLevel;
            //this.BackupDirectory = _sql.BackupDirectory;
            //this.BrowserServiceAccount = _sql.BrowserServiceAccount;
            //this.BrowserStartMode = _sql.BrowserStartMode;
            //this.BuildClrVersion = _sql.BuildClrVersion;
            //this.BuildNumber = _sql.BuildNumber;
            //this.ClusterName = _sql.ClusterName;
            //this.ClusterQuorumState = _sql.ClusterQuorumState;
            //this.ClusterQuorumType = _sql.ClusterQuorumType;
            //this.Collation = _sql.Collation;
            //this.CollationID = _sql.CollationID;
            //this.ComparisonStyle = _sql.ComparisonStyle;
            //this.ComputerNamePhysicalNetBIOS = _sql.ComputerNamePhysicalNetBIOS;
            //this.DatabaseEngineEdition = _sql.DatabaseEngineEdition;
            //this.DatabaseEngineType = _sql.DatabaseEngineType;
            //this.DefaultAvailabilityGroupClusterType = _sql.DefaultAvailabilityGroupClusterType;
            //this.DefaultFile = _sql.DefaultFile;
            //this.DefaultLog = _sql.DefaultLog;
            //this.DefaultTextMode = _sql.DefaultTextMode;
            //this.Edition = _sql.Edition;
            //this.EngineEdition = _sql.EngineEdition;
            //this.ErrorLogPath = _sql.ErrorLogPath;
            //this.FilestreamLevel = _sql.FilestreamLevel;
            //this.FilestreamShareName = _sql.FilestreamShareName;
            //this.HadrManagerStatus = _sql.HadrManagerStatus;
            ////this.HostDistribution = _sql.HostDistribution;
            //this.HostPlatform = _sql.HostPlatform;
            ////this.HostRelease = _sql.HostRelease;
            ////this.HostServicePackLevel = _sql.HostServicePackLevel;
            ////this.HostSku = _sql.HostSku;
            //this.InstallDataDirectory = _sql.InstallDataDirectory;
            //this.InstallSharedDirectory = _sql.InstallSharedDirectory;
            //this.InstanceName = _sql.InstanceName;
            //this.IsAvailabilityReplicaSeedingModeSupported = _sql.IsAvailabilityReplicaSeedingModeSupported;
            //this.IsCaseSensitive = _sql.IsCaseSensitive;
            //this.IsClustered = _sql.IsClustered;
            //this.IsConfigurationOnlyAvailabilityReplicaSupported = _sql.IsConfigurationOnlyAvailabilityReplicaSupported;
            //this.IsCrossPlatformAvailabilityGroupSupported = _sql.IsCrossPlatformAvailabilityGroupSupported;
            //this.IsFullTextInstalled = _sql.IsFullTextInstalled;
            //this.IsHadrEnabled = _sql.IsHadrEnabled;
            //this.IsMemberOfWsfcCluster = _sql.IsMemberOfWsfcCluster;
            //this.IsPolyBaseInstalled = _sql.IsPolyBaseInstalled;
            //this.IsReadOnlyListWithLoadBalancingSupported = _sql.IsReadOnlyListWithLoadBalancingSupported;
            //this.IsSingleUser = _sql.IsSingleUser;
            //this.IsXTPSupported = _sql.IsXTPSupported;
            //this.Language = _sql.Language;
            //this.LoginMode = _sql.LoginMode;
            //this.MailProfile = _sql.MailProfile;
            //this.MasterDBLogPath = _sql.MasterDBLogPath;
            //this.MasterDBPath = _sql.MasterDBPath;
            //this.MaxPrecision = _sql.MaxPrecision;
            //this.Name = _sql.Name;
            //this.NamedPipesEnabled = _sql.NamedPipesEnabled;
            //this.NetName = _sql.NetName;
            //this.NumberOfLogFiles = _sql.NumberOfLogFiles;
            //this.OSVersion = _sql.OSVersion;
            //this.PathSeparator = _sql.PathSeparator;
            //this.PerfMonMode = _sql.PerfMonMode;
            //this.PhysicalMemory = _sql.PhysicalMemory;
            //this.PhysicalMemoryUsageInKB = _sql.PhysicalMemoryUsageInKB;
            //this.Platform = _sql.Platform;
            //this.Processors = _sql.Processors;
            //this.ProcessorUsage = _sql.ProcessorUsage;
            //this.Product = _sql.Product;
            //this.ProductLevel = _sql.ProductLevel;
            //this.ResourceLastUpdateDateTime = _sql.ResourceLastUpdateDateTime;
            //this.ResourceVersion = _sql.ResourceVersion;
            //this.RootDirectory = _sql.RootDirectory;
            //this.ServerType = _sql.ServerType;
            //this.ServiceAccount = _sql.ServiceAccount;
            //this.ServiceInstanceId = _sql.ServiceInstanceId;
            //this.ServiceName = _sql.ServiceName;
            //this.ServiceStartMode = _sql.ServiceStartMode;
            //this.SqlCharSet = _sql.SqlCharSet;
            //this.SqlCharSetName = _sql.SqlCharSetName;
            //this.SqlDomainGroup = _sql.SqlDomainGroup;
            //this.SqlSortOrder = _sql.SqlSortOrder;
            //this.SqlSortOrderName = _sql.SqlSortOrderName;
            //this.State = _sql.State;
            //this.Status = _sql.Status;
            //this.SupportedAvailabilityGroupClusterTypes = _sql.SupportedAvailabilityGroupClusterTypes;
            //this.TapeLoadWaitTime = _sql.TapeLoadWaitTime;
            //this.TcpEnabled = _sql.TcpEnabled;
            //this.Version = _sql.Version;
        }

        public void LoadProperty(params string[] propertyNames)
        {
            foreach (PropertyInfo pi in this.MatchNames(propertyNames))
            {
                if (pi.GetValue(this) == null)
                {
                    PropertyInfo origPi = this.GetOriginalProperty(pi);
                    object origVal = origPi.GetValue(_sql);
                    pi.SetValue(this, origVal);
                }
            }
        }
        
        public void UpdatePropertyValue(params string[] propertyNames)
        {
            foreach (PropertyInfo pi in this.MatchNames(propertyNames))
            {
                PropertyInfo origPi = this.GetOriginalProperty(pi);
                object origVal = origPi.GetValue(_sql);
                pi.SetValue(this, origVal);
            }
        }
        private PropertyInfo GetOriginalProperty(PropertyInfo thisPi) => origProps.Single(x => x.Name.Equals(thisPi.Name));
        private IEnumerable<PropertyInfo> MatchNames(string[] propertyNames) => thisProps.Where(x => propertyNames.Contains(x.Name));

        private readonly MethodInfo CastMethod = typeof(SmoServer).GetMethod("Cast", BindingFlags.NonPublic | BindingFlags.Instance);

        private T Cast<T>(dynamic o) => (T)o;

        public static explicit operator SmoServer(Server server) => new SmoServer(server);
    }
}
