using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MG.Sql.Smo
{
    public class JobHistoryCollection : DataItemCollection<JobHistoryItem>
    {
        private const int NEG = -1;

        private JobHistoryCollection(DataTable dataTable) : base(dataTable) { }

        public void SortByDate(bool descending = false)
        {
            if (!descending)
                _list.Sort(new JobHistoryAscendingDateComparer());

            else
                _list.Sort(new JobHistoryDateComparer());
        }
        public void SortByJobAndServer() => _list.Sort(new JobHistoryNameComparer());

        #region COMPARERS
        private class JobHistoryDateComparer : IComparer<JobHistoryItem>
        {
            public int Compare(JobHistoryItem x, JobHistoryItem y) => x.RunDate.CompareTo(y.RunDate);
        }
        private class JobHistoryAscendingDateComparer : IComparer<JobHistoryItem>
        {
            public int Compare(JobHistoryItem x, JobHistoryItem y) => x.RunDate.CompareTo(y.RunDate) * NEG;
        }
        private class JobHistoryNameComparer : IComparer<JobHistoryItem>
        {
            public int Compare(JobHistoryItem x, JobHistoryItem y)
            {
                int retInt = x.Server.CompareTo(y.Server);
                if (retInt == 0)
                {
                    retInt = x.JobName.CompareTo(y.JobName);
                }
                return retInt;
            }
        }

        #endregion

        public static JobHistoryCollection GetHistory(JobServer jobServer)
        {
            return new JobHistoryCollection(jobServer.EnumJobHistory());
        }
    }
}
