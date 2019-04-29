using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Set, "SmoDatabaseState", DefaultParameterSetName = "ByDatabaseName", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    public class SetSmoDatabaseState : GetSmoDatabaseBase
    {
        #region FIELDS/CONSTANTS
        private const string STATUS_FORMAT_1 = "Setting ReadAccess on database {0}/{1}... {2}";
        private const string STATUS_FORMAT_2 = "Setting status on database {0}/{1}... {2}";
        private const string STATUS_FORMAT_3 = "Setting UserAccess on database {0}/{1}... {2}";
        private const string ACTIVITY = "Altering Databases";

        private string CurrentProgressStatus;
        protected override string StatusFormat => CurrentProgressStatus;
        protected override string Activity => ACTIVITY;
        protected override ICollection<string> Items => names;

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ParameterSetName = "ByPipelineInput", Position = 0, DontShow = true, ValueFromPipeline = true)]
        public Database InputObject { get; set; }

        [Parameter(Mandatory = false)]
        public ReadAccess ReadAccess { get; set; }

        [Parameter(Mandatory = false)]
        public DatabaseStatus Status { get; set; }

        [Parameter(Mandatory = false)]
        public DatabaseUserAccess UserAccess { get; set; }
        
        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        #endregion

        private bool UsingNames = false;
        private List<string> names;

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (this.ParameterSetName == "ByDatabaseName" && (rtDict == null || !rtDict[pName].IsSet))
                throw new ArgumentNullException("Name");

            else if (this.ParameterSetName == "ByDatabaseName")
                UsingNames = true;

            names = new List<string>();
        }

        protected override void ProcessRecord()
        {
            if (UsingNames)
            {
                names.AddRange(GetChosenValues<string>(NAME, rtDict));
            }
            else
                names.Add(InputObject.Name);
        }

        protected override void EndProcessing()
        {
            Database[] dbs = SMOContext.Connection.Databases.Cast<Database>().Where(x => names.Contains(x.Name)).ToArray();
            if (this.MyInvocation.BoundParameters.ContainsKey("ReadAccess"))
            {
                CurrentProgressStatus = STATUS_FORMAT_1;
                this.SetReadAccess(dbs, ReadAccess);
            }

            if (this.MyInvocation.BoundParameters.ContainsKey("Status"))
            {
                CurrentProgressStatus = STATUS_FORMAT_2;
                this.SetDatabaseStatus(dbs, Status);
            }

            if (this.MyInvocation.BoundParameters.ContainsKey("UserAccess"))
            {
                CurrentProgressStatus = STATUS_FORMAT_3;
                this.SetUserAccess(dbs, UserAccess);
            }
        }

        #endregion

        #region CMDLET METHODS
        private void SetReadAccess(Database[] dbs, ReadAccess access)
        {
            for (int i = 1; i <= dbs.Length; i++)
            {
                Database db = dbs[i - 1];
                base.UpdateProgressAndName(0, i, db.Name);
                if (Force || ShouldProcess(db.Name, string.Format("Make {0}", access.ToString())))
                {
                    db.ReadOnly = Convert.ToBoolean(access);
                    db.Alter();
                }
            }
            base.UpdateProgress(0);
        }

        private void SetDatabaseStatus(Database[] dbs, DatabaseStatus status)
        {
            for (int i = 1; i <= dbs.Length; i++)
            {
                Database db = dbs[i - 1];
                base.UpdateProgressAndName(1, i, db.Name);
                switch (status)
                {
                    case DatabaseStatus.Offline:
                        if (Force || ShouldProcess(db.Name, "Take Offline"))
                            db.SetOffline();

                        break;

                    case DatabaseStatus.Online:
                        if (Force || ShouldProcess(db.Name, "Bring Online"))
                            db.SetOnline();
                        break;

                    case DatabaseStatus.Detached:
                        if (Force || ShouldProcess(db.Name, "Detach Database"))
                            db.Parent.DetachDatabase(db.Name, true);
                        break;
                }
            }
        }

        private void SetUserAccess(Database[] dbs, DatabaseUserAccess access)
        {
            foreach (Database db in dbs)
            {
                if (Force || ShouldProcess(db.Name, string.Format("Make {0}", access.ToString())))
                {
                    db.UserAccess = access;
                    db.Alter();
                }
            }
        }

        #endregion
    }
}