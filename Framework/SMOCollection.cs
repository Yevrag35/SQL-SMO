using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SQL.SMO.Framework
{
    public class SMOCollection<T> : IList<T>, ICollection where T : ISMOWrapper
    {
        private protected List<T> _list;

        #region Constructors
        internal protected SMOCollection()
        {
            _list = new List<T>();
            IsReadOnly = false;
        }

        internal protected SMOCollection(int capacity)
        {
            _list = new List<T>(capacity);
            IsReadOnly = false;
        }

        internal protected SMOCollection(IEnumerable<T> errors)
        {
            _list = new List<T>(errors);
            IsReadOnly = false;
        }

        internal protected SMOCollection(T oneError)
            : this(((IEnumerable)oneError).Cast<T>().ToArray())
        {
        }

        #endregion

        #region IList and ICollection Methods

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public int Count => _list.Count;

        public bool IsReadOnly { get; private set; }
        public object SyncRoot => ((ICollection)_list).SyncRoot;
        public bool IsSynchronized => ((ICollection)_list).IsSynchronized;

        public void Add(T item)
        {
            if (!IsReadOnly)
                _list.Add(item);
            else
                throw new ReadOnlyCollectionException("Add", item);
        }

        public void Clear()
        {
            if (!IsReadOnly)
                _list.Clear();
            else
                throw new ReadOnlyCollectionException("Clear");
        }

        public bool Contains(T item) => _list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (!IsReadOnly)
                _list.CopyTo(array, arrayIndex);
            else
                throw new ReadOnlyCollectionException("CopyTo", array, arrayIndex);
        }
        public void CopyTo(Array array, int index)
        {
            if (!IsReadOnly)
                ((ICollection)_list).CopyTo(array, index);
            else
                throw new ReadOnlyCollectionException("CopyTo", array, index);
        }

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        public int IndexOf(T item) => _list.IndexOf(item);

        public void Insert(int index, T item)
        {
            if (!IsReadOnly)
                _list.Insert(index, item);
            else
                throw new ReadOnlyCollectionException("Insert", index, item);
        }

        public bool Remove(T item)
        {
            if (IsReadOnly)
                throw new ReadOnlyCollectionException("Remove", item);
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            if (!IsReadOnly)
                _list.RemoveAt(index);
            else
                throw new ReadOnlyCollectionException("RemoveAt", index);
        }

        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        #endregion

        #region Custom Methods
        internal protected void MakeReadOnly() => this.IsReadOnly = true;

        internal protected void LoadProperties(params string[] propertyNames)
        {
            for (int i = 0; i < this.Count; i++)
            {
                var obj = this[i];
                obj.Load(propertyNames);
            }
        }

        public void AddRange(IEnumerable<T> tableCol) =>
            _list.AddRange(tableCol);

        public T[] ToArray() => _list.ToArray();

        #endregion
    }
}
