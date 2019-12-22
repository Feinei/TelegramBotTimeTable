using System.Collections.Generic;

namespace TimeTBot
{
    public abstract class BaseCommandHandler
    {
        internal ICacheDictionary<string, Dictionary<string, string>> cacheDictionary;
        internal Command[] commands;

        public BaseCommandHandler(ICacheDictionary<string, Dictionary<string, string>> cacheDictionary, Command[] commands)
        {
            this.cacheDictionary = cacheDictionary;
            this.commands = commands;
        }

        public abstract string Execute(string id, string message);
    }
}
