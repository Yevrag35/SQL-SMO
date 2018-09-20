using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Config;
using SQL.SMO.Framework;
using System;
using System.Management.Automation;

namespace SQL.SMO.Cmdlets
{
    [Cmdlet(VerbsCommon.Set, "SMOConfiguration", DefaultParameterSetName = "ViaCommandInput", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [OutputType(typeof(PropertyChanged))]
    [CmdletBinding(PositionalBinding = false)]
    public class SetSMOConfiguration : SharedCmdlet
    {
        private Dynamic _dyn;
        private const string pSetName = "ViaCommandInput";

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ViaPipelineInput")]
        public SMOProperty SMOProperty { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public object NewValue { get; set; }

        internal override RuntimeDefinedParameterDictionary GenerateFor()
        {
            if (_dyn == null) { _dyn = new Dynamic(); }
            _source = _dyn.Generate(pName, Context.ConfigProperties, true, pSetName);
            return _source;
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            CheckSession();
        }

        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case "ViaCommandInput":
                    ByCommandInput((string)_source[pName].Value);
                    break;
                case "ViaPipelineInput":
                    ByPipelineInput(SMOProperty);
                    break;
            }
        }

        internal void ByCommandInput(string s)
        {
            SMOProperty smop = GetPropertyValue(s);
            ByPipelineInput(smop);
        }

        internal void ByPipelineInput(SMOProperty prop)
        {
            PropertyChanged change = new PropertyChanged(prop, prop.ConfigValue, NewValue);
            if (ShouldProcess(prop.Name, "Set to " + NewValue))
            {
                prop.Alter(NewValue);
                WriteObject(change);
            }
        }
    }
}
