using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace MG.Sql.Smo
{
    public class JobHistoryItem : DataRowClass
    {
        #region PROPERTIES
        public int InstanceId { get; private set; }
        public Guid JobId { get; private set; }
        public string JobName { get; private set; }
        public string Message { get; private set; }
        public string OperatorEmailed { get; private set; }
        public string OperatorNetSent { get; private set; }
        public int RetriesAttempted { get; private set; }
        public DateTime RunDate { get; private set; }
        public int RunDuration { get; private set; }
        public int RunStatus { get; private set; }
        public string Server { get; private set; }
        public int SqlMessageID { get; private set; }
        public int Severity { get; private set; }
        public int StepID { get; private set; }
        public string StepName { get; private set; }

        #endregion

        public JobHistoryItem(DataRow dataRow)
            : base(dataRow)
        {
        }
    }
}
