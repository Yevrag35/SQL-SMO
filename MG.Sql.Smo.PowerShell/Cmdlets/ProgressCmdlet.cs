using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    public abstract class ProgressCmdlet : BaseSqlCmdlet
    {
        private const string COMPLETED = "Completed";
        protected private const double HUNDRED = 100d;
        protected private const MidpointRounding MIDPOINT = MidpointRounding.ToEven;
        protected private const ProgressRecordType REC_TYPE_COMPLETED = ProgressRecordType.Completed;
        protected private const int ROUND_DIGITS = 2;

        protected abstract string StatusFormat { get; }
        protected abstract string Activity { get; }
        protected abstract ICollection<string> Items { get; }

        protected private void UpdateProgress(int id, int on)
        {
            var pr = new ProgressRecord(id, this.Activity, string.Format(
                this.StatusFormat, on, Items.Count)
            );
            this.WriteTheProgress(pr, on);
        }

        protected private void UpdateProgressAndName(int id, int on, string name)
        {
            var pr = new ProgressRecord(id, this.Activity, string.Format(
                this.StatusFormat, on, Items.Count, name)
            );
            this.WriteTheProgress(pr, on);
        }

        protected private void UpdateProgress(int id)
        {
            var pr = new ProgressRecord(id, this.Activity, COMPLETED)
            {
                RecordType = REC_TYPE_COMPLETED
            };
            WriteProgress(pr);
        }

        private void WriteTheProgress(ProgressRecord pr, int on)
        {
            double num = Math.Round(on / (double)Items.Count * HUNDRED, ROUND_DIGITS, MIDPOINT);
            pr.PercentComplete = Convert.ToInt32(num);
            WriteProgress(pr);
        }
    }
}