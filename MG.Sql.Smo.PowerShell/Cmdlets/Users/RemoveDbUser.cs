using MG.Sql.Smo.Exceptions;
using MG.Dynamic;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Remove, "DbUser", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true, DefaultParameterSetName = "ByUserName")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(void))]
    public class RemoveDbUser : BaseUserCmdlet, IDynamicParameters
    {
        #region PARAMETERS
        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord() => base.ProcessRecord();

        protected override void EndProcessing()
        {
            for (int i = 0; i < _users.Count; i++)
            {
                SqlUserBase sub = _users[i];
                IEnumerable<Database> _onlyThese = _dbs.Where(x => x.Users.Contains(sub.Name));
                foreach (Database db in _dbs)
                {
                    if (this.Force || base.ShouldProcess(db.Name + " within " + _server.Name, "Removing " + sub.Name))
                    {
                        this.RemoveFromDb(db, sub);
                    }
                }
            }
            base.Refresh(_dbs);
        }

        #endregion

        #region METHODS
        private void CheckForOwnedObjects(User user)
        {

        }

        private void RemoveFromDb(Database db, SqlUserBase sub)
        {
            try
            {
                User u = db.Users[sub.Name];
                //u.Drop();
            }
            catch (FailedOperationException foe)
            {
                base.ThrowInnerException(foe);
            }
        }

        #endregion
    }
}