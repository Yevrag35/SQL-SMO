using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;

namespace SQL.SMO.Config
{
    public class SMOProperty : ISMOObject
    {
        private ConfigProperty _prop;
        private string goodName;
        public string Name
        {
            get
            {
                if (String.IsNullOrEmpty(goodName) && _prop != null)
                {
                    return _prop.DisplayName;
                }
                else if (!String.IsNullOrEmpty(goodName))
                {
                    return goodName;
                }
                else
                {
                    return null;
                }
            }
        }
        public string RealName
        {
            get { if (_prop != null) { return _prop.DisplayName; } else { return null; } }
        }
        public string Description
        {
            get { if (_prop != null) { return _prop.Description; } else { return null; } }
        }
        public object RunningValue
        {
            get { if (_prop != null) { return _prop.RunValue; } else { return null; } }
        }
        public object ConfigValue
        {
            get { if (_prop != null) { return _prop.ConfigValue; } else { return null; } }
        }
        public bool? ConfigMatchesRunning
        {
            get
            {
                if (_prop != null)
                {
                    return _prop.RunValue.Equals(_prop.ConfigValue);
                }
                else
                {
                    return null;
                }
            }
        }

        public bool IsDefined => _prop != null;

        public Type MSType => typeof(ConfigProperty);


        public SMOProperty(ConfigProperty prop)
        {
            _prop = prop;
        }
        public SMOProperty(string gname, ConfigProperty prop)
        {
            _prop = prop;
            goodName = gname;
        }
        internal SMOProperty(string gname, object prop)
        {
            if (prop == null)
            {
                _prop = null;
            }
            else
            {
                _prop = (ConfigProperty)prop;
            }
            goodName = gname;
        }

        internal void Alter(object newVal)
        {
            _prop.ConfigValue = Convert.ToInt32(newVal);
            ((Server)Context.Connection).Configuration.Alter();
        }

        public object ShowOriginal() => _prop;
    }
}