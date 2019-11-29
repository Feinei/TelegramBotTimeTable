using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TimeTBot
{
    public class TimeTableCommand : Command
    {
        private Command[] subCommands { get; }

        public TimeTableCommand() : base(@"/tt")
        {
            subCommands = new Command[] { new CreateTimeTableCommand(), new ShowTimeTableCommand() };
        }

        public override string GetDescription()
        {
            return String.Join(", ", subCommands.Select(c => c.GetDescription()));
        }

        public override string Execute(string id, string message)
        {
            foreach(var command in subCommands)
                if (command.Contains(message))
                    return command.Execute(id, message);

            return null;
        }
    }
}
