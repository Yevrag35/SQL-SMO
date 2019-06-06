using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.SqlServer.Management.Smo.Mail;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;

namespace MG.Sql.Smo
{
    public class SmoServer : IAlienObject, IAlienRoot, IAlterable, IDmfFacet, IRefreshable, IScriptable,
        IServerSettings, ISfcDomainLite, ISfcHasConnection, ISfcSupportsDesignMode, ISfcValidate
    {
        #region FIELDS AND CONSTANTS
        private Server _sql;
        private const BindingFlags PUB_INST = BindingFlags.Public | BindingFlags.Instance;
        internal static readonly PropertyInfo[] thisProps = typeof(SmoServer).GetProperties(PUB_INST);
        private static readonly PropertyInfo[] origProps = typeof(Server).GetProperties(PUB_INST);

        #endregion

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
        SfcConnectionContext ISfcHasConnection.ConnectionContext => ((ISfcHasConnection)_sql).ConnectionContext;
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
        string ISfcDomainLite.DomainName => ((ISfcDomainLite)_sql).DomainName;
        string ISfcDomainLite.DomainInstanceName => ((ISfcDomainLite)_sql).DomainInstanceName;
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
        bool ISfcSupportsDesignMode.IsDesignMode => ((ISfcSupportsDesignMode)_sql).IsDesignMode;
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
        public SQLYearVersion YearVersion => _sql.Version.Major != 10 && _sql.Version.Minor != 50
            ? (SQLYearVersion)_sql.Version.Major
            : SQLYearVersion.SQLServer2008R2;

        //public int? VersionMajor { get; private set; }
        //public int? VersionMinor { get; private set; }
        //public string VersionString { get; private set; }

        #endregion

        #region CONSTRUCTORS

        public SmoServer() { }

        public SmoServer(Server server) => _sql = server;

        #endregion

        #region REFLECTIVE METHODS

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

        #endregion

        #region CASTS

        public static explicit operator SmoServer(Server server) => new SmoServer(server);

        public static implicit operator Server(SmoServer smoServer) => smoServer._sql;
        public static implicit operator SqlSmoObject(SmoServer smoServer) => smoServer._sql;

        #endregion

        #region SERVER METHODS
        public void Alter() => _sql.Alter();
        public void Alter(bool overrideValueChecking) => _sql.Alter(overrideValueChecking);
        public void AttachDatabase(string name, StringCollection files) => _sql.AttachDatabase(name, files);
        public void AttachDatabase(string name, StringCollection files, string owner) => _sql.AttachDatabase(name, files, owner);
        public void AttachDatabase(string name, StringCollection files, AttachOptions options) => _sql.AttachDatabase(name, files, options);
        public void AttachDatabase(string name, StringCollection files, string owner, AttachOptions options) => _sql.AttachDatabase(name, files, owner, options);
        public int CompareUrn(Urn urn1, Urn urn2) => _sql.CompareUrn(urn1, urn2);
        public void DeleteBackupHistory(DateTime oldestDate) => _sql.DeleteBackupHistory(oldestDate);
        public void DeleteBackupHistory(int mediaSetId) => _sql.DeleteBackupHistory(mediaSetId);
        public void DeleteBackupHistory(string database) => _sql.DeleteBackupHistory(database);
        public void Deny(ServerPermissionSet permission, string granteeName) => _sql.Deny(permission, granteeName);
        public void Deny(ServerPermissionSet permission, string granteeName, bool cascade) => _sql.Deny(permission, granteeName, cascade);
        public void Deny(ServerPermissionSet permission, string[] granteeNames) => _sql.Deny(permission, granteeNames);
        public void Deny(ServerPermissionSet permission, string[] granteeNames, bool cascade) => _sql.Deny(permission, granteeNames, cascade);
        void IAlienRoot.DesignModeInitialize() => ((IAlienRoot)_sql).DesignModeInitialize();
        public void DetachDatabase(string dbName, bool updateStats) => _sql.DetachDatabase(dbName, updateStats);
        public void DetachDatabase(string dbName, bool updateStats, bool removeIndexFile) => _sql.DetachDatabase(dbName, updateStats, removeIndexFile);
        public DataTable DetachedDatabaseInfo(string mdfName) => _sql.DetachedDatabaseInfo(mdfName);
        public List<object> Discover() => _sql.Discover();
        public DataTable EnumActiveCurrentSessionTraceFlags() => _sql.EnumActiveCurrentSessionTraceFlags();
        public DataTable EnumActiveGlobalTraceFlags() => _sql.EnumActiveGlobalTraceFlags();
        public DataTable EnumAvailableMedia() => _sql.EnumAvailableMedia();
        public DataTable EnumAvailableMedia(MediaTypes media) => _sql.EnumAvailableMedia(media);
        public DataTable EnumClusterMembersState() => _sql.EnumClusterMembersState();
        public DataTable EnumClusterSubnets() => _sql.EnumClusterSubnets();
        public DataTable EnumCollations() => _sql.EnumCollations();
        public DataTable EnumDatabaseMirrorWitnessRoles() => _sql.EnumDatabaseMirrorWitnessRoles();
        public DataTable EnumDatabaseMirrorWitnessRoles(string db) => _sql.EnumDatabaseMirrorWitnessRoles(db);
        public StringCollection EnumDetachedDatabaseFiles(string mdfName) => _sql.EnumDetachedDatabaseFiles(mdfName);
        public StringCollection EnumDetachedLogFiles(string mdfName) => _sql.EnumDetachedLogFiles(mdfName);
        public DataTable EnumDirectories(string path) => _sql.EnumDirectories(path);
        public DataTable EnumErrorLogs() => _sql.EnumErrorLogs();
        public DataTable EnumLocks() => _sql.EnumLocks();
        public DataTable EnumLocks(int processId) => _sql.EnumLocks(processId);
        public StringCollection EnumMembers(RoleTypes roleType) => _sql.EnumMembers(roleType);
        public ObjectPermissionInfo[] EnumObjectPermissions() => _sql.EnumObjectPermissions();
        public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName) => _sql.EnumObjectPermissions(granteeName);
        public ObjectPermissionInfo[] EnumObjectPermissions(ObjectPermissionSet perms) => _sql.EnumObjectPermissions(perms);
        public ObjectPermissionInfo[] EnumObjectPermission(string granteeName, ObjectPermissionSet perms) => _sql.EnumObjectPermissions(granteeName, perms);
        public DataTable EnumPerformanceCounters() => _sql.EnumPerformanceCounters();
        public DataTable EnumPerformanceCounters(string objName) => _sql.EnumPerformanceCounters(objName);
        public DataTable EnumPerformanceCounters(string objName, string counterName) => _sql.EnumPerformanceCounters(objName, counterName);
        public DataTable EnumPerformanceCounters(string objName, string counterName, string instName) => _sql.EnumPerformanceCounters(objName, counterName, instName);
        public DataTable EnumProcesses() => _sql.EnumProcesses();
        public DataTable EnumProcesses(int processId) => _sql.EnumProcesses(processId);
        public DataTable EnumProcesses(bool excludeSysProcs) => _sql.EnumProcesses(excludeSysProcs);
        public DataTable EnumProcesses(string loginName) => _sql.EnumProcesses(loginName);
        public DataTable EnumServerAttributes() => _sql.EnumServerAttributes();
        public ServerPermissionInfo[] EnumServerPermissions() => _sql.EnumServerPermissions();
        public ServerPermissionInfo[] EnumServerPermissions(string granteeName) => _sql.EnumServerPermissions(granteeName);
        public ServerPermissionInfo[] EnumServerPermissions(ServerPermissionSet perms) => _sql.EnumServerPermissions(perms);
        public ServerPermissionInfo[] EnumServerPermissions(string granteeName, ServerPermissionSet perms) => _sql.EnumServerPermissions(granteeName, perms);
        public DataTable EnumStartupProcedures() => _sql.EnumStartupProcedures();
        public DataTable EnumWindowsDomainGroups() => _sql.EnumWindowsDomainGroups();
        public DataTable EnumWindowsDomainGroups(string domain) => _sql.EnumWindowsDomainGroups(domain);
        public DataTable EnumWindowsGroupInfo() => _sql.EnumWindowsGroupInfo();
        public DataTable EnumWindowsGroupInfo(string group) => _sql.EnumWindowsGroupInfo(group);
        public DataTable EnumWindowsGroupInfo(string group, bool listMembers) => _sql.EnumWindowsGroupInfo(group, listMembers);
        public DataTable EnumWindowsUserInfo() => _sql.EnumWindowsUserInfo();
        public DataTable EnumWindowsUserInfo(string account) => _sql.EnumWindowsUserInfo(account);
        public DataTable EnumWindowsUserInfo(string account, bool listPerms) => _sql.EnumWindowsUserInfo(account, listPerms);
        public void ExecuteWithModes(SqlExecutionModes modes, Action action) => _sql.ExecuteWithModes(modes, action);
        public bool FileExists(string filePath) => _sql.FileExists(filePath);
        public int GetActiveDBConnectionCount(string dbName) => _sql.GetActiveDBConnectionCount(dbName);
        public ISfcConnection GetConnection() => ((ISfcHasConnection)_sql).GetConnection();
        ISfcConnection ISfcHasConnection.GetConnection(SfcObjectQueryMode activeQueriesMode) => ((ISfcHasConnection)_sql).GetConnection(activeQueriesMode);
        public StringCollection GetDefaultInitFields(Type objectType) => _sql.GetDefaultInitFields(objectType);
        public StringCollection GetDefaultInitFields(Type oType, DatabaseEngineEdition edition) => _sql.GetDefaultInitFields(oType, edition);
        ISfcDomainLite IAlienObject.GetDomainRoot() => ((IAlienObject)_sql).GetDomainRoot();
        int ISfcDomainLite.GetLogicalVersion() => ((ISfcDomainLite)_sql).GetLogicalVersion();
        object IAlienObject.GetParent() => ((IAlienObject)_sql).GetParent();
        public StringCollection GetPropertyNames(Type objType, DatabaseEngineEdition edition) => _sql.GetPropertyNames(objType, edition);
        Type IAlienObject.GetPropertyType(string propertyName) => ((IAlienObject)_sql).GetPropertyType(propertyName);
        object IAlienObject.GetPropertyValue(string propertyName, Type propertyType) => ((IAlienObject)_sql).GetPropertyValue(propertyName, propertyType);
        Urn IAlienObject.GetUrn() => ((IAlienObject)_sql).GetUrn();
        public void Grant(ServerPermissionSet perm, string[] grantees) => _sql.Grant(perm, grantees);
        public void Grant(ServerPermissionSet perm, string[] grantees, bool grantGrant) => _sql.Grant(perm, grantees, grantGrant);
        public void Grant(ServerPermissionSet perm, string[] grantees, bool grantGrant, string asRole) => _sql.Grant(perm, grantees, grantGrant, asRole);
        public void Grant(ServerPermissionSet perm, string grantee) => _sql.Grant(perm, grantee);
        public void Grant(ServerPermissionSet perm, string grantee, bool grantGrant) => _sql.Grant(perm, grantee, grantGrant);
        public void Grant(ServerPermissionSet perm, string grantee, bool grantGrant, string asRole) => _sql.Grant(perm, grantee, grantGrant, asRole);
        public void GrantAvailabilityGroupCreateDatabasePrivilege(string agName) => _sql.GrantAvailabilityGroupCreateDatabasePrivilege(agName);
        public bool Initialize() => _sql.Initialize();
        public bool Initialize(bool allProps) => _sql.Initialize(allProps);
        public bool IsDetachedPrimaryFile(string mdfName) => _sql.IsDetachedPrimaryFile(mdfName);
        public bool IsExpressSku() => _sql.IsExpressSku();
        public bool IsSupportedProperty(string propertyName) => _sql.IsSupportedProperty(propertyName);
        public bool IsTraceFlagOn(int traceFlag, bool isGlobalTraceFlag) => _sql.IsTraceFlagOn(traceFlag, isGlobalTraceFlag);
        public bool IsWindowsGroupMember(string group, string user) => _sql.IsWindowsGroupMember(group, user);
        public void JoinAvailabilityGroup(string agName) => _sql.JoinAvailabilityGroup(agName);
        public void JoinAvailabilityGroup(string agName, AvailabilityGroupClusterType type) => _sql.JoinAvailabilityGroup(agName, type);
        public void KillAllProcesses(string dbName) => _sql.KillAllProcesses(dbName);
        public void KillDatabase(string db) => _sql.KillDatabase(db);
        public void KillProcess(int processId) => _sql.KillProcess(processId);
        public bool ParentDirectoryExists(string path) => _sql.ParentDirectoryExists(path);
        public ServerVersion PingSqlServerVersion(string name) => _sql.PingSqlServerVersion(name);
        public ServerVersion PingSqlServerVersion(string name, string login, string pass) => _sql.PingSqlServerVersion(name, login, pass);
        public DataTable ReadErrorLog() => _sql.ReadErrorLog();
        public DataTable ReadErrorLog(int logNum) => _sql.ReadErrorLog(logNum);
        public void Refresh() => _sql.Refresh();
        object IAlienObject.Resolve(string urnString) => ((IAlienObject)_sql).Resolve(urnString);
        public void Revoke(ServerPermissionSet perm, string[] names) => _sql.Revoke(perm, names);
        public void RevokeAvailabilityGroupCreateDatabasePrivilege(string agName) => _sql.RevokeAvailabilityGroupCreateDatabasePrivilege(agName);
        public StringCollection Script() => _sql.Script();
        public StringCollection Script(ScriptingOptions scriptingOptions) => _sql.Script(scriptingOptions);
        public void SetAccessToken(IRenewableToken token) => _sql.SetAccessToken(token);
        void ISfcHasConnection.SetConnection(ISfcConnection connection) => ((ISfcHasConnection)_sql).SetConnection(connection);
        public void SetDefaultInitFields(Type objType, StringCollection fields) => _sql.SetDefaultInitFields(objType, fields);
        public void SetDefaultInitFields(Type objType, StringCollection fields, DatabaseEngineEdition edition) => _sql.SetDefaultInitFields(objType, fields, edition);
        public void SetDefaultInitFields(Type objType, params string[] fields) => _sql.SetDefaultInitFields(objType, fields);
        public void SetDefaultInitFields(Type objType, DatabaseEngineEdition edition, params string[] fields) => _sql.SetDefaultInitFields(objType, edition, fields);
        public void SetDefaultInitFields(Type objType, bool allFields, DatabaseEngineEdition edition) => _sql.SetDefaultInitFields(objType, allFields, edition);
        public void SetDefaultInitFields(bool allFields) => _sql.SetDefaultInitFields(allFields);
        void IAlienObject.SetObjectState(SfcObjectState state) => ((IAlienObject)_sql).SetObjectState(state);
        void IAlienObject.SetPropertyValue(string propertyName, Type propertyType, object value) => ((IAlienObject)_sql).SetPropertyValue(propertyName, propertyType, value);
        public void SetTraceFlag(int num, bool isOn) => _sql.SetTraceFlag(num, isOn);
        DataTable IAlienRoot.SfcHelper_GetDataTable(object connection, string urn, string[] fields, OrderBy[] orderByFields) => ((IAlienRoot)_sql).SfcHelper_GetDataTable(connection, urn, fields, orderByFields);
        object IAlienRoot.SfcHelper_GetSmoObject(string urn) => ((IAlienRoot)_sql).SfcHelper_GetSmoObject(urn);
        List<string> IAlienRoot.SfcHelper_GetSmoObjectQuery(string queryString, string[] fields, OrderBy[] orderByFields) => ((IAlienRoot)_sql).SfcHelper_GetSmoObjectQuery(queryString, fields, orderByFields);
        public void Touch() => _sql.Touch();
        public ValidationState Validate(string methodName, params object[] arguments) => _sql.Validate(methodName, arguments);

        #endregion
    }
}
