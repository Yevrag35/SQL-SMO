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

        //public static T Translate<T>(SqlSmoObject smoObj) where T : BaseSmoObject
        //{
        //    T newObj = Activator.CreateInstance<T>();
        //    IEnumerable<PropertyInfo> tProps = typeof(T).GetProperties(
        //        (BindingFlags)52).Where(
        //            x => x.CanWrite);

        //    foreach (Property prop in smoObj.Properties)
        //    {

        //    }
        //}

        private static T Cast<T>(dynamic o) => (T)o;
    }
}
