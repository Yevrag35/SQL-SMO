using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace SQL.SMO.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "SMOSqlAgent", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "DynamicParameter")]
    [OutputType(typeof(SMOAgent))]
    [CmdletBinding(PositionalBinding = false)]
    public class GetSMOSqlAgent : SharedCmdlet
    {
        private protected Dynamic _dyn;
        private protected SMOAgent smoagnt;
        private protected string[] _pns;
        
        private protected const string ptl = "Properties";

        [Parameter(Mandatory = true, ParameterSetName = "Non-DynamicParameter", DontShow = true)]
        [SupportsWildcards()]
        public string[] Property { get; set; }

        internal override RuntimeDefinedParameterDictionary GenerateFor()
        {
            if (_dyn == null)
                _dyn = new Dynamic();

            if (smoagnt == null)
                smoagnt = (SMOAgent)Context.Connection.JobServer;

            if (_pns == null)
                _pns = SMOPropertyLoader.GetPropertyNames(smoagnt).Where(
                    x => !SMOAgent.SkipThese.Contains(x)).ToArray();

            _source = Dynamic.ToDictionary(_dyn.Generate(ptl, _pns, false, 0, "DynamicParameter"));
            return _source;
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (smoagnt == null)
                smoagnt = (SMOAgent)Context.Connection.JobServer;

            if (_pns == null)
                _pns = SMOPropertyLoader.GetPropertyNames(smoagnt).Where(
                    x => !SMOAgent.SkipThese.Contains(x)).ToArray();

            CheckSession();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (ParameterSetName != "Non-DynamicParameter" && _source[ptl].Value != null)
                LoadWithDynamic(ptl, smoagnt);
            else if (ParameterSetName == "Non-DynamicParameter")
                LoadWithExplicit(Property, _pns, smoagnt);

            smoagnt.Load(SMOAgent.SkipThese);
            WriteObject(smoagnt, false);
        }
    }
}
