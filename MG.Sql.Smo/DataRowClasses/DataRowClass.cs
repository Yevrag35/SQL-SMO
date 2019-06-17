using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace MG.Sql.Smo
{
    public class DataRowClass : IDataRowClass
    {
        internal DataRowClass(DataRow dataRow)
        {
            var dict = new Dictionary<string, object>();
            foreach (DataColumn col in dataRow.Table.Columns)
            {
                dict.Add(col.ColumnName, ((IDataRowClass)this).GetGenericField(dataRow, col));
            }
            ((IDataRowClass)this).SetProperties(dict);
        }

        private const string CAST = "Cast";
        private const string GETFIELD = "GetField";
        private const string STRS_TO_ENUM = "StringsToEnum";
        private const string STRS_TO_ENUMS = "StringsToEnums";
        private const BindingFlags PUB_FLAGS = BindingFlags.Instance | BindingFlags.Public;
        private const BindingFlags PRIV_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;
        private MethodInfo CastMethod => typeof(DataRowClass).GetMethod(CAST, PRIV_FLAGS);
        private MethodInfo FieldMethod => this.GetType().GetMethod(GETFIELD, PUB_FLAGS);
        private MethodInfo StringsToEnumMethod => typeof(DataRowClass).GetMethod(STRS_TO_ENUM, PRIV_FLAGS);
        private MethodInfo StringsToEnumsMethod => typeof(DataRowClass).GetMethod(STRS_TO_ENUMS, PRIV_FLAGS);

        private T Cast<T>(dynamic o) => (T)o;

        public T GetField<T>(DataRow dataRow, DataColumn dataColumn) => dataRow.Field<T>(dataColumn);
        private string[] ObjToStrings(object obj)
        {
            string[] arr = null;
            if (obj != null)
            {
                if (obj is string[] strArr)
                    arr = strArr;

                else if (obj is string oneStr && !string.IsNullOrEmpty(oneStr))
                    arr = new string[1] { oneStr };

            }
            return arr;
        }
        private T StringsToEnum<T>(string[] strs)
        {
            string oneStr = string.Join(",", strs);
            return (T)Enum.Parse(typeof(T), oneStr);
        }
        private T[] StringsToEnums<T>(string[] strs)
        {
            T[] tArr = new T[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                tArr[i] = (T)Enum.Parse(typeof(T), strs[i]);
            }
            return tArr;
        }

        object IDataRowClass.GetGenericField(DataRow dataRow, DataColumn dataColumn)
        {
            MethodInfo mi = this.GetGenericFieldMethod(dataColumn.DataType);
            return mi.Invoke(this, new object[2] { dataRow, dataColumn });
        }

        private MethodInfo GetGenericCast(Type type) => CastMethod.MakeGenericMethod(type);
        private MethodInfo GetGenericFieldMethod(Type type) => FieldMethod.MakeGenericMethod(type);
        private MethodInfo GetGenericStrToEnum(Type type, bool isArray)
        {
            return !isArray 
                ? StringsToEnumMethod.MakeGenericMethod(type) 
                : StringsToEnumsMethod.MakeGenericMethod(type);
        }

        void IDataRowClass.SetProperties(IDictionary<string, object> props)
        {
            IEnumerable<PropertyInfo> thisProps = this.GetType().GetProperties().Where(x => x.CanWrite && props.Keys.Contains(x.Name));
            foreach (KeyValuePair<string, object> kvp in props)
            {
                foreach (PropertyInfo prop in thisProps)
                {
                    if (prop.Name.Equals(kvp.Key))
                    {
                        object val = kvp.Value;
                        Type check = Nullable.GetUnderlyingType(prop.PropertyType);
                        Type realType = check == null
                            ? prop.PropertyType
                            : check;

                        if (realType.IsEnum)
                        {
                            if (val is int num)
                            {
                                MethodInfo castMeth = this.GetGenericCast(realType);
                                val = castMeth.Invoke(this, new object[1] { kvp.Value });
                                prop.SetValue(this, val);
                            }
                            else if (val is string[] strs)
                            {
                                MethodInfo genMeth = this.GetGenericStrToEnum(realType, realType.IsArray);
                                val = genMeth.Invoke(this, new object[1] { strs });
                                prop.SetValue(this, val);
                            }
                            else if (val is string oneStr && !string.IsNullOrEmpty(oneStr))
                            {
                                MethodInfo genMeth = this.GetGenericStrToEnum(realType, realType.IsArray);
                                string[] strArr = new string[1] { oneStr };
                                val = genMeth.Invoke(this, new object[1] { strArr });
                                prop.SetValue(this, val);
                            }
                        }
                        else if (val is string str && !string.IsNullOrEmpty(str))
                            prop.SetValue(this, str);

                        else if (val != null)
                            prop.SetValue(this, val);

                        break;
                    }
                }
            }
        }
    }
}
