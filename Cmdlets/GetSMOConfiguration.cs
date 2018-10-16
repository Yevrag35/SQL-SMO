using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Config;
using SQL.SMO.Framework;
using System;
using System.Management.Automation;

namespace SQL.SMO.Cmdlets
{

    [Cmdlet(VerbsCommon.Get, "SMOConfiguration")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SMOProperty))]
    public class GetSMOConfiguration : SharedCmdlet
    {
        private bool _includenulls;
        private Dynamic _dyn;
        [Parameter(Mandatory = false)]
        public SwitchParameter IncludeInvalidProperties
        {
            get => _includenulls;
            set => _includenulls = value;
        }

        internal override RuntimeDefinedParameterDictionary GenerateFor()
        {
            if (_dyn == null) { _dyn = new Dynamic(); }
            _source = _dyn.Generate(pName, Context.ConfigProperties, false);
            return _source;
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            CheckSession();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            // Determine properties that were chosen.
            var props = (string[])_source[pName].Value;
            if (props == null)
            {
                props = Context.ConfigProperties;
            }
            for (int i = 0; i < props.Length; i++)
            {
                string p = props[i];
                SMOProperty get = GetPropertyValue(p);
                if (get != null && (get.IsDefined || (!get.IsDefined && _includenulls)))
                {
                    WriteObject(get);
                }
            }
        }
    }
}
