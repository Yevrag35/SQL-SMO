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

        private const string GETFIELD = "GetField";
        private const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Public;
        private MethodInfo FieldMethod => this.GetType().GetMethod(GETFIELD, FLAGS);

        public T GetField<T>(DataRow dataRow, DataColumn dataColumn) => dataRow.Field<T>(dataColumn);

        object IDataRowClass.GetGenericField(DataRow dataRow, DataColumn dataColumn)
        {
            MethodInfo mi = this.GetGenericCast(dataColumn.DataType);
            return mi.Invoke(this, new object[2] { dataRow, dataColumn });
        }

        private MethodInfo GetGenericCast(Type type) => FieldMethod.MakeGenericMethod(type);

        void IDataRowClass.SetProperties(IDictionary<string, object> props)
        {
            IEnumerable<PropertyInfo> thisProps = this.GetType().GetProperties().Where(x => x.CanWrite && props.Keys.Contains(x.Name));
            foreach (KeyValuePair<string, object> kvp in props)
            {
                foreach (PropertyInfo prop in thisProps)
                {
                    if (prop.Name.Equals(kvp.Key))
                    {
                        prop.SetValue(this, kvp.Value);
                        break;
                    }
                }
            }
        }
    }
}
