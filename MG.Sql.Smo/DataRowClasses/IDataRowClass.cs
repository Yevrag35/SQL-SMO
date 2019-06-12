using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MG.Sql.Smo
{
    public interface IDataRowClass
    {
        T GetField<T>(DataRow dataRow, DataColumn dataColumn);
        object GetGenericField(DataRow dataRow, DataColumn dataColumn);

        void SetProperties(IDictionary<string, object> properties);
    }
}
