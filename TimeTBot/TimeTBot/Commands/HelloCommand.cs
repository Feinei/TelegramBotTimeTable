using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TimeTBot
{
    public class HelloCommand : Command
    {
        public override string Name => @"/hi";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            await client.SendTextMessageAsync(chatId, "Hallo I'm ASP.NET Core Bot", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
    }
}
