using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Config;
using SQL.SMO.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace SQL.SMO.Cmdlets
{
    public abstract class SharedCmdlet : PSCmdlet, IDynamicParameters
    {
        internal const string pName = "Property";
        internal const string dName = "Database";
        internal RuntimeDefinedParameterDictionary _source;

        #region Dynamic Parameters
        public object GetDynamicParameters() => CheckSession(true) ? GenerateFor() : null;

        internal abstract RuntimeDefinedParameterDictionary GenerateFor();

        #endregion

        internal static void CheckSession()
        {
            if (!Context.IsSet && !Context.IsConnected)
            {
                throw new SMOContextNotSetException();
            }
        }
        internal static bool CheckSession(bool Is = true) => 
            Context.IsSet.Equals(Is) && Context.IsConnected.Equals(Is) ? true : false;

        internal protected static SMOProperty GetPropertyValue(string propName)
        {
            SMOProperty propval = null;
            // Try and get the property value
            try
            {
                object trytoget = typeof(Configuration).InvokeMember(propName, BindingFlags.GetProperty,
                    null, ((Server)Context.Connection).Configuration, null);
                propval = new SMOProperty(propName, trytoget);
            }
            catch (TargetInvocationException)
            {
                propval = new SMOProperty(propName, null);
            }
            return propval;
        }

        #region Cmdlet Loads

        private protected void LoadWithDynamic(string paramName, ISMOWrapper wrapper)
        {
            var addProps = _source[paramName].Value;
            string[] propNames = ((IEnumerable)addProps).Cast<string>().ToArray();
            wrapper.Load(propNames);
        }

        internal protected void LoadWithExplicit(string[] props, string[] references, ISMOWrapper wrapper)
        {
            var psToLoad = new List<string>();

            var wco = WildcardOptions.IgnoreCase;
            for (int i = 0; i < props.Length; i++)
            {
                var p = props[i];
                var wcp = new WildcardPattern(p, wco);
                for (int t = 0; t < references.Length; t++)
                {
                    var name = references[t];
                    if (wcp.IsMatch(name))
                        psToLoad.Add(name);
                }
            }
            wrapper.Load(psToLoad.ToArray());
        }

        #endregion
    }
}
