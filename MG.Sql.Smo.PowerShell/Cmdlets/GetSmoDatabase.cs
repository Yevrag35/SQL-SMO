using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "SmoDatabase", ConfirmImpact = ConfirmImpact.None)]
    [OutputType(typeof(Database))]
    public class GetSmoDatabase : GetSmoDatabaseBase
    {
        protected override string StatusFormat => null;
        protected override string Activity => null;
        protected override ICollection<string> Items => null;

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            string[] names = this.MyInvocation.BoundParameters.ContainsKey(NAME)
                ? GetChosenValues<string>(NAME, rtDict)
                : SMOContext.DatabaseNames;

            for (int i = 0; i < names.Length; i++)
            {
                WriteObject(this.GetDatabase(names[i]), false);
            }
        }

        #endregion
    }
}