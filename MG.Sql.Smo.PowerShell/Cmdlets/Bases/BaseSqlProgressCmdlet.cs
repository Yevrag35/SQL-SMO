using MG.Dynamic;
using MG.Progress.PowerShell;
using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Threading.Tasks;

namespace MG.Sql.Smo.PowerShell
{
    public abstract class BaseSqlProgressCmdlet : ProgressCmdlet
    {
        //internal const string NAME = "Name";
        //internal const string PROPERTIES = "Properties";
        //internal static readonly Type STR_TYPE = typeof(string);
        //internal static readonly Type STRARR_TYPE = typeof(string[]);
        //internal const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.Instance;

        protected private Server _server;

        protected override void BeginProcessing()
        {
            if (!SmoContext.IsSet || !SmoContext.IsConnected)
                throw new SmoContextNotSetException();
        }
    }
}
