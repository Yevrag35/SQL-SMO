using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MG.Sql
{
    public abstract class BaseSmoObject
    {
        private SqlSmoObject _orig;
        private static readonly MethodInfo CastMethod = typeof(BaseSmoObject).GetMethod("Cast", BindingFlags.NonPublic | BindingFlags.Static);

        protected abstract string[] PropertiesToLoad { get; }

        public BaseSmoObject() { }

        private static T Cast<T>(dynamic o) => (T)o;
    }
}
