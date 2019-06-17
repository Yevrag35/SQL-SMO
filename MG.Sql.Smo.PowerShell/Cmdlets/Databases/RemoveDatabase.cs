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
    [Cmdlet(VerbsCommon.Remove, "Database", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true, 
        DefaultParameterSetName = "ByDatabaseName")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(void))]
    public class RemoveDatabase : BaseForceSqlCmdlet
    {
        #region FIELDS/CONSTANTS
        private List<Database> _dbs;

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "FromPipelineInput", DontShow = true)]
        public Database InputObject { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByDatabaseName")]
        public string[] Name { get; set; }

        [Parameter(Mandatory = false, DontShow = true)]
        public Server SqlServer { get; set; }

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

            _dbs = new List<Database>();
        }

        protected override void ProcessRecord()
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("InputObject"))
                _dbs.Add(this.InputObject);

            else
            {
                for (int i = 0; i < this.Name.Length; i++)
                {
                    string n = this.Name[i];
                    if (_server.Databases.Contains(n))
                        _dbs.Add(_server.Databases[n]);
                }
            }
        }

        protected override void EndProcessing()
        {
            for (int i = 0; i < _dbs.Count; i++)
            {
                Database db = _dbs[i];
                if (this.Force || base.ShouldProcess(_server.Name, "Delete database '" + db.Name))
                {
                    try
                    {
                        _server.KillDatabase(db.Name);
                    }
                    catch (FailedOperationException foe)
                    {
                        base.ThrowInnerException(foe);
                    }
                }
            }
            _server.Refresh();
        }

        #endregion

        #region METHODS


        #endregion
    }
}