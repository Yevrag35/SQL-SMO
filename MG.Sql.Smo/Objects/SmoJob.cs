using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace MG.Sql.Smo
{
    public class SmoJob : IAlterable, ICreatable, IDroppable, IDropIfExists, IRefreshable, IRenamable, IScriptable, ISfcValidate
    {
        #region FIELDS/CONSTANTS
        private Job _job;

        #endregion

        #region PROPERTIES
        public string Category { get => _job.Category; set => _job.Category = value; }
        public int CategoryId => _job.CategoryID;
        public byte CategoryType { get => _job.CategoryType; set => _job.CategoryType = value; }
        public int CurrentRunRetryAttempt => _job.CurrentRunRetryAttempt;
        public JobExecutionStatus CurrentRunStatus => _job.CurrentRunStatus;
        public string CurrentRunStep => _job.CurrentRunStep;
        public DatabaseEngineEdition DatabaseEngineEdition => _job.DatabaseEngineEdition;
        public DatabaseEngineType DatabaseEngineType => _job.DatabaseEngineType;
        public DateTime DateCreated => _job.DateCreated;
        public DateTime DateLastModified => _job.DateLastModified;
        public CompletionAction DeleteLevel { get => _job.DeleteLevel; set => _job.DeleteLevel = value; }
        public string Description { get => _job.Description; set => _job.Description = value; }
        public CompletionAction EmailLevel { get => _job.EmailLevel; set => _job.EmailLevel = value; }
        public CompletionAction EventLogLevel { get => _job.EventLogLevel; set => _job.EventLogLevel = value; }
        public ExecutionManager ExecutionManager => _job.ExecutionManager;
        public bool HasSchedule => _job.HasSchedule;
        public bool HasServer => _job.HasServer;
        public bool HasStep => _job.HasStep;
        public bool IsEnabled { get => _job.IsEnabled; set => _job.IsEnabled = value; }
        public Guid JobID => _job.JobID;
        //public IEnumerable<JobSchedule> JobSchedules => _job.JobSchedules.Cast<JobSchedule>();
        public MgSmoCollection<JobSchedule> JobSchedules => MgSmoCollection<JobSchedule>.FromSmoCollection(_job.JobSchedules);
        //public IEnumerable<JobStep> JobSteps => _job.JobSteps.Cast<JobStep>();
        public MgSmoCollection<JobStep> JobSteps => MgSmoCollection<JobStep>.FromSmoCollection(_job.JobSteps);
        public JobType JobType => _job.JobType;
        public DateTime LastRunDate => _job.LastRunDate;
        public CompletionResult LastRunOutcome => _job.LastRunOutcome;
        public string Name { get => _job.Name; set => _job.Name = value; }
        public CompletionAction NetSendLevel { get => _job.NetSendLevel; set => _job.NetSendLevel = value; }
        public DateTime NextRunDate => _job.NextRunDate;
        public int NextRunScheduleID => _job.NextRunScheduleID;
        public string OperatorToEmail { get => _job.OperatorToEmail; set => _job.OperatorToEmail = value; }
        public string OperatorToNetSend { get => _job.OperatorToNetSend; set => _job.OperatorToNetSend = value; }
        public string OperatorToPage { get => _job.OperatorToPage; set => _job.OperatorToPage = value; }
        public string OriginatingServer => _job.OriginatingServer;
        public string OwnerLoginName { get => _job.OwnerLoginName; set => _job.OwnerLoginName = value; }
        public CompletionAction PageLevel { get => _job.PageLevel; set => _job.PageLevel = value; }
        public JobServer Parent { get => _job.Parent; set => _job.Parent = value; }
        public SqlPropertyCollection Properties => _job.Properties;
        public int StartStepID { get => _job.StartStepID; set => _job.StartStepID = value; }
        public SqlSmoState State => _job.State;
        public Urn Urn => _job.Urn;
        public object UserData { get => _job.UserData; set => _job.UserData = value; }
        public int VersionNumber => _job.VersionNumber;

        #endregion

        #region CONSTRUCTOR
        private SmoJob(Job realJob) => _job = realJob;

        #endregion

        #region METHODS
        public void AddSharedSchedule(int scheduleId) => _job.AddSharedSchedule(scheduleId);
        public void Alter() => _job.Alter();
        public void ApplyToTargetServer(string serverName) => _job.ApplyToTargetServer(serverName);
        public void ApplyToTargetServerGroup(string groupName) => _job.ApplyToTargetServerGroup(groupName);
        void ICreatable.Create() => _job.Create();
        public void DeleteJobStepLogs(DateTime olderThan) => _job.DeleteJobStepLogs(olderThan);
        public void DeleteJobStepLogs(int largerThan) => _job.DeleteJobStepLogs(largerThan);
        public List<object> Discover() => _job.Discover();
        void IDroppable.Drop() => _job.Drop();
        public void Drop(bool keepUnusedSchedules) => _job.Drop(keepUnusedSchedules);
        void IDropIfExists.DropIfExists() => _job.DropIfExists();
        public DataTable EnumAlerts() => _job.EnumAlerts();
        public DataTable EnumHistory() => _job.EnumHistory();
        public DataTable EnumHistory(JobHistoryFilter filter) => _job.EnumHistory(filter);
        public DataTable EnumJobStepLogs() => _job.EnumJobStepLogs();
        public DataTable EnumJobStepLogs(int stepId) => _job.EnumJobStepLogs(stepId);
        public DataTable EnumJobStepLogs(string stepName) => _job.EnumJobStepLogs(stepName);
        public JobStep[] EnumJobStepsByID() => _job.EnumJobStepsByID();
        public DataTable EnumTargetServers() => _job.EnumTargetServers();
        public override bool Equals(object obj) => _job.Equals(obj);
        public void ExecuteWithModes(SqlExecutionModes modes, Action action) => _job.ExecuteWithModes(modes, action);
        public override int GetHashCode() => _job.GetHashCode();
        public bool Initialize() => _job.Initialize();
        public bool Initialize(bool allProperties) => _job.Initialize(allProperties);
        public void Invoke() => _job.Invoke();
        public bool IsExpressSku() => _job.IsExpressSku();
        public bool IsSupportedProperty(string propertyName) => _job.IsSupportedProperty(propertyName);
        public void PurgeHistory() => _job.PurgeHistory();
        void IRefreshable.Refresh() => _job.Refresh();
        public void RemoveAllJobSchedules() => _job.RemoveAllJobSchedules();
        public void RemoveAllJobSchedules(bool keepUnusedSchedules) => _job.RemoveAllJobSchedules(keepUnusedSchedules);
        public void RemoveAllJobSteps() => _job.RemoveAllJobSteps();
        public void RemoveFromTargetServer(string serverName) => _job.RemoveFromTargetServer(serverName);
        public void RemoveFromTargetServerGroup(string groupName) => _job.RemoveFromTargetServerGroup(groupName);
        public void RemoveSharedSchedule(int scheduleId) => _job.RemoveSharedSchedule(scheduleId);
        public void RemoveSharedSchedule(int scheduleId, bool keepUnusedSchedules) => _job.RemoveSharedSchedule(scheduleId, keepUnusedSchedules);
        void IRenamable.Rename(string newname) => _job.Rename(newname);
        StringCollection IScriptable.Script() => _job.Script();
        StringCollection IScriptable.Script(ScriptingOptions scriptingOptions) => _job.Script(scriptingOptions);
        public void SetAccessToken(IRenewableToken token) => _job.SetAccessToken(token);
        public void Start() => _job.Start();
        public void Start(string stepName) => _job.Start(stepName);
        public void Stop() => _job.Stop();
        public void Touch() => _job.Touch();
        ValidationState ISfcValidate.Validate(string methodName, params object[] arguments) => _job.Validate(methodName, arguments);

        #endregion

        #region CASTS/OPERATORS
        public static implicit operator SmoJob(Job sqlJob) => new SmoJob(sqlJob);
        public static implicit operator Job(SmoJob smoJob) => smoJob._job;

        public static MgSmoCollection<Job> ToSmoCollection(JobCollection jobCol)
        {
            return new MgSmoCollection<Job>(jobCol);
        }

        #endregion
    }
}
