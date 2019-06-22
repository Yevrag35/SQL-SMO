using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell
{
    public class NumStrIdentity
    {
        #region FIELDS/CONSTANTS
        private int? _backingId;
        private readonly string _backingStr;

        #endregion

        #region PROPERTIES
        public bool IsNumeric { get; }
        public bool IsString { get; }

        #endregion

        #region CONSTRUCTORS
        private NumStrIdentity(int id)
        {
            this.IsNumeric = true;
            _backingId = id;
        }
        private NumStrIdentity(string strId)
        {
            this.IsString = true;
            _backingStr = strId;
        }

        #endregion

        #region METHODS
        public static implicit operator NumStrIdentity(int intId) => new NumStrIdentity(intId);
        public static implicit operator NumStrIdentity(string str) => new NumStrIdentity(str);
        public static implicit operator NumStrIdentity(long longId) => new NumStrIdentity(Convert.ToInt32(longId));
        public static implicit operator NumStrIdentity(short shortId) => new NumStrIdentity(Convert.ToInt32(shortId));
        public static implicit operator NumStrIdentity(byte byteId) => new NumStrIdentity(Convert.ToInt32(byteId));
        public static explicit operator int(NumStrIdentity identity) => identity._backingId.Value;
        public static explicit operator string(NumStrIdentity identity) => identity._backingStr;

        #endregion
    }
}