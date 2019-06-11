using System;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    public abstract class BaseForceSqlCmdlet : BaseSqlCmdlet
    {
        #region PARAMETERS
        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        #endregion
    }
}
