using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell
{
    public abstract class BaseSqlCmdlet : PSCmdlet
    {
        internal const string NAME = "Name";
        internal const string PROPERTIES = "Properties";
        internal static readonly Type STR_TYPE = typeof(string);
        internal static readonly Type STRARR_TYPE = typeof(string[]);
        internal const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.Instance;

        internal static string pName;
        internal static Type pType;
        internal static Collection<Attribute> attCol;

        protected override void BeginProcessing()
        {
            if (!SMOContext.IsSet || !SMOContext.IsConnected)
                throw new SmoContextNotSetException();
        }

        internal static RuntimeDefinedParameterDictionary GetRTDictionary(string[] values)
        {
            attCol.Add(new ValidateSetAttribute(values));
            var rtParam = new RuntimeDefinedParameter(pName, pType, attCol);
            return new RuntimeDefinedParameterDictionary
            {
                { pName, rtParam }
            };
        }

        internal static T GetChosenValue<T>(string parameterName, RuntimeDefinedParameterDictionary rtDict) => 
            (T)rtDict[parameterName].Value;
        internal static T[] GetChosenValues<T>(string parameterName, RuntimeDefinedParameterDictionary rtDict) => 
            (T[])rtDict[parameterName].Value;
    }
}