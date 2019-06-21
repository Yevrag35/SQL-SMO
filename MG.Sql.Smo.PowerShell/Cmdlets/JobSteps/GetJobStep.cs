using MG.Sql.Smo.PowerShell.Extensions;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "JobStep", ConfirmImpact = ConfirmImpact.None)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(JobStep))]
    public class GetJobStep : BaseSqlCmdlet
    {
        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public SmoJob Job { get; set; }

        [Parameter(Mandatory = false, Position = 0)]
        [SupportsWildcards]
        public NumStrIdentity Identity { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            if (this.Identity != null)
            {
                if (this.Identity.IsNumeric && this.Job.JobSteps.TryFind(x => x.ID.Equals((int)this.Identity), out JobStep jStep))
                {
                    base.WriteObject(jStep);
                }
                else
                {
                    base.WriteObject(this.Job.JobSteps.WildcardMatch("Name", (string)this.Identity), true);
                }
            }
            else
            {
                base.WriteObject(this.Job.JobSteps, true);
            }
            NoEnd = true;
        }

        protected override void EndProcessing() { }

        #endregion
    }
}