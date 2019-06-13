using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace MG.Sql.Smo
{
    #region COLUMN
    public class ColumnEquality : IEqualityComparer<Column>
    {
        public bool Equals(Column x, Column y) => StaticEquals(x, y);

        internal static bool StaticEquals(Column x, Column y)
        {
            bool result = false;
            if (x.Parent is Table tabA && y.Parent is Table tabB && TableEquality.StaticEquals(tabA, tabB))
            {
                result = x.Name.Equals(y.Name);
            }

            return result;
        }

        int IEqualityComparer<Column>.GetHashCode(Column obj) => 0;
    }

    #endregion

    #region DATABASE
    public class DatabaseEquality : IEqualityComparer<Database>
    {
        public bool Equals(Database x, Database y) => StaticEquals(x, y);

        internal static bool StaticEquals(Database x, Database y)
        {
            bool result = false;
            if (x.Parent.Name.Equals(y.Parent.Name))
            {
                result = x.ID.Equals(y.ID);
            }
            return result;
        }

        int IEqualityComparer<Database>.GetHashCode(Database obj) => 0;
    }

    #endregion

    #region TABLE
    public class TableEquality : IEqualityComparer<Table>
    {
        public bool Equals(Table x, Table y) => StaticEquals(x, y);
        
        internal static bool StaticEquals(Table x, Table y)
        {
            bool result = false;
            if (DatabaseEquality.StaticEquals(x.Parent, y.Parent))
            {
                result = x.Name.Equals(y.Name);
            }

            return result;
        }

        int IEqualityComparer<Table>.GetHashCode(Table obj) => 0;
    }

    #endregion
}