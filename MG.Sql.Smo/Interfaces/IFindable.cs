using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.Sql.Smo
{
    public interface IFindable<T> : IEnumerable<T>
    {
        T this[int index] { get; }
        int Count { get; }

        T Find(Predicate<T> match);
        IFindable<T> FindAll(Predicate<T> match);
    }
}
