﻿using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Remove, "Job", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true, DefaultParameterSetName = "ByPipelineInput")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(void))]
    public class RemoveJob : JobModifyBaseCmdlet
    {
        private JobServer _js;

        #region PARAMETERS

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _js = !this.MyInvocation.BoundParameters.ContainsKey("SqlServer")
                ? SmoContext.Connection.JobServer
                : this.SqlServer.JobServer;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (base.Force || base.ShouldProcess("Job - " + _input.Name, "Remove"))
                _js.RemoveJobByID(_input.JobID);
        }

        #endregion
    }
}