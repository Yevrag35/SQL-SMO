using System;
using System.Management.Automation;

namespace SQL.SMO.Cmdlets
{
    public abstract class ProgressCmdlet : SharedCmdlet
    {
        internal abstract string Activity { get; }
        internal abstract string StatusFormat { get; }
        internal abstract int Count { get; }

        internal protected void UpdateProgress(int id, int on)
        {
            var progressRecord = new ProgressRecord(id, Activity, string.Format(StatusFormat, on, Count));
            double num = Math.Round((((double)on / (double)Count) * 100), 2, MidpointRounding.ToEven);
            progressRecord.PercentComplete = Convert.ToInt32(num);
            WriteProgress(progressRecord);
        }
        internal protected void UpdateProgress(int id)      // Complete it.
        {
            var pr = new ProgressRecord(id, Activity, "Completed!")
            {
                RecordType = ProgressRecordType.Completed
            };
            WriteProgress(pr);
        }
    }
}
