using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "SmoServer", ConfirmImpact = ConfirmImpact.None)]
    [OutputType(typeof(SmoServer))]
    public class GetSmoServer : BaseSqlCmdlet, IDynamicParameters
    {
        private const string alias = "p";
        protected private RuntimeDefinedParameterDictionary rtDict;

        #region DYNAMIC PARAMETER PROCESSING
        public object GetDynamicParameters()
        {
            if (rtDict == null)
            {
                attCol = new Collection<Attribute>
                {
                    new ParameterAttribute
                    {
                        Mandatory = false
                    }
                };
                pName = PROPERTIES;
                pType = STRARR_TYPE;

                rtDict = GetRTDictionary(this.GetPropsToLoad());
            }
            
            return rtDict;
        }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            var smoServer = (SmoServer)SmoContext.Connection;
            if (rtDict[pName].IsSet)
            {
                string[] props = GetChosenValues<string>(pName, rtDict);
                smoServer.LoadProperty(props);
            }
            WriteObject(smoServer, false);
        }

        #endregion

        #region CMDLET METHODS
        private string[] GetPropsToLoad() => typeof(SmoServer).GetProperties().Where(
            x => x.CanWrite).Select(
                x => x.Name).ToArray();

        #endregion
    }
}