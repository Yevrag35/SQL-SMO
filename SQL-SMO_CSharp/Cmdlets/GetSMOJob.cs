using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace SQL.SMO.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "SMOJob", ConfirmImpact = ConfirmImpact.None)]
    [OutputType(typeof(SMOJob))]
    [CmdletBinding(PositionalBinding = false)]
    public class GetSMOJob : SharedCmdlet
    {
        private Dynamic _dyn;
        private string[] _pns;
        private string[] _jobNames;

        private const string pt1 = "Name";
        private const string pt2 = "Properties";

        internal override RuntimeDefinedParameterDictionary GenerateFor()
        {
            if (_dyn == null)
                _dyn = new Dynamic();

            if (_jobNames == null)
            {
                _jobNames = new string[Context.Connection.JobServer.Jobs.Count];
                for (int i = 0; i < Context.Connection.JobServer.Jobs.Count; i++)
                {
                    var j = Context.Connection.JobServer.Jobs[i];
                    _jobNames[i] = j.Name;
                }
            }

            if (_pns == null)
                _pns = SMOPropertyLoader.GetPropertyNames(typeof(SMOJob)).Where(
                    x => !SMOJob.SkipThese.Contains(x)).ToArray();

            var param1 = _dyn.Generate(pt1, _jobNames, false, 0);
            var param2 = _dyn.Generate(pt2, _pns, false, 1);
            _source = Dynamic.ToDictionary(param1, param2);
            return _source;
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (_pns == null)
                _pns = SMOPropertyLoader.GetPropertyNames(typeof(SMOJob)).Where(
                    x => !SMOJob.SkipThese.Contains(x)).ToArray();

            CheckSession();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            SMOJobCollection jobCol;
            if (_source[pt1].Value is string[] names)
                jobCol = (SMOJobCollection)((IEnumerable<Job>)Context.Connection.JobServer.Jobs).Where(
                    x => names.Contains(x.Name));
            
            else
                jobCol = (SMOJobCollection)Context.Connection.JobServer.Jobs;
            
            if (_source[pt2].Value is string[] props)
                jobCol.LoadProperties(props);

            WriteObject(jobCol, true);
        }
    }
}
