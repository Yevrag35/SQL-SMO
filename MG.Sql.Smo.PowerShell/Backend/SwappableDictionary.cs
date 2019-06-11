using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;

namespace MG.Sql.Smo.PowerShell.Backend
{
    public class SwappableDictionary : Dictionary<string, object>
    {
        private const StringComparison CASESENSITIVE = StringComparison.CurrentCulture;
        private const string ERR_MSG = "A dictionary entry with the key \"{0}\" was not found.";

        #region CONSTRUCTORS
        public SwappableDictionary() : base() { }
        public SwappableDictionary(int capacity) : base(capacity) { }
        public SwappableDictionary(IDictionary<string, object> dictionary) : base(dictionary) {  }
        public SwappableDictionary(IEqualityComparer<string> comparer) : base(comparer) { }
        public SwappableDictionary(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer) { }

        public SwappableDictionary(IDictionary<string, object> dictionary, IEqualityComparer<string> comparer)
            : base(dictionary, comparer) {  }

        public SwappableDictionary(SerializationInfo serializationInfo, StreamingContext streamingContext) 
            : base(serializationInfo, streamingContext) { }

        #endregion

        #region ADDITIVE METHODS
        public void Add(KeyValuePair<string, object> kvp) => base.Add(kvp.Key, kvp.Value);
        public void Add(object key, object value)
        {
            string strKey = null;
            if (key is ValueType vt)
                strKey = Convert.ToString(vt);

            else if (key is string sk)
                strKey = sk;

            else
                throw new InvalidCastException(key.GetType().FullName + " cannot be converted to System.String.");

            base.Add(strKey, value);
        }
        public bool ContainsKey(string key, StringComparison comparison)
        {
            bool result = false;
            foreach (var kvp in this)
            {
                if (kvp.Key.Equals(key, comparison))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public bool TryGetKey(string key, StringComparison comparison, out string outKey)
        {
            bool result = false;
            outKey = null;
            foreach (var kvp in this)
            {
                if (kvp.Key.Equals(key, comparison))
                {
                    outKey = kvp.Key;
                    result = true;
                    break;
                }
            }
            return result;
        }
        public bool TryGetKvp(string key, StringComparison comparison, out KeyValuePair<string, object> outKvp)
        {
            bool result = false;
            outKvp = default;
            foreach (var kvp in this)
            {
                if (kvp.Key.Equals(key, comparison))
                {
                    outKvp = kvp;
                    result = true;
                    break;
                }
            }
            return result;
        }

        #endregion

        #region SWAPPABLE METHODS
        public void Swap(string newKey, StringComparison comparison = CASESENSITIVE)
        {
            if (this.TryGetKvp(newKey, comparison, out KeyValuePair<string, object> foundKvp))
            {
                var iswap = DictionarySwap.New(newKey, foundKvp);
                this.Swap(iswap);
            }
            else
                throw new KeyNotFoundException(string.Format(ERR_MSG, newKey));
        }

        public void Swap(IDictionarySwappable swapWith)
        {
            if (base.ContainsKey(swapWith.OldKey))
            {
                base.Remove(swapWith.OldKey);
                this.Add(swapWith.AsNewPair());
            }
            else
                throw new KeyNotFoundException(string.Format(ERR_MSG, swapWith.OldKey));
        }

        public IDictionarySwappable NewSwappable(string newKey, string oldKey, StringComparison comparison = CASESENSITIVE)
        {
            if (!base.ContainsKey(newKey) && this.TryGetKvp(oldKey, comparison, out KeyValuePair<string, object> outKvp))
                return DictionarySwap.New(newKey, outKvp);

            else
                throw new KeyNotFoundException(string.Format(ERR_MSG, oldKey));
        }

        #endregion
    }
}
