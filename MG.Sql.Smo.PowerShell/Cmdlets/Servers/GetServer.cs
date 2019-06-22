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
    [Cmdlet(VerbsCommon.Get, "Server", ConfirmImpact = ConfirmImpact.None)]
    [OutputType(typeof(SmoServer))]
    public class GetServer : BaseSqlCmdlet, IDynamicParameters
    {
        private const string alias = "p";
        //protected private RuntimeDefinedParameterDictionary rtDict;
        protected private DynamicLibrary _dynLib;
        private string[] AllProps;

        #region DYNAMIC PARAMETER PROCESSING
        public object GetDynamicParameters()
        {
            if (_dynLib == null)
            {
                if (AllProps == null)
                    AllProps = this.GetPropsToLoad();

                _dynLib = new DynamicLibrary();
                var dp = new DynamicParameter<string>(PROPERTIES, AllProps, x => x, null, true)
                {
                    Mandatory = false
                };
                dp.ValidatedItems.Add("*");
                _dynLib.Add(dp);
            }
            return _dynLib;
        }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            var smoServer = (SmoServer)SmoContext.Connection;
            if (_dynLib.ParameterHasValue(PROPERTIES))
            {
                string[] props = _dynLib.GetParameterValue<string[]>(PROPERTIES);
                if (props.Contains("*"))
                {
                    WriteWarning("Loading all properties can cause PowerShell to use large amounts of memory if enumerated.  Use caution.");
                    smoServer.LoadProperty(AllProps);
                }
                else
                    smoServer.LoadProperty(props);
                
            }
            WriteObject(smoServer, false);
        }

        #endregion

        #region CMDLET METHODS
        private string[] GetPropsToLoad()
        {
            // Validate Server Version
            Func<PropertyInfo, bool> clause = SmoContext.Connection.Version.Major > 11
                ? x => !x.Name.Equals("ActiveDirectory") && x.CanWrite
                : (Func<PropertyInfo, bool>)(x => x.CanWrite);

            return typeof(SmoServer).GetProperties(FLAGS).Where(clause).Select(x => x.Name).ToArray();
        }
        #endregion
    }
}