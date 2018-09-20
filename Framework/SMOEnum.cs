using MG.Attributes;
using MG.Attributes.Interfaces;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Reflection;

namespace SQL.SMO.Framework
{

    public class CompatAttribute : Attribute, IAttribute
    {
        private string _n;
        public object Value { get { return _n; } }

        public CompatAttribute(string value)
        {
            _n = value;
        }
    }

    public enum CompatTable : int
    {
        [Compat("Version90")]
        [MGName("Microsoft SQL Server 2005")]
        SQL2005 = 0,

        [Compat("Version100")]
        [MGName("Microsoft SQL Server 2008")]
        SQL2008 = 1,

        [Compat("Version110")]
        [MGName("Microsoft SQL Server 2012")]
        SQL2012 = 2,

        [Compat("Version120")]
        [MGName("Microsoft SQL Server 2014")]
        SQL2014 = 3,

        [Compat("Version130")]
        [MGName("Microsoft SQL Server 2016")]
        SQL2016 = 4,

        [Compat("Version140")]
        [MGName("Microsoft SQL Server 2017")]
        SQL2017 = 5
    }
}
