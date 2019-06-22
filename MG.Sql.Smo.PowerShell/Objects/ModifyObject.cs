using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell
{
    public class ModifyObject
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PROPERTIES
        public bool HasDefinition { get; internal set; }
        public string[] Add { get; internal set; }
        public string[] Remove { get; internal set; }
        //public string[] Replace { get; internal set; }
        public string[] Set { get; internal set; }

        #endregion

        #region CONSTRUCTORS
        private ModifyObject(string[] oneObj)
        {
            this.Set = oneObj;
        }
        private ModifyObject(Hashtable ht)
        {
            this.HasDefinition = true;
            if (ht.ContainsKey("Add"))
                this.Add = this.ConvertObjectToString(ht["Add"]);

            if (ht.ContainsKey("Remove"))
                this.Remove = this.ConvertObjectToString(ht["Remove"]);
        }

        #endregion

        #region METHODS
        public static implicit operator ModifyObject(object[] objs)
        {
            if (objs is string[] strs)
                return new ModifyObject(strs);

            else
                throw new InvalidCastException("Cannot cast object[] to string[].");
        }
        public static implicit operator ModifyObject(string[] str) => new ModifyObject(str);
        public static implicit operator ModifyObject(Hashtable ht) => new ModifyObject(ht);

        public string[] ConvertObjectToString(object obj)
        {
            var strs = new List<string>();
            if (obj is object[] objs)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    strs.Add(Convert.ToString(objs[i]));
                }
            }
            else if (obj is string oneStr)
                strs.Add(oneStr);

            return strs.ToArray();
        }

        #endregion
    }
}