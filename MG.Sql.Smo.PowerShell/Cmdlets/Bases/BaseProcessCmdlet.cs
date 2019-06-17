using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace MG.Sql.Smo.PowerShell
{
    public abstract class BaseProcessCmdlet : BaseSqlCmdlet
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = false, DontShow = true)]
        public virtual Server SqlServer { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            if (this.SqlServer == null)
            {
                base.BeginProcessing();
                _server = SmoContext.Connection;
            }
            else
                _server = this.SqlServer;
        }

        #endregion
    }
}