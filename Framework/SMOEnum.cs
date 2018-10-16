using MG.Attributes;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Reflection;

namespace SQL.SMO.Framework
{

    public class CompatAttribute : MGAbstractAttribute
    {
        public CompatAttribute(CompatibilityLevel[] compat)
            : base(compat)
        {
        }
    }

    public enum CompatTable : int
    {
        [Compat(new CompatibilityLevel[1] { CompatibilityLevel.Version90 })]
        [MGName("Microsoft SQL Server 2005")]
        SQL2005 = 0,

        [Compat(new CompatibilityLevel[1] { CompatibilityLevel.Version100 })]
        [MGName("Microsoft SQL Server 2008")]
        SQL2008 = 1,

        [Compat(new CompatibilityLevel[1] { CompatibilityLevel.Version110 })]
        [MGName("Microsoft SQL Server 2012")]
        SQL2012 = 2,

        [Compat(new CompatibilityLevel[1] { CompatibilityLevel.Version120 })]
        [MGName("Microsoft SQL Server 2014")]
        SQL2014 = 3,

        [Compat(new CompatibilityLevel[1] { CompatibilityLevel.Version130 })]
        [MGName("Microsoft SQL Server 2016")]
        SQL2016 = 4,

        [Compat(new CompatibilityLevel[1] { CompatibilityLevel.Version140 })]
        [MGName("Microsoft SQL Server 2017")]
        SQL2017 = 5
    }
}
