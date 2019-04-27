using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "SmoServer", ConfirmImpact = ConfirmImpact.None)]
    [OutputType(typeof(SmoServer))]
    public class GetSmoServer : BaseSqlCmdlet, IDynamicParameters
    {
        private const string pName = "Properties";
        private const string alias = "p";
        private static readonly Type pType = typeof(string[]);
        private static readonly Collection<Attribute> attCol = new Collection<Attribute>
        {
            new ParameterAttribute
            {
                Position = 0,
                Mandatory = false
            },
            new AliasAttribute(alias)
        };

        private RuntimeDefinedParameterDictionary rtDict;

        #region DYNAMIC PARAMETER PROCESSING
        public object GetDynamicParameters()
        {
            if (rtDict == null)
                rtDict = this.GetRTDictionary(this.GetPropsToLoad());
            
            return rtDict;
        }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            var smoServer = (SmoServer)SMOContext.Connection;
            if (rtDict[pName].Value is string[] props)
            {
                smoServer.LoadProperty(props);
            }
            WriteObject(smoServer, false);
        }

        #endregion

        #region CMDLET METHODS
        private string[] GetPropsToLoad() => SmoServer.thisProps.Where(
            x => x.CanWrite).Select(
                x => x.Name).ToArray();

        private RuntimeDefinedParameterDictionary GetRTDictionary(string[] props)
        {
            attCol.Add(new ValidateSetAttribute(props));

            var rtDict = new RuntimeDefinedParameterDictionary
            {
                { pName, new RuntimeDefinedParameter(pName, pType, attCol) }
            };
            return rtDict;
        }

        #endregion
    }
}