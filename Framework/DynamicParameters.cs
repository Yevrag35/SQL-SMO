using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Reflection;


namespace SQL.SMO.Framework
{
    internal class Dynamic
    {
        internal Dynamic() { }

        public static T Cast<T>(object o)
        {
            return (T)o;
        }

        internal static RuntimeDefinedParameterDictionary ToDictionary(params RuntimeDefinedParameter[] parameters)
        {
            var rtDict = new RuntimeDefinedParameterDictionary();
            for (int i = 0; i < parameters.Length; i++)
            {
                var rtParam = parameters[i];
                rtDict.Add(rtParam.Name, rtParam);
            }
            return rtDict;
        }

        internal RuntimeDefinedParameter Generate(string pName, string[] validatedItems, bool mandatory, int position = 0, string pSetName = null)
        {
            var atts = new Dictionary<string, object>()
            {
                { "Mandatory", mandatory },
                { "Position", position }
            };
            Type retType = mandatory ? typeof(string) : typeof(string[]);
            if (!string.IsNullOrEmpty(pSetName))
            {
                atts.Add("ParameterSetName", pSetName);
            }
            Collection<Attribute> attCol = CreateAttributes(atts);
            var valSet = new ValidateSetAttribute(validatedItems);
            attCol.Add(valSet);
            var rtParam = new RuntimeDefinedParameter(
                pName, retType, attCol
            );
            return rtParam;
        }

        internal protected Collection<Attribute> CreateAttributes(IDictionary hashtable)
        {
            var colAtt = new Collection<Attribute>();
            var pAtt = new ParameterAttribute();
            foreach (PropertyInfo info in pAtt.GetType().GetProperties())
            {
                foreach (string key in hashtable.Keys)
                {
                    if (info.Name == key)
                    {
                        MethodInfo castMethod = this.GetType().GetMethod("Cast").MakeGenericMethod(info.PropertyType);
                        object castedObject = castMethod.Invoke(null, new object[] { hashtable[key] });
                        info.SetValue(pAtt, castedObject, null);
                    }
                }
            }
            colAtt.Add(pAtt);
            return colAtt;
        }
    }
}
