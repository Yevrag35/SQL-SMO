using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Threading.Tasks;

namespace MG.Sql.Smo.PowerShell
{
    public abstract class BaseSqlCmdlet : PSCmdlet
    {
        internal const string NAME = "Name";
        internal const string NEW = "New";
        internal const string PROPERTIES = "Properties";
        internal const string REMOVE = "Remove";
        internal const string SET = "Set";
        internal static readonly Type STR_TYPE = typeof(string);
        internal static readonly Type STRARR_TYPE = typeof(string[]);
        internal const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.Instance;

        protected private bool NoEnd = false;
        protected private Server _server;

        protected override void BeginProcessing()
        {
            if (!SmoContext.IsSet || !SmoContext.IsConnected)
                throw new SmoContextNotSetException();
        }

        public void ChangeValues(object objectToSet, IDictionary newProps, IEnumerable<PropertyInfo> objProps)
        {
            object[] keys = newProps.Keys.Cast<object>().ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                object key = keys[i];
                PropertyInfo pi = objProps.Single(x => x.Name.Equals(key));
                pi.SetValue(objectToSet, newProps[key]);
            }
        }

        internal static T GetChosenValue<T>(string parameterName, RuntimeDefinedParameterDictionary rtDict) => 
            (T)rtDict[parameterName].Value;
        internal static T[] GetChosenValues<T>(string parameterName, RuntimeDefinedParameterDictionary rtDict) => 
            (T[])rtDict[parameterName].Value;

        protected private bool GetLoginFromName(string loginName, out SmoLogin outLogin)
        {
            bool contains = _server.Logins.Contains(loginName);
            outLogin = null;
            if (contains)
                outLogin = _server.Logins[loginName];

            return contains;
        }

        protected private bool NameMatchesPatterns(string name, params WildcardPattern[] pats)
        {
            bool result = false;
            for (int i = 0; i < pats.Length; i++)
            {
                if (pats[i].IsMatch(name))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        protected private WildcardPattern PatternFromName(string name) => new WildcardPattern(name, WildcardOptions.IgnoreCase);

        protected private WildcardPattern[] PatternsFromNames(string[] names)
        {
            var wcps = new WildcardPattern[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                wcps[i] = new WildcardPattern(names[i], WildcardOptions.IgnoreCase);
            }
            return wcps;
        }

        protected private void ThrowInnerException(Exception e)
        {
            while (e.InnerException != null)
            {
                e = e.InnerException;
            }
            throw e;
        }
    }
}