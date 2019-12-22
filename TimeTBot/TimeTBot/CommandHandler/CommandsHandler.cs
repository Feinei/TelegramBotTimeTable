using System.Collections.Generic;
using System.Linq;

namespace TimeTBot
{
    public class StandartCommandsHandler : BaseCommandHandler
    {
        private Dictionary<string, Command> allCommands;

        public StandartCommandsHandler(ICacheDictionary<string, Dictionary<string, string>> cacheDictionary, Command[] commands) 
            : base(cacheDictionary, commands)
        {
            allCommands = commands
                .ToDictionary(c => c.Name, c => c);
        }

        public override string Execute(string id, string message)
        {        
            // Execute previous command (if exists)
            if (cacheDictionary.ContainsKey("next"))
            {
                var commandName = cacheDictionary.GetValue("id")?["next"];
                if (allCommands.ContainsKey(commandName))
                    return allCommands[commandName].Execute(id, message);
            }
            
            // Execute command
            foreach(var command in commands)
            {
                if (command.Contains(message))
                    return command.Execute(id, message);
            }

            return "";
        }
    }
}
