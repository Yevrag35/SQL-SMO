using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    public class BaseViewCmdlet : BaseSqlCmdlet
    {

        #region PARAMETERS
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public Database Database { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            
        }

        #endregion

        #region CMDLET METHODS


        #endregion
    }
}