using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace SQL.SMO.Databases
{
    public class SMOColumn : SMOPropertyLoader
    {
        private Column _c;
        private static readonly Type _type = typeof(Column);
        internal static string[] SkipThese => new string[5]
        {
            "Name", "ID", "DataType", "IsEncrypted", "Nullable"
        };

        #region All Properties

        public bool? AnsiPaddingStatus { get; internal set; }
        public string Collation { get; internal set; }
        public int? ColumnEncryptionKeyID { get; internal set; }
        public string ColumnEncryptionKeyName { get; internal set; }
        public bool? Computed { get; internal set; }
        public string ComputedText { get; internal set; }
        public string Default { get; internal set; }
        public string DefaultSchema { get; internal set; }
        public string EncryptionAlgorithm { get; internal set; }
        public ColumnEncryptionType? EncryptionType { get; internal set; }
        public GeneratedAlwaysType? GeneratedAlwaysType { get; internal set; }
        public GraphType? GraphType { get; internal set; }
        public bool? Identity { get; internal set; }
        public long? IdentityIncrement { get; internal set; }
        public long? IdentitySeed { get; internal set; }
        public bool? InPrimaryKey { get; internal set; }
        public bool? IsColumnSet { get; internal set; }
        public bool? IsDeterministic { get; internal set; }
        public bool? IsFileStream { get; internal set; }
        public bool? IsForeignKey { get; internal set; }
        public bool? IsFullTextIndexed { get; internal set; }
        public bool? IsHidden { get; internal set; }
        public bool? IsMasked { get; internal set; }
        public bool? IsPersisted { get; internal set; }
        public bool? IsPrecise { get; internal set; }
        public bool? IsSparse { get; internal set; }
        public string MaskingFunction { get; internal set; }
        public bool? NotForReplication { get; internal set; }
        public SqlSmoObject Parent { get; internal set; }
        public bool? RowGuidCol { get; internal set; }
        public string Rule { get; internal set; }
        public string RuleSchema { get; internal set; }
        public int? StatisticalSemantics { get; internal set; }
        public string DistributionColumnName { get; internal set; }
        public bool? IsDistributedColumn { get; internal set; }
        public ExtendedPropertyCollection ExtendedProperties { get; internal set; }
        public DefaultConstraint DefaultConstraint { get; internal set; }
        public Urn Urn { get; internal set; }
        public SqlPropertyCollection Properties { get; internal set; }
        public DatabaseEngineType? DatabaseEngineType { get; internal set; }
        public DatabaseEngineEdition? DatabaseEngineEdition { get; internal set; }
        public ExecutionManager ExecutionManager { get; internal set; }
        public object UserData { get; internal set; }
        public SqlSmoState? State { get; internal set; }

        #endregion

        #region Default Properties
        public string Database { get; }
        public string Table { get; }
        public override string Name { get; internal set; }
        public long ID => _c.ID;
        public DataType DataType => _c.DataType;
        public bool IsEncrypted => _c.IsEncrypted;
        public bool Nullable => _c.Nullable;
        public override Type OriginalType => _type;
        #endregion

        internal SMOColumn(Column c)
        {
            _c = c;
            var par = (Table)_c.Parent;
            Database = par.Parent.Name;
            Table = par.Name;
            Name = _c.Name;
        }

        public SMODatabase GetParentDatabase() => (SMODatabase)(((Table)_c.Parent).Parent as Database);
        public SMOTable GetParentTable() => (SMOTable)(_c.Parent as Table);


        public override object ShowOriginal() => _c;
        public override object Load(params string[] propertyNames)
        {
            if (propertyNames == null)
                return null;

            LoadValue(_c, propertyNames);
            return this;
        }

        public static implicit operator Column(SMOColumn smoc) => smoc.ShowOriginal() as Column;
        public static explicit operator SMOColumn(Column col) => new SMOColumn(col);
    }
}
