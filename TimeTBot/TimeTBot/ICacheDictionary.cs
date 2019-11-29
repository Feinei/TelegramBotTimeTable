using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTBot
{
    public interface ICacheDictionary<TKey, TValue>
    {
        bool ContainsKey(TKey key);
        void AddValue(TKey key, TValue value);
        void RemoveValue(TKey key);
        TValue GetValue(TKey key);
    }
}
