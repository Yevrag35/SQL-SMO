using System;
using System.Collections.Generic;

namespace MG.Sql.Smo.PowerShell.Backend
{
    public interface IDictionarySwappable
    {
        string NewKey { get; set; }
        string OldKey { get; set; }
        object Value { get; set; }

        KeyValuePair<string, object> AsNewPair();
    }
}
