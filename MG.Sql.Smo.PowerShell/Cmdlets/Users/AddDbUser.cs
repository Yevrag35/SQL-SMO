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
    [Cmdlet(VerbsCommon.Add, "DbUser", ConfirmImpact = ConfirmImpact.Low, SupportsShouldProcess = true, DefaultParameterSetName = "ByUserName")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SmoUser))]
    public class AddDbUser : BaseUserCmdlet, IDynamicParameters
    {
        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord() => base.ProcessRecord();

        protected override void EndProcessing()
        {
            for (int i = 0; i < _users.Count; i++)
            {
                SqlUserBase sub = _users[i];
                foreach (Database db in _dbs)
                {
                    if (base.ShouldProcess(db.Name + " within " + _server.Name, "Adding " + sub.Name))
                    {
                        SmoUser user = this.AddToDb(db, sub);
                        base.WriteObject(user);
                    }
                }
            }
            base.Refresh(_dbs);
        }

        #endregion

        #region CMDLET METHODS
        private SmoUser AddToDb(Database db, SqlUserBase sub)
        {
            var user = new User(db, sub.Name);
            try
            {
                user.Create();
            }
            catch (FailedOperationException foe)
            {
                base.ThrowInnerException(foe);
            }
            return user;
        }

        #endregion
    }
}