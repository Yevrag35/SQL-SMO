using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.Sql.Smo
{
    public interface ISortable<T> : IEnumerable<T>
    {
        T this[int index] { get; }
        int Count { get; }

        void Sort(IComparer<T> comparer);
    }
}
