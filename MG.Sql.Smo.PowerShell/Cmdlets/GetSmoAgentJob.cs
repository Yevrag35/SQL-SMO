using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "SmoAgentJob", ConfirmImpact = ConfirmImpact.None)]
    public class GetSmoAgentJob : BaseSqlCmdlet, IDynamicParameters
    {
        protected private RuntimeDefinedParameterDictionary rtDict;
        protected private bool ParameterSet => rtDict[pName].IsSet;

        #region PARAMETERS

        #endregion

        #region CMDLET PROCESSING
        public object GetDynamicParameters()
        {
            if (SMOContext.IsSet && SMOContext.IsConnected && rtDict == null)
            {
                pName = NAME;
                pType = STRARR_TYPE;
                attCol = new Collection<Attribute>
                {
                    new ParameterAttribute
                    {
                        Mandatory = false,
                        Position = 0,
                        ParameterSetName = "ByJobName"
                    }
                };
                rtDict = GetRTDictionary(SMOContext.JobNames);
            }
            return rtDict;
        }

        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            if (this.ParameterSet)
                WriteObject(this.GetJobsByName(GetChosenValues<string>(pName, rtDict)), true);

            else
                WriteObject(this.GetAllJobs(), true);

        }

        #endregion

        #region CMDLET METHODS

        protected private JobCollection GetAllJobs() =>
            SMOContext.Connection.JobServer.Jobs;

        protected private IEnumerable<Microsoft.SqlServer.Management.Smo.Agent.Job> GetJobsByName(string[] names) =>
            SMOContext.Connection.JobServer.Jobs.Cast<Microsoft.SqlServer.Management.Smo.Agent.Job>().Where(
                    x => names.Contains(x.Name)
                );

        #endregion
    }
}