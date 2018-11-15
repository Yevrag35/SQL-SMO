using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using SQL.SMO.Framework;
using System;

namespace SQL.SMO
{
    public class SMOJob : SMOPropertyLoader
    {
        private protected Job _job;
        internal static string[] SkipThese => new string[7]
        {
            "Name", "JobID", "IsEnabled", "Description",
            "JobType", "DateLastModified", "HasSchedule"
        };

        #region All Properties
        public SMOAgent Parent { get; internal set; }
        public string Category { get; internal set; }
        public byte CategoryType { get; internal set; }
        public int? CurrentRunRetryAttempt { get; internal set; }
        public JobExecutionStatus? CurrentRunStatus { get; internal set; }
        public string CurrentRunStep { get; internal set; }
        public DateTime? DateCreated { get; internal set; }
        public CompletionAction DeleteLevel { get; internal set; }
        public CompletionAction? EmailLevel { get; internal set; }
        public CompletionAction? EventLogLevel { get; internal set; }
        public bool? HasServer { get; internal set; }
        public bool? HasStep { get; internal set; }
        public DateTime? LastRunDate { get; internal set; }
        public CompletionResult? LastRunOutcome { get; internal set; }
        public CompletionAction? NetSendLevel { get; internal set; }
        public DateTime? NextRunDate { get; internal set; }
        public int? NextRunScheduleID { get; internal set; }
        public string OperatorToEmail { get; internal set; }
        public string OperatorToNetSend { get; internal set; }
        public string OperatorToPage { get; internal set; }
        public string OriginatingServer { get; internal set; }
        public string OwnerLoginName { get; internal set; }
        public CompletionAction? PageLevel { get; internal set; }
        public int? StartStepID { get; internal set; }
        public int? VersionNumber { get; internal set; }
        public int? CategoryID { get; internal set; }
        public JobStepCollection JobSteps { get; internal set; }
        public JobScheduleCollection JobSchedules { get; internal set; }
        public Urn Urn { get; internal set; }
        public SqlPropertyCollection Properties { get; internal set; }
        public DatabaseEngineType? DatabaseEngineType { get; internal set; }
        public DatabaseEngineEdition? DatabaseEngineEdition { get; internal set; }
        public ExecutionManager ExecutionManager { get; internal set; }
        public object UserData { get; internal set; }
        public SqlSmoState? State { get; internal set; }

        #endregion

        #region Default Properties
        public Guid JobID { get; internal set; }
        public bool? IsEnabled { get; internal set; }
        public override string Name { get; internal set; }
        public string Description { get; internal set; }
        public JobType? JobType { get; internal set; }
        public DateTime? DateLastModified { get; internal set; }
        public bool? HasSchedule { get; internal set; }
        public override Type OriginalType => _job.GetType();

        #endregion

        private SMOJob(Job j)
        {
            Name = j.Name;
            JobID = j.JobID;
            IsEnabled = j.IsEnabled;
            Description = j.Description;
            JobType = j.JobType;
            DateLastModified = j.DateLastModified;
            HasSchedule = j.HasSchedule;
            _job = j;
        }

        public override object ShowOriginal() => _job;
        public override void Load(params string[] propertyNames)
        {
            if (propertyNames == null)
                return;

            LoadValue(_job, propertyNames);
        }

        public static explicit operator SMOJob(Job job) => new SMOJob(job);
    }
}
