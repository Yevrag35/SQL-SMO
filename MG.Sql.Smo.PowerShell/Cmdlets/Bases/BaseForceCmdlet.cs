using System;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    public abstract class BaseForceSqlCmdlet : BaseServerSqlCmdlet
    {
        #region PARAMETERS
        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        protected override void BeginProcessing() => base.BeginProcessing();

        #endregion
    }
}
