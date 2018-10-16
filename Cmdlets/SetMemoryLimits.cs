using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Config;
using SQL.SMO.Framework;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace SQL.SMO.Cmdlets
{
    [Cmdlet(VerbsCommon.Set, "SQLMemoryLimit", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(PropertyChanged))]
    public class SetSQLMemoryLimits : PSCmdlet
    {
        [Parameter(Mandatory = false, Position = 0)]
        public long? MinServerMemory = null;

        [Parameter(Mandatory = false, Position = 1)]
        public long? MaxServerMemory = null;

        [Parameter(Mandatory = false, Position = 2)]
        public MeasurementUnit InUnitsOf = MeasurementUnit.MB;

        private bool _force;
        [Parameter(Mandatory = false)]
        public SwitchParameter Force
        {
            get => _force;
            set => _force = value;
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (MaxServerMemory == null && MinServerMemory == null)
            {
                throw new ArgumentNullException("Either specify a single Min or Max server memory value or both!");
            }
            else if (MinServerMemory.Equals(MaxServerMemory))
            {
                WriteWarning("Both the Minimum and Maximum memory values are specified at " + MaxServerMemory + ".  Ignore if this was intended.");
            }
            SharedCmdlet.CheckSession();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var col = new List<PropertyChanged>();

            Configuration conf = ((Server)Context.Connection).Configuration;
            var serverMemory = new MemoryProperty(
                conf.MinServerMemory,
                conf.MaxServerMemory,
                MeasurementUnit.MB
            );
            var newProp = new MemoryProperty(
                MinServerMemory,
                MaxServerMemory,
                InUnitsOf
            );
            newProp = newProp.ChangeUnitsTo(MeasurementUnit.MB);
            if (MinServerMemory.HasValue)
            {
                if (newProp.MinimumMemory.Value.Equals(serverMemory.MinimumMemory.Value))
                {
                    WriteWarning("The specified minimum value is already the current running value.  No settings will be changed.");
                }
                else if (_force || ShouldProcess(conf.MinServerMemory.DisplayName, "Change config value"))
                {
                    var changed = new PropertyChanged("MinServerMemory");
                    changed.SetOldValue(serverMemory.ToString(MemoryProperty.MemType.Min, InUnitsOf));
                    conf.MinServerMemory.ConfigValue = Convert.ToInt32(newProp.MinimumMemory.Value);
                    conf.Alter();
                    changed.SetNewValue(newProp.ToString(MemoryProperty.MemType.Min, InUnitsOf));
                    col.Add(changed);
                }
            }
            if (MaxServerMemory.HasValue)
            {
                if (newProp.MaximumMemory.Value.Equals(serverMemory.MaximumMemory.Value))
                {
                    WriteWarning("The specified maximum value is already the current running value.  No settings will be changed.");
                }
                else if (_force || ShouldProcess(conf.MaxServerMemory.DisplayName, "Change config value"))
                {
                    var changed = new PropertyChanged("MaxServerMemory");
                    changed.SetOldValue(serverMemory.ToString(MemoryProperty.MemType.Max, InUnitsOf));
                    conf.MaxServerMemory.ConfigValue = Convert.ToInt32(newProp.MaximumMemory.Value);
                    conf.Alter();
                    changed.SetNewValue(newProp.ToString(MemoryProperty.MemType.Max, InUnitsOf));
                    col.Add(changed);
                }
            }
            WriteObject(col.ToArray(), true);
        }
    }
}
