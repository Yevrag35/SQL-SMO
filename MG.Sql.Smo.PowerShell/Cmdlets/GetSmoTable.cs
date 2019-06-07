using MG.Dynamic;
using MG.Progress.PowerShell;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell.Cmdlets
{
    public class GetSmoTable : GetSmoDatabaseBase
    {
        private List<Table> _tables;
        protected override string Activity => "SQL Table Retrieval";
        protected override ICollection<string> Items => _tables.Select(x => x.Name).ToArray();
    }
}
