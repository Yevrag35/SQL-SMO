using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG.Sql.Smo
{
    public class MgSmoCollection<T> : IFindable<T>, ISortable<T>, IList<T> where T : SqlSmoObject
    {
        private List<T> _list;

        public MgSmoCollection() => _list = new List<T>();
        public MgSmoCollection(int capacity) => _list = new List<T>(capacity);
        public MgSmoCollection(IEnumerable<T> items) => _list = new List<T>(items);
        public MgSmoCollection(SmoCollectionBase smoColBase)
            : this(smoColBase.Count) => this.AddRange(smoColBase);

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public void Add(T item) => _list.Add(item);
        public void AddRange(IEnumerable<T> items) => _list.AddRange(items);
        public void AddRange(SmoCollectionBase smoCol)
        {
            try
            {
                foreach (T tItem in smoCol)
                {
                    _list.Add(tItem);
                }
            }
            catch (ExecutionFailureException exe)
            {
                this.ThrowInnerException(exe);
            }
            catch (FailedOperationException foe)
            {
                this.ThrowInnerException(foe);
            }
        }
        public void Clear() => _list.Clear();
        public bool Contains(T item) => _list.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        public T Find(Predicate<T> match) => _list.Find(match);
        public IFindable<T> FindAll(Predicate<T> match) => (MgSmoCollection<T>)_list.FindAll(match);
        public T GetItemById(object id, string idProperty = null)
        {
            T obj = null;
            if (_list.Count > 0)
            {
                if (string.IsNullOrEmpty(idProperty))
                    idProperty = "ID";

                PropertyInfo propId = typeof(T).GetProperty(idProperty, BindingFlags.Public | BindingFlags.Instance);
                if (propId != null)
                {

                    for (int i = 0; i < _list.Count; i++)
                    {
                        T item = _list[i];
                        object idVal = propId.GetValue(item);
                        if (idVal != null && idVal.Equals(id))
                        {
                            obj = item;
                            break;
                        }
                    }
                }
            }
            return obj;
        }

        public int IndexOf(T item) => _list.IndexOf(item);
        public void Insert(int index, T item) => _list.Insert(index, item);
        public bool Remove(T item) => _list.Remove(item);
        public void RemoveAt(int index) => _list.RemoveAt(index);
        public void Sort(IComparer<T> comparer) => _list.Sort(comparer);

        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        private void ThrowInnerException(Exception e)
        {
            while (e.InnerException != null)
            {
                e = e.InnerException;
            }
            throw e;
        }

        public bool TryFind(Predicate<T> match, out T matched)
        {
            bool result = false;
            matched = _list.Find(match);
            if (matched != null)
            {
                result = true;
            }
            return result;
        }


        public static implicit operator MgSmoCollection<T>(List<T> list) => new MgSmoCollection<T>(list);
        public static explicit operator List<T>(MgSmoCollection<T> mgSmo) => mgSmo._list;

        public static MgSmoCollection<T> FromSmoCollection(SmoCollectionBase colBase) => new MgSmoCollection<T>(colBase);
    }
}
