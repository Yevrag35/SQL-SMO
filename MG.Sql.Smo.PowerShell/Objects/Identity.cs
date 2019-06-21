using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell.Objects
{
    public class Identity
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
        private Identity(int id)
        {
            this.IsNumeric = true;
            _backingId = id;
        }
        private Identity(string strId)
        {
            this.IsString = true;
            _backingStr = strId;
        }

        #endregion

        #region METHODS
        public static implicit operator Identity(int intId) => new Identity(intId);
        public static implicit operator Identity(string str) => new Identity(str);
        public static explicit operator int 

        #endregion
    }
}