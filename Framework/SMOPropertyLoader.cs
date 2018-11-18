using MG.Attributes;
using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Config;
using SQL.SMO.Databases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SQL.SMO.Framework
{
    public abstract class SMOPropertyLoader : AttributeResolver, ISMOWrapper
    {
        #region Constants
        private const BindingFlags getProp = BindingFlags.GetProperty;
        private const BindingFlags setFlags = BindingFlags.NonPublic | BindingFlags.Instance;

        #endregion

        public SMOCollection<SMOPropertyError> LoadingErrors = new SMOCollection<SMOPropertyError>();

        internal protected PropertyInfo[] allPropInfo;
        internal static string[] GetPropertyNames<T>(T obj)
            where T : ISMOWrapper
        {
            var allProps = obj.OriginalType.GetProperties();
            var propNames = new List<string>(allProps.Length);
            for (int i = 0; i < allProps.Length; i++)
            {
                var prop = allProps[i];
                if (prop.Name != "Name")
                    propNames.Add(prop.Name);
            }
            return propNames.ToArray();
        }
        internal static string[] GetPropertyNames(Type t)
        {
            var allProps = t.GetProperties();
            var propNames = new List<string>(allProps.Length);
            for (int i = 0; i < allProps.Length; i++)
            {
                var prop = allProps[i];
                if (prop.Name != "Name")
                    propNames.Add(prop.Name);
            }
            return propNames.ToArray();
        }

        public abstract string Name { get; internal set; }
        public abstract Type OriginalType { get; }
        public abstract object ShowOriginal();

        #region Translate 'MS'Type to 'SMO'Type
        internal Type GetSMOType<T>() where T : SqlSmoObject
        {
            string name = typeof(T).Name;
            switch (name)     // Only add types here if they have an explicit operator from the 'case' type.
            {
                case "Database":
                    return typeof(SMODatabase);

                case "Table":
                    return typeof(SMOTable);

                case "Column":
                    return typeof(SMOColumn);

                case "ConfigProperty":
                    return typeof(SMOProperty);

                case "JobServer":
                    return typeof(SMOAgent);

                default:
                    return null;
            }
        }
        internal Type GetSMOCollectionType<T>() where T : SmoCollectionBase
        {
            string name = typeof(T).Name;
            switch (name)
            {
                case "DatabaseCollection":
                    return typeof(SMODatabaseCollection);

                case "JobCollection":
                    return typeof(SMOJobCollection);

                case "TableCollection":
                    return typeof(SMOTableCollection);

                case "ColumnCollection":
                    return typeof(SMOColumnCollection);

                default:
                    return null;
            }
        }

        public bool IsPropertyLoaded(string propName)
        {
            var thisType = this.GetType();
            bool result = false;
            if (allPropInfo == null)
                allPropInfo = thisType.GetProperties();

            for (int i = 0; i < allPropInfo.Length; i++)
            {
                var prop = allPropInfo[i];
                if (string.Equals(propName, prop.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = prop.CanRead && prop.GetValue(this) != null;
                    break;
                }
            }
            return result;
        }

        private bool ToSMOType(Type t, out Type returnType)
        {
            MethodInfo mi = this.GetType().GetMethod(
                "GetSMOType", setFlags).MakeGenericMethod(t);
            returnType = (Type)mi.Invoke(this, null);
            return returnType != null;
        }
        private bool ToSMOColType(Type t, out Type returnType)
        {
            MethodInfo mi = this.GetType().GetMethod(
                "GetSMOCollectionType", setFlags).MakeGenericMethod(t);
            returnType = (Type)mi.Invoke(this, null);
            return returnType != null;
        }

        #endregion

        #region The 'Load' Functionality
        public abstract void Load(params string[] propertyNames);

        internal void LoadValue<T>(T original, params string[] propertyNames) where T : SqlSmoObject
        {
            var thisType = this.GetType();
            var thatType = typeof(T);

            for (int i = 0; i < propertyNames.Length; i++)
            {
                var prop = propertyNames[i];
                var propInfo = thisType.GetProperty(prop);
                if (propInfo == null)
                {
                    if (allPropInfo == null)
                        allPropInfo = thisType.GetProperties();

                    for (int p = 0; p < allPropInfo.Length; p++)
                    {
                        var pi = allPropInfo[p];
                        if (string.Equals(pi.Name, prop, StringComparison.InvariantCultureIgnoreCase))
                        {
                            propInfo = pi;
                            break;
                        }
                    }
                    if (propInfo == null)
                        throw new ArgumentException(prop + " was not recognized as a valid property name for this object!");
                }
                object thatObj;
                try
                {
                    thatObj = thatType.InvokeMember(propInfo.Name, getProp, null, original, null);
                }
                catch (TargetInvocationException ex)
                {
                    LoadingErrors.Add(
                        new SMOPropertyError(
                            propInfo.Name, ex, typeof(T).GetProperty(propInfo.Name).PropertyType
                        )
                    );
                    thatObj = null;
                }
                if (thatObj != null)
                {
                    if (thatObj is SqlSmoObject && ToSMOType(thatObj.GetType(), out Type newType))
                    {
                        MethodInfo GenericCast = thisType.GetMethod(
                            "Cast", setFlags).MakeGenericMethod(newType);
                        thatObj = GenericCast.Invoke(this, new object[1] { thatObj });
                    }
                    else if (thatObj is SmoCollectionBase && ToSMOColType(thatObj.GetType(), out Type newColType))
                    {
                        MethodInfo GenericCast = thisType.GetMethod(
                            "Cast", setFlags).MakeGenericMethod(newColType);
                        thatObj = GenericCast.Invoke(this, new object[1] { thatObj });
                    }
                    propInfo.SetValue(this, thatObj, setFlags,
                        null, null, CultureInfo.CurrentCulture);
                }
            }
        }

        #endregion
    }
}
