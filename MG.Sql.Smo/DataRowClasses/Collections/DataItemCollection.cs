using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace MG.Sql.Smo
{
    public class DataItemCollection<T> : IFindable<T>, ISortable<T> where T : DataRowClass
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
        public IFindable<T> FindAll(Predicate<T> match) => (DataItemCollection<T>)_list.FindAll(match);

        private T NewBlankItem(DataRow dataRow) => (T)Activator.CreateInstance(typeof(T), new object[1] { dataRow });

        public void Sort(IComparer<T> comparer) => _list.Sort(comparer);
        public T[] ToArray()
        {
            var tArr = new T[_list.Count];
            _list.CopyTo(tArr);
            return tArr;
        }
        public List<T> ToList() => _list;

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        public static explicit operator DataItemCollection<T>(List<T> list) => new DataItemCollection<T>(list);
    }
}
