using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;

namespace SQL.SMO.Config
{
    public class SMOProperty : ISMOWrapper
    {
        private ConfigProperty _prop;
        private readonly string goodName;
        public string Name => 
            string.IsNullOrEmpty(goodName) && _prop != null ? 
                _prop.DisplayName : !string.IsNullOrEmpty(goodName) ? 
                    goodName : null;

        public string RealName => _prop != null ? _prop.DisplayName : null;
        public string Description => _prop != null ? _prop.Description : null;
        public object RunningValue => _prop != null ? _prop.RunValue : (object)null;
        public object ConfigValue => _prop != null ? _prop.ConfigValue : (object)null;

        public bool? ConfigMatchesRunning => _prop != null ? 
            _prop.RunValue.Equals(_prop.ConfigValue) : (bool?)null;

        public bool IsDefined => _prop != null;

        public Type OriginalType => typeof(ConfigProperty);


        public SMOProperty(ConfigProperty prop) => _prop = prop;
        public SMOProperty(string gname, ConfigProperty prop)
        {
            _prop = prop;
            goodName = gname;
        }
        internal SMOProperty(string gname, object prop)
        {
            _prop = prop == null ? null : (ConfigProperty)prop;
            goodName = gname;
        }

        internal void Alter(object newVal)
        {
            _prop.ConfigValue = Convert.ToInt32(newVal);
            ((Server)Context.Connection).Configuration.Alter();
        }

        public void Load(params string[] names) => throw new NotImplementedException();
        public object ShowOriginal() => _prop;
    }
}