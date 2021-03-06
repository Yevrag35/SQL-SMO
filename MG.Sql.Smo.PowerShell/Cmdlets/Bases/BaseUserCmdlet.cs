﻿using MG.Sql.Smo.Exceptions;
using MG.Dynamic;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell
{
    public abstract class BaseUserCmdlet : BaseServerSqlCmdlet, IDynamicParameters
    {
        protected private DynamicLibrary _dynLib;
        protected private IEnumerable<Database> _dbs;
        protected private List<SqlUserBase> _users;

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "BySmoUserBase")]
        public SqlUserBase UserObject { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByUserName")]
        public string[] UserName { get; set; }

        #endregion

        #region DYNAMIC PARAMETERS
        public virtual object GetDynamicParameters()
        {
            if (_dynLib == null && SmoContext.IsSet && SmoContext.IsConnected)
            {
                if (SmoContext.GetNullOrEmpty(SmoContext.Databases))
                    SmoContext.SetDatabases(SmoContext.Connection.Databases);

                _dynLib = this.NewDynamicLibrary(SmoContext.Databases);
            }
            else if (_dynLib == null)
                _dynLib = this.NewDynamicLibrary();


            return _dynLib;
        }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (_dynLib.ParameterHasValue("Database"))
                _dbs = _dynLib.GetUnderlyingValues<Database>("Database");

            _users = new List<SqlUserBase>();
        }

        protected override void ProcessRecord()
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("UserObject"))
                _users.Add(this.UserObject);

            else if (this.UserName != null && this.UserName.Length > 0)
            {
                for (int i = 0; i < this.UserName.Length; i++)
                {
                    _users.Add(this.GetUser(this.UserName[i]));
                }
            }
        }

        #endregion

        #region CMDLET METHODS
        protected private SqlUserBase GetUser(string userName)
        {
            return _server.Logins.Contains(userName)
                ? SqlUserBase.ConvertFromSql(_server.Logins[userName])
                : null;
        }

        protected private DynamicLibrary NewDynamicLibrary()
        {
            var dl = new DynamicLibrary
            {
                {
                    BaseDatabaseCmdlet.DBNAME,
                    new RuntimeDefinedParameter(BaseDatabaseCmdlet.DBNAME, typeof(string[]), new Collection<Attribute>
            {
                new ParameterAttribute
                {
                    Mandatory = false
                }
            })
                }
            };
            return dl;
        }
        protected private DynamicLibrary NewDynamicLibrary(MgSmoCollection<Database> dbCol)
        {
            var dl = new DynamicLibrary();
            var dp = new DynamicParameter<Database>(BaseDatabaseCmdlet.DBNAME, SmoContext.Databases, x => x.Name, "Name", true)
            {
                Mandatory = true
            };
            dl.Add(dp);
            return dl;
        }

        protected private void Refresh(IEnumerable<IRefreshable> refreshables)
        {
            foreach (IRefreshable ire in refreshables)
            {
                ire.Refresh();
            }
        }

        protected private void Refresh(IEnumerable<SmoCollectionBase> cols)
        {
            foreach (SmoCollectionBase col in cols)
            {
                col.Refresh();
            }
        }

        #endregion
    }
}