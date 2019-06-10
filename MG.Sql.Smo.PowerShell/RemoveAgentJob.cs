using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Remove, "AgentJob", ConfirmImpact = ConfirmImpact.High)]
    public class RemoveAgentJob : PSCmdlet
    {
        #region PARAMETERS


        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {

        }

        #endregion

        #region CMDLET METHODS


        #endregion
    }
}