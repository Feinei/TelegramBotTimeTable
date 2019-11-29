using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace TimeTBot
{
    public static class CommandHelper
    {
        public static async Task<TimeTable> GetTimeTable(ApplicationContext context, TelegramBotClient client)
        {
            var id = GetId(context, client);
            return context.GetTimeTable(id);
        }

        public static async Task AddTimeTableEvent(ApplicationContext context, TelegramBotClient client, TimeTableEvent timeTableEvent)
        {
            throw new NotImplementedException();
        }

        internal static int GetId(ApplicationContext context, TelegramBotClient client)
        {
            throw new NotImplementedException();
        }
    }
}
