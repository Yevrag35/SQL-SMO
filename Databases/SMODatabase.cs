using MG.Attributes;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Broker;
using SQL.SMO.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace SQL.SMO.Databases
{
    public class SMODatabase : SMOPropertyLoader
    {
        private Database _db;
        private readonly CompatTable _compat;
        private static readonly Type _type = typeof(Database);
        internal static readonly string[] SkipThese = new string[9]
        {
            "Name", "AutoShrink", "Collation", "CompatibilityLevel",
            "State", "Status", "RecoveryModel", "LogReuseWaitStats", "Parent"
        };

        #region All Properties
        public int? ActiveConnections { get; internal set; }
        public bool? AnsiNullDefault { get; internal set; }
        public bool? AnsiNullsEnabled { get; internal set; }
        public bool? AnsiPaddingEnabled { get; internal set; }
        public bool? AnsiWarningsEnabled { get; internal set; }
        public bool? ArithmeticAbortEnabled { get; internal set; }
        public bool? AutoClose { get; internal set; }
        public bool? AutoCreateIncrementalStatisticsEnabled { get; internal set; }
        public bool? AutoCreateStatisticsEnabled { get; internal set; }
        public bool? AutoUpdateStatisticsAsync { get; internal set; }
        public bool? AutoUpdateStatisticsEnabled { get; internal set; }
        public AvailabilityDatabaseSynchronizationState? AvailabilityDatabaseSynchronizationState { get; internal set; }
        public string AvailabilityGroupName { get; internal set; }
        public bool? BrokerEnabled { get; internal set; }
        public bool? CaseSensitive { get; internal set; }
        public bool? ChangeTrackingAutoCleanUp { get; internal set; }
        public bool? ChangeTrackingEnabled { get; internal set; }
        public int? ChangeTrackingRetentionPeriod { get; internal set; }
        public RetentionPeriodUnits? ChangeTrackingRetentionPeriodUnits { get; internal set; }
        public bool? CloseCursorsOnCommitEnabled { get; internal set; }
        public bool? ConcatenateNullYieldsNull { get; internal set; }
        public ContainmentType? ContainmentType { get; internal set; }
        public DateTime? CreateDate { get; internal set; }
        public Guid? DatabaseGuid { get; internal set; }
        public string DatabaseSnapshotBaseName { get; internal set; }
        public double? DataSpaceUsage { get; internal set; }
        public bool? DateCorrelationOptimization { get; internal set; }
        public bool? DboLogin { get; internal set; }
        public string DefaultFileGroup { get; internal set; }
        public string DefaultFileStreamFileGroup { get; internal set; }
        public string DefaultFullTextCatalog { get; internal set; }
        public string DefaultSchema { get; internal set; }
        public DelayedDurability? DelayedDurability { get; internal set; }
        public bool? EncryptionEnabled { get; internal set; }
        public string FilestreamDirectoryName { get; internal set; }
        public FilestreamNonTransactedAccessType? FilestreamNonTransactedAccess { get; internal set; }
        public bool? HasFileInCloud { get; internal set; }
        public bool? HasMemoryOptimizedObjects { get; internal set; }
        public bool? HonorBrokerPriority { get; internal set; }
        public int? ID { get; internal set; }
        public double? IndexSpaceUsage { get; internal set; }
        public bool? IsAccessible { get; internal set; }
        public bool? IsDatabaseSnapshot { get; internal set; }
        public bool? IsDatabaseSnapshotBase { get; internal set; }
        public bool? IsDbAccessAdmin { get; internal set; }
        public bool? IsDbBackupOperator { get; internal set; }
        public bool? IsDbDatareader { get; internal set; }
        public bool? IsDbDatawriter { get; internal set; }
        public bool? IsDbDdlAdmin { get; internal set; }
        public bool? IsDbDenyDatareader { get; internal set; }
        public bool? IsDbDenyDatawriter { get; internal set; }
        public bool? IsDbOwner { get; internal set; }
        public bool? IsDbSecurityAdmin { get; internal set; }
        public bool? IsFullTextEnabled { get; internal set; }
        public bool? IsMailHost { get; internal set; }
        public bool? IsManagementDataWarehouse { get; internal set; }
        public bool? IsMirroringEnabled { get; internal set; }
        public bool? IsParameterizationForced { get; internal set; }
        public bool? IsReadCommittedSnapshotOn { get; internal set; }
        public bool? IsSqlDw { get; internal set; }
        public bool? IsSystemObject { get; internal set; }
        public bool? IsUpdateable { get; internal set; }
        public DateTime? LastBackupDate { get; internal set; }
        public DateTime? LastDifferentialBackupDate { get; internal set; }
        public DateTime? LastLogBackupDate { get; internal set; }
        public bool? LocalCursorsDefault { get; internal set; }
        public double? MemoryAllocatedToMemoryOptimizedObjectsInKB { get; internal set; }
        public double? MemoryUsedByMemoryOptimizedObjectsInKB { get; internal set; }
        public decimal? MirroringFailoverLogSequenceNumber { get; internal set; }
        public Guid? MirroringID { get; internal set; }
        public string MirroringPartner { get; internal set; }
        public string MirroringPartnerInstance { get; internal set; }
        public int? MirroringRedoQueueMaxSize { get; internal set; }
        public int? MirroringRoleSequence { get; internal set; }
        public MirroringSafetyLevel? MirroringSafetyLevel { get; internal set; }
        public int? MirroringSafetySequence { get; internal set; }
        public MirroringStatus? MirroringStatus { get; internal set; }
        public int? MirroringTimeout { get; internal set; }
        public string MirroringWitness { get; internal set; }
        public MirroringWitnessStatus? MirroringWitnessStatus { get; internal set; }
        public bool? NestedTriggersEnabled { get; internal set; }
        public bool? NumericRoundAbortEnabled { get; internal set; }
        public string Owner { get; internal set; }
        public PageVerify? PageVerify { get; internal set; }
        public string PrimaryFilePath { get; internal set; }
        public bool? QuotedIdentifiersEnabled { get; internal set; }
        public bool? ReadOnly { get; internal set; }
        public Guid? RecoveryForkGuid { get; internal set; }
        public bool? RecursiveTriggersEnabled { get; internal set; }
        public string RemoteDataArchiveCredential { get; internal set; }
        public bool? RemoteDataArchiveEnabled { get; internal set; }
        public string RemoteDataArchiveEndpoint { get; internal set; }
        public string RemoteDataArchiveLinkedServer { get; internal set; }
        public bool? RemoteDataArchiveUseFederatedServiceAccount { get; internal set; }
        public string RemoteDatabaseName { get; internal set; }
        public ReplicationOptions? ReplicationOptions { get; internal set; }
        public Guid? ServiceBrokerGuid { get; internal set; }
        public double? Size { get; internal set; }
        public SnapshotIsolationState? SnapshotIsolationState { get; internal set; }
        public double? SpaceAvailable { get; internal set; }
        public int? TargetRecoveryTime { get; internal set; }
        public bool? TransformNoiseWords { get; internal set; }
        public bool? Trustworthy { get; internal set; }
        public int? TwoDigitYearCutoff { get; internal set; }
        public DatabaseUserAccess? UserAccess { get; internal set; }
        public string UserName { get; internal set; }
        public int? Version { get; internal set; }
        public string AzureEdition { get; internal set; }
        public string AzureServiceObjective { get; internal set; }
        public bool? IsDbManager { get; internal set; }
        public bool? IsLoginManager { get; internal set; }
        public bool? IsSqlDwEdition { get; internal set; }
        public double? MaxSizeInBytes { get; internal set; }
        public bool? TemporalHistoryRetentionEnabled { get; internal set; }
        public DatabaseEvents Events { get; internal set; }
        public ExecutionManager ExecutionManager { get; internal set; }
        public DatabaseEngineType? DatabaseEngineType { get; internal set; }
        public DatabaseEngineEdition? DatabaseEngineEdition { get; internal set; }
        public bool? WarnOnRename { get; internal set; }
        public bool? DatabaseOwnershipChaining { get; internal set; }
        public CatalogCollationType? CatalogCollation { get; internal set; }
        public ExtendedPropertyCollection ExtendedProperties { get; internal set; }
        public DatabaseOptions DatabaseOptions { get; internal set; }
        public QueryStoreOptions QueryStoreOptions { get; internal set; }
        public SynonymCollection Synonyms { get; internal set; }
        public SequenceCollection Sequences { get; internal set; }
        public SMOTableCollection Tables { get; internal set; }
        public DatabaseScopedCredentialCollection DatabaseScopedCredentials { get; internal set; }
        public StoredProcedureCollection StoredProcedures { get; internal set; }
        public SqlAssemblyCollection Assemblies { get; internal set; }
        public ExternalLibraryCollection ExternalLibraries { get; internal set; }
        public UserDefinedTypeCollection UserDefinedTypes { get; internal set; }
        public UserDefinedAggregateCollection UserDefinedAggregates { get; internal set; }
        public FullTextCatalogCollection FullTextCatalogs { get; internal set; }
        public FullTextStopListCollection FullTextStopLists { get; internal set; }
        public SearchPropertyListCollection SearchPropertyLists { get; internal set; }
        public SecurityPolicyCollection SecurityPolicies { get; internal set; }
        public DatabaseScopedConfigurationCollection DatabaseScopedConfigurations { get; internal set; }
        public ExternalDataSourceCollection ExternalDataSources { get; internal set; }
        public ExternalFileFormatCollection ExternalFileFormats { get; internal set; }
        public CertificateCollection Certificates { get; internal set; }
        public ColumnMasterKeyCollection ColumnMasterKeys { get; internal set; }
        public ColumnEncryptionKeyCollection ColumnEncryptionKeys { get; internal set; }
        public SymmetricKeyCollection SymmetricKeys { get; internal set; }
        public AsymmetricKeyCollection AsymmetricKeys { get; internal set; }
        public DatabaseEncryptionKey DatabaseEncryptionKey { get; internal set; }
        public ExtendedStoredProcedureCollection ExtendedStoredProcedures { get; internal set; }
        public UserDefinedFunctionCollection UserDefinedFunctions { get; internal set; }
        public ViewCollection Views { get; internal set; }
        public UserCollection Users { get; internal set; }
        public DatabaseAuditSpecificationCollection DatabaseAuditSpecifications { get; internal set; }
        public SchemaCollection Schemas { get; internal set; }
        public DatabaseRoleCollection Roles { get; internal set; }
        public ApplicationRoleCollection ApplicationRoles { get; internal set; }
        public LogFileCollection LogFiles { get; internal set; }
        public FileGroupCollection FileGroups { get; internal set; }
        public PlanGuideCollection PlanGuides { get; internal set; }
        public DefaultCollection Defaults { get; internal set; }
        public RuleCollection Rules { get; internal set; }
        public UserDefinedDataTypeCollection UserDefinedDataTypes { get; internal set; }
        public UserDefinedTableTypeCollection UserDefinedTableTypes { get; internal set; }
        public XmlSchemaCollectionCollection XmlSchemaCollections { get; internal set; }
        public PartitionFunctionCollection PartitionFunctions { get; internal set; }
        public PartitionSchemeCollection PartitionSchemes { get; internal set; }
        public DatabaseActiveDirectory ActiveDirectory { get; internal set; }
        public MasterKey MasterKey { get; internal set; }
        public DatabaseDdlTriggerCollection Triggers { get; internal set; }
        public DefaultLanguage DefaultLanguage { get; internal set; }
        public DefaultLanguage DefaultFullTextLanguage { get; internal set; }
        public ServiceBroker ServiceBroker { get; internal set; }
        public int? MaxDop { get; internal set; }
        public int? MaxDopForSecondary { get; internal set; }
        public DatabaseScopedConfigurationOnOff? LegacyCardinalityEstimation { get; internal set; }
        public DatabaseScopedConfigurationOnOff? LegacyCardinalityEstimationForSecondary { get; internal set; }
        public DatabaseScopedConfigurationOnOff? ParameterSniffing { get; internal set; }
        public DatabaseScopedConfigurationOnOff? ParameterSniffingForSecondary { get; internal set; }
        public DatabaseScopedConfigurationOnOff? QueryOptimizerHotfixes { get; internal set; }
        public DatabaseScopedConfigurationOnOff? QueryOptimizerHotfixesForSecondary { get; internal set; }
        public bool? IsVarDecimalStorageFormatEnabled { get; internal set; }
        public Urn Urn { get; internal set; }
        public SqlPropertyCollection Properties { get; internal set; }
        public object UserData { get; internal set; }

        #endregion

        #region Default Properties
        public override string Name { get; internal set; }
        public bool AutoShrink => _db.AutoShrink;
        public string Collation => _db.Collation;
        public string Compatibility => GetNameAttribute(_compat);
        public SqlSmoState State => _db.State;
        public DatabaseStatus Status => _db.Status;
        public RecoveryModel RecoveryModel => _db.RecoveryModel;
        public LogReuseWaitStatus LogReuseWaitStatus => _db.LogReuseWaitStatus;
        public override Type OriginalType => _type;

        #endregion

        private protected SMODatabase(Database db)
        {
            _db = db;
            _compat = GetEnumFromValue<CompatTable>(_db.CompatibilityLevel, typeof(CompatAttribute));
        }

        public override object ShowOriginal() => _db;

        public override void Load(params string[] propertyNames)
        {
            if (propertyNames == null)
                return;

            LoadValue(_db, propertyNames);
        }

        public static implicit operator Database(SMODatabase smod) => smod.ShowOriginal() as Database;
        public static explicit operator SMODatabase(Database db) => new SMODatabase(db);
    }
}
