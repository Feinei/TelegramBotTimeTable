using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTBot
{
    class CommandsHandler : IHandler
    {
        private Dictionary<string, Command> allCommands;
        private ICacheDictionary<string, Dictionary<string, string>> cacheDictionary;

        public CommandsHandler(ICacheDictionary<string, Dictionary<string, string>> cacheDictionary, Command[] commands)
        {
            this.cacheDictionary = cacheDictionary;
            foreach (var command in commands)
                allCommands.Add(command.Name, command);
        }

        public string Execute(string id, string message, string commandName = null)
        {        
            // Execute previous command (if exists)
            if (cacheDictionary.ContainsKey(id) && cacheDictionary.GetValue(id).ContainsKey("next"))
            {
                if (commandName != null && allCommands[commandName].Contains(cacheDictionary.GetValue(id)["next"]))
                {
                    return allCommands[commandName].Execute(id, message);
                }
                else
                {
                    foreach (var command in allCommands.Values)
                        if (command.Contains(cacheDictionary.GetValue(id)["next"]))
                            return command.Execute(id, message);
                }
            }

            // Execute command by user message (if previous command is null)
            if (commandName != null)
            {
                return allCommands[commandName].Execute(id, message);
            }
            else
            {
                foreach (var command in allCommands.Values)
                    if (command.Contains(message))
                        return command.Execute(id, message);
            }

            return "";
        }
    }
}
