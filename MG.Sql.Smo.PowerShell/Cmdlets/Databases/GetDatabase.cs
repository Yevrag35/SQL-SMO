using MG.Dynamic;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "Database", ConfirmImpact = ConfirmImpact.None)]
    [OutputType(typeof(Database))]
    [Alias("getdb")]
    [CmdletBinding(PositionalBinding = false)]
    public class GetDatabase : BaseDatabaseCmdlet
    {
        protected override string Activity => null;
        protected override ICollection<string> Items => null;

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (_dynLib != null)
                _dbs = new MgSmoCollection<Database>(base.RetrieveDatabases());
        }

        protected override void ProcessRecord()
        {
            if (_dbs != null && _dbs.Count > 0)
            {
                base.WriteObject(_dbs, true);
            }
            else if (this.SqlServer != null)
            {
                var dbCol = this.SqlServer.Databases;
                if (_dynLib.ParameterHasValue(DBNAME))
                {
                    string[] names = _dynLib.GetParameterValues<string>(DBNAME);
                    for (int i = 0; i < dbCol.Count; i++)
                    {
                        Database db = dbCol[i];

                        if (names.Contains(db.Name, new SmoContext.CaseInsensitiveComparer()))
                        {
                            base.WriteObject(db);
                        }
                    }
                }
                else
                    base.WriteObject(dbCol, true);
            }
            else
                throw new ArgumentException("SqlServer can't be null if you don't connect to an instance.");
        }

        #endregion
    }
}