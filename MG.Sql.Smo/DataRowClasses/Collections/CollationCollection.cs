using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MG.Sql.Smo
{
    public class CollationCollection : DataItemCollection<Collation>, ICloneable
    {
        private CollationCollection(IEnumerable<Collation> items) : base(items) { }
        private CollationCollection(DataTable dataTable) : base(dataTable) { }

        public object Clone()
        {
            return new CollationCollection(_list);
        }

        public CollationCollection FindByLocaleID(int localeID)
        {
            var newCol = new CollationCollection(_list);
            for (int i = newCol.Count - 1; i >= 0; i--)
            {
                Collation c = newCol[i];
                if (!c.LocaleID.Equals(localeID))
                    newCol.Remove(c);
            }
            return newCol;
        }

        public void SortByName(bool ascending = false)
        {
            if (!ascending)
                _list.Sort(new CollationNameComparer());

            else
                _list.Sort(new CollationNameAscendingComparer());
        }
        public void SortByLocaleID() => _list.Sort(new CollationLocaleIDComparer());
        public void SortByVersion() => _list.Sort(new CollationVersionComparer());

        #region COMPARERS

        private class CollationNameComparer : IComparer<Collation>
        {
            public int Compare(Collation x, Collation y) => x.Name.CompareTo(y.Name);
        }
        private class CollationNameAscendingComparer : IComparer<Collation>
        {
            public int Compare(Collation x, Collation y) => x.Name.CompareTo(y.Name) * NEG;
        }
        private class CollationLocaleIDComparer : IComparer<Collation>
        {
            public int Compare(Collation x, Collation y) => x.LocaleID.CompareTo(y.LocaleID);
        }
        private class CollationVersionComparer : IComparer<Collation>
        {
            public int Compare(Collation x, Collation y) => x.CollationVersion.CompareTo(y.CollationVersion);
        }

        public void Remove(Collation collation) => _list.Remove(collation);

        #endregion

        public static CollationCollection GetCollations(Server server) => new CollationCollection(server.EnumCollations());
        public static CollationCollection GetCollations(Server server, int localeID)
        {
            var col = GetCollations(server);
            for (int i = col.Count - 1; i >= 0; i--)
            {
                Collation c = col[i];
                if (!c.LocaleID.Equals(localeID))
                    col.Remove(c);
            }
            return col;
        }
        public static CollationCollection GetCollations(Server server, CollationVersion version)
        {
            var col = GetCollations(server);
            for (int i = col.Count - 1; i >= 0; i--)
            {
                Collation c = col[i];
                if (!c.CollationVersion.Equals(version))
                    col.Remove(c);
            }
            return col;
        }
        public static CollationCollection GetCollations(Server server, int localeID, CollationVersion version)
        {
            var col = GetCollations(server);
            for (int i = col.Count - 1; i >= 0; i--)
            {
                Collation c = col[i];
                if (!c.LocaleID.Equals(localeID) || !c.CollationVersion.Equals(version))
                    col.Remove(c);
            }
            return col;
        }
    }
}