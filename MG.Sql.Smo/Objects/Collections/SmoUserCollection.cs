using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG.Sql.Smo
{
    public class SmoUserCollection : IFindable<SmoUser>
    {
        #region FIELDS/CONSTANTS
        private List<SmoUser> _list;

        #endregion

        #region PROPERTIES
        public int Count => _list.Count;

        #endregion

        #region INDEXERS
        public SmoUser this[int index] => _list[index];

        #endregion

        #region CONSTRUCTORS
        public SmoUserCollection() => _list = new List<SmoUser>();
        public SmoUserCollection(int capacity) => _list = new List<SmoUser>(capacity);
        public SmoUserCollection(IEnumerable<SmoUser> smoUsers) => _list = new List<SmoUser>(smoUsers);
        public SmoUserCollection(IEnumerable<User> sqlUsers)
        {
            _list = new List<SmoUser>();
            foreach (User u in sqlUsers)
            {
                _list.Add(u);
            }
        }

        #endregion

        #region METHODS
        public SmoUser Find(Predicate<SmoUser> match) => _list.Find(match);
        public IFindable<SmoUser> FindAll(Predicate<SmoUser> match) => (SmoUserCollection)_list.FindAll(match);
        public IEnumerator<SmoUser> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        #endregion

        #region OPERATORS/CASTS
        public static explicit operator SmoUserCollection(List<SmoUser> userList) => new SmoUserCollection(userList);

        #endregion
    }
}