using MG.Dynamic;
using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "Process", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "None")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SqlProcess))]
    public class GetProcess : BaseServerSqlCmdlet, IDynamicParameters
    {
        #region FIELDS/CONSTANTS
        protected private DynamicLibrary _dynLib = null;

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "FromPipelineInput", DontShow = true)]
        public SmoLogin InputObject { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ByLoginName")]
        public string LoginName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ByProcessId")]
        public int ProcessId { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "None")]
        [Parameter(Mandatory = false, ParameterSetName = "FromPipelineInput")]
        [Parameter(Mandatory = false, ParameterSetName = "ByLoginName")]
        [ValidateSet("Id", "Login", "Program")]
        public string SortBy { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "None")]
        [Parameter(Mandatory = false, ParameterSetName = "ByLoginName")]
        [Parameter(Mandatory = false, ParameterSetName = "FromPipelineInput")]
        public ProcessStatus? Status { get; set; }

        #endregion

        #region DYNAMIC PARAMETERS
        public object GetDynamicParameters()
        {
            if (!string.IsNullOrEmpty(this.SortBy) && this.SortBy == "Id")
            {
                _dynLib = new DynamicLibrary
                {
                    {
                        "Ascending",
                        new RuntimeDefinedParameter("Ascending", typeof(SwitchParameter), new Collection<Attribute>
                    {
                        new ParameterAttribute
                        {
                            Mandatory = false,
                            ParameterSetName = "None"
                        },
                        new ParameterAttribute
                        {
                            Mandatory = false,
                            ParameterSetName = "FromPipelineInput"
                        },
                        new ParameterAttribute
                        {
                            Mandatory = false,
                            ParameterSetName = "ByLoginName"
                        }
                    }
                )
                    }
                };
            }
            return _dynLib;
        }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            if (_server == null && this.InputObject != null)
                _server = this.InputObject.Parent;

            if (_server == null)
                throw new SmoContextNotSetException();

            if (!string.IsNullOrEmpty(this.LoginName) && base.GetLoginFromName(this.LoginName, out SmoLogin found))
                this.InputObject = found;

            if (this.MyInvocation.BoundParameters.ContainsKey("ProcessId"))
                base.WriteObject(this.FindProcess(this.ProcessId));

            else
            {
                SqlProcessCollection col = this.InputObject != null 
                    ? this.FindProcess(this.InputObject) 
                    : this.FindProcess();

                if (this.Status.HasValue)
                    col = (SqlProcessCollection)col.FindAll(x => x.Status.HasValue && x.Status.Value == this.Status.Value);

                switch (this.SortBy)
                {
                    case "Id":
                    {
                        if (_dynLib.ParameterHasValue("Ascending") && _dynLib.GetParameterValue<bool>("Ascending"))
                            col.SortBySpid(true);

                        else
                            col.SortBySpid(false);

                        break;
                    }
                    case "Login":
                        col.SortByLogin();
                        break;

                    case "Program":
                        col.SortByProgram();
                        break;

                    default:
                        col.SortBySpid(false);
                        break;
                }
                base.WriteObject(col, true);
            }
        }

        #endregion

        #region METHODS
        protected private SqlProcessCollection FindProcess() => SqlProcessCollection.GetProcesses(_server);
        protected private SqlProcessCollection FindProcess(SmoLogin login) => SqlProcessCollection.GetProcesses(login.Name, _server);
        protected private SqlProcess FindProcess(int processId) => SqlProcessCollection.GetProcess(processId, _server);

        #endregion
    }
}