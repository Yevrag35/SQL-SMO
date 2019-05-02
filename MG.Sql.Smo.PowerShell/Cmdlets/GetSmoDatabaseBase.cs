using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
	//[Cmdlet(VerbsCommon.Get, "SmoDatabase", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "ByDatabaseName")]
    //[OutputType(typeof(Database))]
    public abstract class GetSmoDatabaseBase : ProgressCmdlet, IDynamicParameters
    {
        #region FIELDS/CONSTANTS
        protected private RuntimeDefinedParameterDictionary rtDict;

        #endregion

        #region CMDLET PROCESSING
        public object GetDynamicParameters()
        {
            if (SmoContext.IsSet && SmoContext.IsConnected && rtDict == null)
            {
                pName = NAME;
                pType = STRARR_TYPE;
                attCol = new Collection<Attribute>
                {
                    new ParameterAttribute
                    {
                        Mandatory = false,
                        Position = 0,
                        ParameterSetName = "ByDatabaseName"
                    }
                };

                rtDict = GetRTDictionary(SmoContext.DatabaseNames);
            }
            return rtDict;
        }

        #endregion

        #region CMDLET METHODS
        protected private Database GetDatabase(string name) => SmoContext.Connection.Databases[name];

        #endregion
    }
}