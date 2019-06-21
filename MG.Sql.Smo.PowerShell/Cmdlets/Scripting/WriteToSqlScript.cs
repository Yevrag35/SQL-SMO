using MG.Dynamic;
using MG.Swappable;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Data.SqlClient;
using System.Management.Automation;
using System.Reflection;
using System.Text;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommunications.Write, "ToSqlScript", ConfirmImpact = ConfirmImpact.None)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(string))]
    public class WriteToSqlScript : BaseServerSqlCmdlet, IDynamicParameters
    {
        private const string ENCODING = "Encoding";
        protected private DynamicLibrary _dynLib;
        protected private Encoding _enc;
        protected private ScriptingOptions _so;
        private EncodingInfo[] _info;

        #region OPPOSITE PARAMETERS
        private static readonly Dictionary<string, string> Opposites = new Dictionary<string, string>(7)
        {
            { "NoAgentJobId", "AgentJobId" },
            { "NoSystemObjects", "AllowSystemObjects" },
            { "IncludeVardecimal", "NoVardecimal" },
            { "NoPrimaryObject", "PrimaryObject" },
            { "NoSchemaQualify", "SchemaQualify" },
            { "NoScriptDataCompression", "ScriptDataCompression" },
            { "NoScriptSchema", "ScriptSchema" }
        };

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public IScriptable InputObject { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter AgentAlertJob { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter AgentNotify { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter AnsiFile { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter AnsiPadding { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(1, 2147483647)]
        public int BatchSize { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Bindings { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter ChangeTracking { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter ClusteredIndexes { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter ColumnStoreIndexes { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter ContinueScriptingOnError { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter ConvertUserDefinedDataTypesToBaseType { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DdlBodyOnly { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DdlHeaderOnly { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DrilAll { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DriAllConstraints { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DriAllKeys { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DriChecks { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DriClustered { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DriDefaults { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DriForeignKeys { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DriIncludeSystemNames { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DriIndexes { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DriNonClustered { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DriPrimaryKey { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DriUniqueKeys { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DriWithNoCheck { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter EnforceScriptingOptions { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter ExtendedProperties { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter FullTextCatalogs { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter FullTextIndexes { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter FullTextStopLists { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter IncludeDatabaseContext { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter IncludeDatabaseRoleMemberships { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter IncludeFullTextCatalogRootPath { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter IncludeHeaders { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter IncludeIfNotExists { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter IncludeScriptingParameterHeader { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter IncludeVardecimal { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Indexes { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter LoginSid { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoAgentJobId { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoAssemblies { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoCollation { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoCommandTerminator { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoExecuteAs { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoFileGroup { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoFileStream { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoFileStreamColumn { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoIdentities { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoIndexPartitioningSchemes { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoMailProfileAccounts { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoMailProfilePrincipals { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NonClusteredIndexes { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoPrimaryObject { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoSchemaQualify { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoScriptDataCompression { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoScriptSchema { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoSystemObjects { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoTablePartitioningSchemes { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoViewColumns { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoXmlNamespaces { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter OptimizerData { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Permissions { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter SchemaQualifyForeignKeysReferences { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter ScriptBatchTerminator { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter ScriptData { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter ScriptDrops { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter ScriptForAlter { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter ScriptForCreateDrop { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter ScriptOwner { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter SpatialIndexes { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Statistics { get; set; }

        [Parameter(Mandatory = false)]
        public DatabaseEngineEdition TargetDatabaseEngineEdition { get; set; }

        [Parameter(Mandatory = false)]
        public DatabaseEngineType TargetDatabaseEngineType { get; set; }

        [Parameter(Mandatory = false)]
        public SqlServerVersion TargetServerVersion { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter TimestampToBinary { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Triggers { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter WithDependencies { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter XmlIndexes { get; set; }

    #endregion

        #region DYNAMIC PARAMETERS
        public virtual object GetDynamicParameters()
        {
            if (_info == null)
            {
                _info = Encoding.GetEncodings();
            }
            _dynLib = new DynamicLibrary();
            var dp = new DynamicParameter<EncodingInfo>(ENCODING, _info, x => x.Name, "Name")
            {
                Mandatory = false,
                Position = 1
            };
            _dynLib.Add(dp);
            return _dynLib;
        }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _enc = _dynLib != null && _dynLib.ParameterHasValue(ENCODING)
                ? _dynLib.GetUnderlyingValue<EncodingInfo>(ENCODING).GetEncoding()
                : Encoding.UTF8;

            _so = this.SetOptions(this.MyInvocation.BoundParameters);
        }

        protected override void ProcessRecord()
        {   
            StringCollection script = this.InputObject.Script(_so);
            base.WriteObject(script, true);
        }

        #endregion

        #region CMDLET METHODS
        private bool HasWritableValue(object inVal, out object outVal)
        {
            bool result = false;
            outVal = null;
            if (inVal is SwitchParameter sw)
            {
                outVal = sw.ToBool();
                result = true;
            }
            else if (inVal is int || inVal is bool)
            {
                outVal = inVal;
                result = true;
            }
            else if (inVal is ValueType vt)
            {
                outVal = Convert.ToInt32(vt);
                result = true;
            }
            return result;
        }

        private void ReplaceParameters(ref SwappableDictionary swapd)
        {
            foreach (KeyValuePair<string, string> kvp in Opposites)
            {
                if (swapd.ContainsKey(kvp.Key))
                {
                    bool val = !((SwitchParameter)swapd[kvp.Key]).ToBool();
                    ISwappable swap = swapd.NewSwappable(kvp.Value, kvp.Key);
                    swapd.Swap(swap);
                    swapd[kvp.Value] = val;
                }
            }
        }

        private ScriptingOptions SetOptions(IDictionary inParams)
        {
            var so = new ScriptingOptions();

            var sd = SwappableDictionary.FromIDictionary(inParams);
            so.Encoding = _enc;
            if (sd.ContainsKey(ENCODING))
            {
                sd.Remove(ENCODING);
            }

            this.ReplaceParameters(ref sd);

            PropertyInfo[] allProps = so.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < allProps.Length; i++)
            {
                PropertyInfo pi = allProps[i];
                if (sd.TryGetKey(pi.Name, StringComparison.CurrentCultureIgnoreCase, out string realKey)
                    && this.HasWritableValue(sd[realKey], out object write))
                {
                    pi.SetValue(so, write);
                }
            }
            return so;
        }
        
        #endregion
    }
}