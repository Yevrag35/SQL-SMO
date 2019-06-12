﻿using MG.Dynamic;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "Database", ConfirmImpact = ConfirmImpact.None)]
    [OutputType(typeof(Database))]
    public class GetDatabase : GetDatabaseBase
    {
        protected override string Activity => null;
        protected override ICollection<string> Items => null;

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord() => WriteObject(base.RetrieveDatabases(), true);

        #endregion
    }
}