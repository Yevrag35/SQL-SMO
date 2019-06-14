using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

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

        protected override void BeginProcessing()
        {
            if (!SmoContext.IsSet || !SmoContext.IsConnected)
                throw new SmoContextNotSetException();
        }

        internal static T GetChosenValue<T>(string parameterName, RuntimeDefinedParameterDictionary rtDict) => 
            (T)rtDict[parameterName].Value;
        internal static T[] GetChosenValues<T>(string parameterName, RuntimeDefinedParameterDictionary rtDict) => 
            (T[])rtDict[parameterName].Value;

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
    }
}