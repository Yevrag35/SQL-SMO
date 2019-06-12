using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Column", ConfirmImpact = ConfirmImpact.None)]
    public class GetColumn : BaseSqlProgressCmdlet
    {
        private List<Column> _cols;
        protected override string Activity => "SQL Column Retrieval";
        protected override ICollection<string> Items => _cols.Select(x => x.Name).ToArray();
        protected override int TotalCount => _cols.Count;

        #region PARAMETERS
        [Parameter(Mandatory = false, Position = 0)]
        [SupportsWildcards]
        public string[] ColumnName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 1)]
        public Table Table { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _cols = new List<Column>();
        }

        protected override void ProcessRecord() => _cols.AddRange(this.Table.Columns.Cast<Column>());

        protected override void EndProcessing()
        {
            for (int i = 0; i < _cols.Count; i++)
            {
                base.UpdateProgress(0, i);
                WriteObject(_cols[i]);
            }
        }

        #endregion

        #region CMDLET METHODS


        #endregion
    }
}