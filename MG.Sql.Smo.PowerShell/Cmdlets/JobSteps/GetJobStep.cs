using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace MG.Sql.Smo.PowerShell.Cmdlets.JobSteps
{
    [Cmdlet(VerbsCommon.Get, "JobStep", ConfirmImpact = ConfirmImpact.None)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(JobStep))]
    public class GetJobStep : BaseServerSqlCmdlet
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PARAMETERS


        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {

        }

        #endregion

        #region METHODS


        #endregion
    }
}