using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTBot
{
    public class CacheDictionary : ICacheDictionary<string, Dictionary<string, string>>
    {
        private static Dictionary<string, Dictionary<string, string>> cache { get; }

        static CacheDictionary()
        {
            cache = new Dictionary<string, Dictionary<string, string>>();
        }

        public bool ContainsKey(string key)
        {
            return cache.ContainsKey(key);
        }

        public void AddValue(string key, Dictionary<string, string> value)
        {
            if (ContainsKey(key))
                cache[key] = value;
        }

        public void RemoveValue(string key)
        {
            cache.Remove(key);
        }

        public Dictionary<string, string> GetValue(string key)
        {
            if (ContainsKey(key))
                return cache[key];
            return null;
        }
    }
}
