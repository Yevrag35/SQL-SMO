using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Config;
using SQL.SMO.Framework;
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
    }
}
