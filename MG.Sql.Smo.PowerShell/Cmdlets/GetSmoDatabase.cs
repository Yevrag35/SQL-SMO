using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
	[Cmdlet(VerbsCommon.Get, "SmoDatabase", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "ByDatabaseName")]
    [OutputType(typeof(Database))]
    public class GetSmoDatabase : BaseSqlCmdlet, IDynamicParameters
    {
        #region FIELDS/CONSTANTS
        protected private const string pName = "Name";
        private static readonly Type pType = typeof(string[]);
        private static readonly Collection<Attribute> attCol = new Collection<Attribute>
        {
            new ParameterAttribute
            {
                Mandatory = false,
                Position = 0,
                ParameterSetName = "ByDatabaseName"
            }
        };
        private string[] _dbNames;
        protected private RuntimeDefinedParameterDictionary rtDict;

        #endregion

        #region PARAMETERS


        #endregion

        #region CMDLET PROCESSING
        public object GetDynamicParameters()
        {
            if (SMOContext.IsSet && SMOContext.IsConnected && rtDict == null)
            {
                _dbNames = new string[SMOContext.Connection.Databases.Count];
                for (int i = 0; i < SMOContext.Connection.Databases.Count; i++)
                {
                    _dbNames[i] = SMOContext.Connection.Databases[i].Name;
                }

                rtDict = this.GetRTDictionary(_dbNames);
            }
            return rtDict;
        }

        protected override void BeginProcessing() => base.BeginProcessing();
		
        protected override void ProcessRecord()
		{
            string[] names = this.GetChosenNames();
            for (int i = 0; i < names.Length; i++)
            {
                WriteObject(this.GetDatabase(names[i]), false);
            }
        }

        #endregion

        #region CMDLET METHODS
        protected private Database GetDatabase(string name) => SMOContext.Connection.Databases[name];

        private RuntimeDefinedParameterDictionary GetRTDictionary(string[] dbNames)
        {
            attCol.Add(new ValidateSetAttribute(dbNames));

            var rtDict = new RuntimeDefinedParameterDictionary
            {
                { pName, new RuntimeDefinedParameter(pName, pType, attCol) }
            };
            return rtDict;
        }

        protected private string[] GetChosenNames()
        {
            string[] names = this.MyInvocation.BoundParameters.ContainsKey("Name")
                ? rtDict[pName].Value as string[]
                : _dbNames;
            return names;
        }

        #endregion
    }
}