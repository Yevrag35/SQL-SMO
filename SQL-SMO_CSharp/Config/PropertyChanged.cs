using Microsoft.SqlServer.Management.Smo;
using System;
using System.Linq;
using SQL.SMO.Cmdlets;
using SQL.SMO.Framework;

namespace SQL.SMO.Config
{
    public class PropertyChanged
    {
        private object _old;
        private object _new;
        private readonly SMOProperty _cfgProp;

        public string Name => _cfgProp.Name;
        public object OldValue => _old;
        public object NewValue => _new;
        internal SMOProperty ConfigProperty => _cfgProp;

        public PropertyChanged(SMOProperty smoProperty , object oldValue, object newValue)
        {
            _old = oldValue;
            _new = newValue;
            _cfgProp = smoProperty;
        }

        internal PropertyChanged(SMOProperty smoProperty, object oldValue)
        {
            _cfgProp = smoProperty;
            _old = oldValue;
        }

        internal protected PropertyChanged(string property) => 
            _cfgProp = SharedCmdlet.GetPropertyValue(property);

        internal PropertyChanged(ConfigProperty configProperty) => 
            _cfgProp = new SMOProperty(configProperty);

        internal void SetOldValue(object value) => _old = value;
        internal void SetNewValue(object value) => _new = value;
    }
}
