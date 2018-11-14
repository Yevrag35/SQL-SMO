using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace SQL.SMO.Databases
{
    public class SMOTable : SMOPropertyLoader
    {
        private readonly Table _tbl;
        private static readonly Type _type = typeof(Table);
        internal static string[] SkipThese => new string[4]
        {
            "Name", "ID", "DataSpaceUsed", "DateLastModified"
        };


        #region All Properties
        public SMODatabase Parent { get; internal set; }
        public bool? AnsiNullsStatus { get; internal set; }
        public bool? ChangeTrackingEnabled { get; internal set; }
        public DateTime? CreateDate { get; internal set; }
        public string DataSourceName { get; internal set; }
        public DurabilityType? Durability { get; internal set; }
        public ExternalTableDistributionType? ExternalTableDistribution { get; internal set; }
        public bool? FakeSystemTable { get; internal set; }
        public string FileFormatName { get; internal set; }
        public string FileGroup { get; internal set; }
        public string FileStreamFileGroup { get; internal set; }
        public string FileStreamPartitionScheme { get; internal set; }
        public string FileTableDirectoryName { get; internal set; }
        public string FileTableNameColumnCollation { get; internal set; }
        public bool? FileTableNamespaceEnabled { get; internal set; }
        public bool? HasAfterTrigger { get; internal set; }
        public bool? HasClusteredColumnStoreIndex { get; internal set; }
        public bool? HasClusteredIndex { get; internal set; }
        public bool? HasCompressedPartitions { get; internal set; }
        public bool? HasDeleteTrigger { get; internal set; }
        public bool? HasHeapIndex { get; internal set; }
        public bool? HasIndex { get; internal set; }
        public bool? HasInsertTrigger { get; internal set; }
        public bool? HasInsteadOfTrigger { get; internal set; }
        public bool? HasNonClusteredColumnStoreIndex { get; internal set; }
        public bool? HasNonClusteredIndex { get; internal set; }
        public bool? HasPrimaryClusteredIndex { get; internal set; }
        public bool? HasSparseColumn { get; internal set; }
        public bool? HasSpatialData { get; internal set; }
        public bool? HasSystemTimePeriod { get; internal set; }
        public bool? HasUpdateTrigger { get; internal set; }
        public bool? HasXmlData { get; internal set; }
        public bool? HasXmlIndex { get; internal set; }
        public int? HistoryTableID { get; internal set; }
        public string HistoryTableName { get; internal set; }
        public string HistoryTableSchema { get; internal set; }
        public double? IndexSpaceUsed { get; internal set; }
        public bool? IsExternal { get; internal set; }
        public bool? IsFileTable { get; internal set; }
        public bool? IsIndexable { get; internal set; }
        public bool? IsMemoryOptimized { get; internal set; }
        public bool? IsPartitioned { get; internal set; }
        public bool? IsSchemaOwned { get; internal set; }
        public bool? IsSystemObject { get; internal set; }
        public bool? IsSystemVersioned { get; internal set; }
        public string Location { get; internal set; }
        public LockEscalationType? LockEscalation { get; internal set; }
        public string Owner { get; internal set; }
        public string PartitionScheme { get; internal set; }
        public bool? QuotedIdentifierStatus { get; internal set; }
        public double? RejectSampleValue { get; internal set; }
        public ExternalTableRejectType? RejectType { get; internal set; }
        public double? RejectValue { get; internal set; }
        public RemoteDataArchiveMigrationState? RemoteDataArchiveDataMigrationState { get; internal set; }
        public bool? RemoteDataArchiveEnabled { get; internal set; }
        public string RemoteDataArchiveFilterPredicate { get; internal set; }
        public string RemoteObjectName { get; internal set; }
        public string RemoteSchemaName { get; internal set; }
        public string RemoteTableName { get; internal set; }
        public bool? RemoteTableProvisioned { get; internal set; }
        public bool? Replicated { get; internal set; }
        public string ShardingColumnName { get; internal set; }
        public string SystemTimePeriodEndColumn { get; internal set; }
        public string SystemTimePeriodStartColumn { get; internal set; }
        public TableTemporalType? TemporalType { get; internal set; }
        public string TextFileGroup { get; internal set; }
        public bool? TrackColumnsUpdatedEnabled { get; internal set; }
        public int? HistoryRetentionPeriod { get; internal set; }
        public TemporalHistoryRetentionPeriodUnit? HistoryRetentionPeriodUnit { get; internal set; }
        public DwTableDistributionType? DwTableDistribution { get; internal set; }
        public string RejectedRowLocation { get; internal set; }
        public TableEvents Events { get; internal set; }
        public IndexCollection Indexes { get; internal set; }
        public CheckCollection Checks { get; internal set; }
        //public ResumableIndexCollection ResumableIndexes { get; internal set; }
        public bool? OnlineHeapOperation { get; internal set; }
        public int? LowPriorityMaxDuration { get; internal set; }
        public bool? DataConsistencyCheck { get; internal set; }
        public AbortAfterWait? LowPriorityAbortAfterWait { get; internal set; }
        public int? MaximumDegreeOfParallelism { get; internal set; }
        public bool? IsNode { get; internal set; }
        public bool? IsEdge { get; internal set; }
        public ForeignKeyCollection ForeignKeys { get; internal set; }
        public PhysicalPartitionCollection PhysicalPartitions { get; internal set; }
        public PartitionSchemeParameterCollection PartitionSchemeParameters { get; internal set; }
        public double? RowCountAsDouble { get; internal set; }
        public bool? IsVarDecimalStorageFormatEnabled { get; internal set; }
        public TriggerCollection Triggers { get; internal set; }
        public StatisticCollection Statistics { get; internal set; }
        public FullTextIndex FullTextIndex { get; internal set; }
        public string Schema { get; internal set; }
        public ExtendedPropertyCollection ExtendedProperties { get; internal set; }
        public SMOColumnCollection Columns { get; internal set; }
        public Urn Urn { get; internal set; }
        public SqlPropertyCollection Properties { get; internal set; }
        public DatabaseEngineType? DatabaseEngineType { get; internal set; }
        public DatabaseEngineEdition? DatabaseEngineEdition { get; internal set; }
        public ExecutionManager ExecutionManager { get; internal set; }
        public object UserData { get; internal set; }
        public SqlSmoState? State { get; internal set; }

        #endregion

        #region Default Properties

        public override string Name { get; internal set; }
        public string Database => _tbl.Parent.Name;
        public int? ID { get; internal set; }
        public long? RowCount { get; internal set; }
        public double? DataSpaceUsed { get; internal set; }
        public DateTime? DateLastModified { get; internal set; }
        public override Type OriginalType => _type;

        #endregion

        internal SMOTable(Table t)
        {
            Name = t.Name;
            ID = t.ID;
            DataSpaceUsed = t.DataSpaceUsed;
            DateLastModified = t.DateLastModified;
            RowCount = t.RowCount;
            _tbl = t;
        }
        
        //internal SMOColumn[] GetColumns(string[] colNames)
        //{
        //    var smoc = new SMOColumn[colNames.Length];
        //    IEnumerable<Column> cols = _tbl.Columns.OfType<Column>();
        //    for (int i = 0; i < colNames.Length; i ++)
        //    {
        //        string s = colNames[i];
        //        Column c = cols.Single(x => x.Name == s);
        //        if (c == null)
        //        {
        //            throw new SmoException("Column '" + s + "' was not found!");
        //        }
        //        smoc[i] = new SMOColumn(c);
        //    }
        //    return smoc;
        //}

        //internal protected void GetColumnNames()
        //{
        //    _cols = new string[_tbl.Columns.Count];
        //    for (int i = 0; i < _tbl.Columns.Count; i++)
        //    {
        //        _cols[i] = _tbl.Columns[i].Name;
        //    }
        //}

        //public SMODatabase GetParentDatabase() => (SMODatabase)(_tbl.Parent as Database);

        public override object ShowOriginal() => _tbl;
        public override void Load(params string[] propertyNames)
        {
            if (propertyNames == null)
                return;

            LoadValue(_tbl, propertyNames);
        }

        public static implicit operator Table(SMOTable smot) => smot.ShowOriginal() as Table;
        public static explicit operator SMOTable(Table tab) => new SMOTable(tab);
    }
}
