using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MG.Sql.Smo.PowerShell.Backend
{
    internal class DictionarySwap : IDictionarySwappable
    {
        string IDictionarySwappable.NewKey { get; set; }
        string IDictionarySwappable.OldKey { get; set; }
        object IDictionarySwappable.Value { get; set; }

        //internal DictionarySwap() { }
        private DictionarySwap(string newKey, KeyValuePair<string, object> oldKvp)
        {
            ((IDictionarySwappable)this).NewKey = newKey;
            ((IDictionarySwappable)this).OldKey = oldKvp.Key;
            ((IDictionarySwappable)this).Value = oldKvp.Value;
        }

        KeyValuePair<string, object> IDictionarySwappable.AsNewPair()
        {
            return new KeyValuePair<string, object>(((IDictionarySwappable)this).NewKey, ((IDictionarySwappable)this).Value);
        }

        internal static IDictionarySwappable New(string newKey, KeyValuePair<string, object> oldKvp) => new DictionarySwap(newKey, oldKvp);
    }
}
