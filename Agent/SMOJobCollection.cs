using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using SQL.SMO.Framework;
using System;
using System.Collections.Generic;

namespace SQL.SMO
{
    public class SMOJobCollection : SMOCollection<SMOJob>
    {
        public SMOJobCollection()
            : base()
        {
        }

        public SMOJobCollection(int capacity)
            : base(capacity)
        {
        }

        public SMOJobCollection(IEnumerable<SMOJob> jobs)
            : base(jobs)
        {
        }

        public SMOJobCollection(SMOJob oneJob)
            : base(oneJob)
        {
        }

        public void Add(Job job) =>
            base.Add((SMOJob)job);

        public static explicit operator SMOJobCollection(JobCollection jobCol)
        {
            var newJobCol = new SMOJobCollection(jobCol.Count);
            for (int j = 0; j < jobCol.Count; j++)
            {
                newJobCol.Add(jobCol[j]);
            }
            return newJobCol;
        }
    }
}
