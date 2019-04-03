using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.Sql
{
    public class SmoPropertyCollection : ICollection
    {
        private ICollection<Property> _col;

        public SmoPropertyCollection() => _col = new List<Property>();

        private SmoPropertyCollection(SqlPropertyCollection sqlPropCol)
        {
            _col = new List<Property>();
            foreach (Property prop in sqlPropCol)
            {
                _col.Add(prop);
            }
        }

        public int Count => _col.Count;

        object ICollection.SyncRoot => ((ICollection)_col).SyncRoot;

        bool ICollection.IsSynchronized => ((ICollection)_col).IsSynchronized;

        void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => _col.GetEnumerator();

        public static explicit operator SmoPropertyCollection(SqlPropertyCollection sqlPropCol) => new SmoPropertyCollection(sqlPropCol);
    }
}
