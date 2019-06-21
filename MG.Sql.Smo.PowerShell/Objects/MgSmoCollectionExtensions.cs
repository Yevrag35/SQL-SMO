using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell.Extensions
{
    public static class MgSmoCollectionExtensions
    {
        private const BindingFlags PUB_INST = BindingFlags.Instance | BindingFlags.Public;

        public static void WildcardMatch<T>(this MgSmoCollection<T> col, string propertyName, string propertyValue) where T : SqlSmoObject
        {
            var wcp = new WildcardPattern(propertyValue, WildcardOptions.IgnoreCase);
            var list = new MgSmoCollection<T>(col.Count);
            PropertyInfo pi = typeof(T).GetProperty(propertyName, PUB_INST);
            if (pi == null)
                throw new ArgumentException("A property named " + propertyName + " could not be found.");

            for (int i = 0; i < col.Count; i++)
            {
                T item = col[i];
                string piVal = pi.GetValue(col[i]) as string;
                if (!wcp.IsMatch(piVal))
                    col.Remove(item);
            }
        }
    }
}