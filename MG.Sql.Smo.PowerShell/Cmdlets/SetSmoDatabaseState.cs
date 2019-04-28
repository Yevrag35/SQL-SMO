using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Set, "SmoDatabaseState", DefaultParameterSetName = "ByDatabaseName", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    public class SetSmoDatabaseState : GetSmoDatabase
    {
        #region PARAMETERS
        [Parameter(Mandatory = true, ParameterSetName = "ByPipelineInput", Position = 0, DontShow = true, ValueFromPipeline = true)]
        public Database InputObject { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateSet("ReadOnly", "ReadWrite")]
        public string Access { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateSet("Detached", "Emergency", "Offline", "Online")]
        public string Status { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateSet("SingleUser", "MultiUser", "RestrictedUser")]
        public string UserMode { get; set; }
        
        public SwitchParameter Force { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (this.ParameterSetName == "ByDatabaseName" && (rtDict == null || !rtDict[pName].IsSet))
                throw new ArgumentNullException("Name");
        }

        protected override void ProcessRecord()
        {

        }

        #endregion

        #region CMDLET METHODS


        #endregion
    }
}