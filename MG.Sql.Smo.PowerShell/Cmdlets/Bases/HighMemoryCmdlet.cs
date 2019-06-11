using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;

namespace MG.Sql.Smo.PowerShell
{
    public abstract class HighMemoryCmdlet : BaseForceSqlCmdlet
    {
        private const string WARNING = "Enumerating this cmdlet's output can cause PowerShell to use large amounts of memory.  Use caution.";

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            base.WriteWarning(WARNING);
        }

        #endregion

        #region BACKEND METHODS
        //public static long Test(IEnumerable collection)
        //{
        //    long tot = GC.GetTotalMemory(true);
        //    Run(collection);
        //    long newTot = GC.GetTotalMemory(false);
        //    return newTot - tot;
        //}

        //private static void Run(IEnumerable collection) => collection.GetEnumerator();

        #endregion
    }
}