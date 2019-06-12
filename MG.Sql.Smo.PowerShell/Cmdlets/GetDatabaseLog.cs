using MG.Dynamic;
using MG.Progress.PowerShell;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "DatabaseLog", ConfirmImpact = ConfirmImpact.None)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(LogFile))]
    public class GetDatabaseLog : GetDatabaseBase, IDynamicParameters
    {
        private List<Database> _dbs;

        protected override string Activity => null;
        protected override ICollection<string> Items => null;

        #region PARAMETERS
        [Parameter(Mandatory = false)]
        public string[] LogName { get; set; }

        #endregion

        #region CMDLET PROCESSING

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _dbs = new List<Database>();
        }

        protected override void ProcessRecord() => _dbs.AddRange(base.RetrieveDatabases());

        protected override void EndProcessing()
        {
            List<LogFile> logs = this.GetLogFiles(_dbs, this.LogName);
            logs.Sort(new LogFileComparer());
            WriteObject(logs);
        }

        #endregion

        #region METHODS
        private List<LogFile> GetLogFiles(IEnumerable<Database> dbs, params string[] logNames)
        {
            return logNames != null && logNames.Length > 0
                ? dbs.SelectMany(x => x.LogFiles.Cast<LogFile>()).Where(x => logNames.Contains(x.Name, new SmoContext.CaseInsensitiveComparer())).ToList()
                : dbs.SelectMany(x => x.LogFiles.Cast<LogFile>()).ToList();
        }

        #endregion

        private class LogFileComparer : IComparer<LogFile>
        {
            public int Compare(LogFile x, LogFile y)
            {
                int retInt = x.Parent.Name.CompareTo(y.Parent.Name);
                if (retInt == 0)
                {
                    retInt = x.Name.CompareTo(y.Name);
                }
                return retInt;
            }
        }
    }
}
