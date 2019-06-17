using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace MG.Sql.Smo
{
    public class DataItemCollection<T> : IEnumerable<T> where T : DataRowClass
    {
        protected private const int NEG = -1;
        protected private List<T> _list;

        internal DataItemCollection(IEnumerable<T> items) => _list = new List<T>(items);
        internal DataItemCollection(DataTable dataTable)
        {
            _list = new List<T>(dataTable.Rows.Count);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                _list.Add(this.NewBlankItem(dataTable.Rows[i]));
            }
        }

        public T this[int index] => _list[index];

        public int Count => _list.Count;

        public T Find(Predicate<T> match) => _list.Find(match);
        public List<T> FindAll(Predicate<T> match) => _list.FindAll(match);

        private T NewBlankItem(DataRow dataRow) => (T)Activator.CreateInstance(typeof(T), new object[1] { dataRow });

        internal void Sort(IComparer<T> comparer) => _list.Sort(comparer);

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}
