using MG.Dynamic;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "SmoDatabase", ConfirmImpact = ConfirmImpact.None)]
    [OutputType(typeof(Database))]
    public class GetSmoDatabase : GetSmoDatabaseBase
    {
        protected override string StatusFormat => null;
        protected override string Activity => null;
        protected override ICollection<string> Items => null;

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            IEnumerable<Database> dbs = _dynLib != null && _dynLib.ParameterHasValue(DBNAME)
                ? _dynLib.GetUnderlyingValues<Database>(DBNAME)
                : SmoContext.Connection.Databases.Cast<Database>();

            WriteObject(dbs, true);
        }

        #endregion
    }
}