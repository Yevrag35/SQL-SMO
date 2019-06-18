using MG.Dynamic;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace MG.Sql.Smo.PowerShell.Cmdlets.Users
{
    [Cmdlet(VerbsCommon.Get, "DbUser", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "None")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SmoUser))]
    public class GetDbUser : BaseUserCmdlet, IDynamicParameters
    {
        #region PARAMETERS


        #endregion

        #region DYNAMIC PARAMETERS
        public override object GetDynamicParameters()
        {
            if (_dynLib == null && (this.SqlServer != null || (SmoContext.IsSet && SmoContext.IsConnected)))
            {
                Server s = this.SqlServer != null
                    ? this.SqlServer
                    : SmoContext.Connection;

                _dynLib = new DynamicLibrary();

                var dp = new DynamicParameter<Database>("Database", s.Databases.Cast<Database>(), x => x.Name, "Name", true)
                {
                    Mandatory = false
                };
                _dynLib.Add(dp);
            }
            return _dynLib;
        }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (_dbs == null)
            {
                _dbs = _dynLib.GetBackingItems<Database>("Database");
            }
        }

        protected override void ProcessRecord() => base.ProcessRecord();

        protected override void EndProcessing()
        {
            //var allUsers = new SmoUserCollection(_dbs.SelectMany(x => x.Users.Cast<User>()));
            Database[] dbs = _dbs.ToArray();
            int start = 0;
            for (int d = 0; d < dbs.Length; d++)
            {
                start = start + dbs[d].Users.Count;
            }

            var allUsers = new List<User>(start);
            this.AddToList(ref allUsers, dbs);

            for (int i = 0; i < allUsers.Count; i++)
            {
                base.WriteObject(this.ConvertUser(allUsers[i]));
            }
        }

        #endregion

        private void AddToList(ref List<User> userList, Database[] dbs)
        {
            for (int i = 0; i < dbs.Length; i++)
            {
                Database db = dbs[i];
                for (int u = 0; u < db.Users.Count; u++)
                {
                    User user = db.Users[u];
                    if (_users.Count == 0 || _users.Any(x => x.Name.Equals(user.Login, StringComparison.CurrentCultureIgnoreCase)))
                        userList.Add(user);
                }
            }
        }

        private SmoUser ConvertUser(User user) => user;
    }
}