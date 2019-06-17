using MG.Dynamic;
using MG.Sql.Smo.PowerShell.Backend;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Management.Automation;
using System.Threading.Tasks;

namespace MG.Sql.Smo.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "Database", ConfirmImpact = ConfirmImpact.Low, SupportsShouldProcess = true)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(Database))]
    public class NewDatabase : BaseSqlCmdlet, IDynamicParameters
    {
        private DynamicLibrary _dynLib;
        private string _collation;
        private FileGrowthType? _fgt => this.PrimaryFileGrowthInMB.HasValue
            ? FileGrowthType.KB
            : (FileGrowthType?)null;

        private FileGrowthType? _lgt => this.LogGrowthInMB.HasValue
            ? FileGrowthType.KB
            : (FileGrowthType?)null;

        #region PARAMETERS
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; }

        [Parameter(Mandatory = false)]
        public RecoveryModel RecoveryModel = RecoveryModel.Full;

        [Parameter(Mandatory = false)]
        public string DataFilePath = null;

        [Parameter(Mandatory = false)]
        public string LogFilePath = null;

        [Parameter(Mandatory = false)]
        public double? PrimaryFileInitialSize { get; set; }

        [Parameter(Mandatory = false)]
        public double? PrimaryFileGrowthInMB { get; set; }

        [Parameter(Mandatory = false)]
        public double? PrimaryFileMaxSize { get; set; }

        [Parameter(Mandatory = false)]
        public double? LogSize { get; set; }

        [Parameter(Mandatory = false)]
        public double? LogGrowthInMB { get; set; }

        [Parameter(Mandatory = false)]
        public string Owner { get; set; }

        #endregion

        #region CMDLET PROCESSING
        public object GetDynamicParameters()
        {
            if (_dynLib == null)
            {
                _dynLib = new DynamicLibrary();
                var dp = new DynamicParameter<string>("Collation", typeof(string))
                {
                    AllowNull = true,
                    Mandatory = false
                };
                dp.ValidatedItems.AddRange(SmoContext.ServerCollations);
                _dynLib.Add(dp);
            }
            return _dynLib;
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _collation = !_dynLib.ParameterHasValue("Collation") 
                ? SmoContext.Connection.Collation 
                : _dynLib.GetParameterValue<string>("Collation");

            if (string.IsNullOrEmpty(this.DataFilePath))
                this.DataFilePath = SmoDatabase.GetDefaultDatabasePath();

            if (string.IsNullOrEmpty(this.LogFilePath))
                this.LogFilePath = SmoDatabase.GetDefaultLogPath();
        }

        protected override void ProcessRecord()
        {
            if (base.ShouldProcess(SmoContext.Connection.Name, "Creating database '" + this.Name + "'"))
            {
                Database newDb = SmoDatabase.NewDatabaseObject(this.Name, _collation, this.RecoveryModel);
                FileGroup primaryFg = SmoDatabase.NewPrimaryFileGroup(ref newDb);

                if (this.PrimaryFileGrowthInMB.HasValue)
                    this.PrimaryFileGrowthInMB = this.PrimaryFileGrowthInMB.Value * 1024;

                if (this.LogGrowthInMB.HasValue)
                    this.LogGrowthInMB = this.LogGrowthInMB.Value * 1024;

                SmoDatabase.NewPrimaryFile(ref primaryFg, this.DataFilePath, this.PrimaryFileInitialSize, this.PrimaryFileGrowthInMB, _fgt, this.PrimaryFileMaxSize);
                SmoDatabase.NewPrimaryLogFile(ref newDb, this.LogFilePath, this.LogSize, this.LogGrowthInMB, _lgt);
                if (this.MyInvocation.BoundParameters.ContainsKey("Owner"))
                    SmoDatabase.SetOwner(ref newDb, this.Owner);

                newDb.Create();
                _server.Databases.Refresh();
                WriteObject(newDb);
            }
        }

        #endregion

        #region CMDLET METHODS

        #endregion
    }
}