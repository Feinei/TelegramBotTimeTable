using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TimeTBot
{
    public class CreateTimeTableCommand<TTimeTableEvent, TEvent, TUser> : Command
        where TTimeTableEvent : class, ITimeTableEvent
        where TEvent : class, IEvent
        where TUser : class, IUser
    {
        private IDbContext<TTimeTableEvent, TEvent, TUser> db;
        private ICacheDictionary<string, Dictionary<string, string>> cache;

        public CreateTimeTableCommand(IDbContext<TTimeTableEvent, TEvent, TUser> context,
            ICacheDictionary<string, Dictionary<string, string>> cacheDictionary) : base(@"/tt create")
        {
            db = context;
            cache = cacheDictionary;
        }

        public override string Execute(string id, string message)
        {
            if (!cache.ContainsKey(id))
            {
                cache.AddValue(id, new Dictionary<string, string>());
            }
            if (message.Contains(this.Name))
                return "Введите события для расписания в формате \"День недели, время от (в формате \"hh:mm:ss\"), время до, название\"." +
                    "\nВведите \"выход\", чтобы закончись.";
            if (message == "Выход")
            {
                if (saveChanges())
                    return "Расписание сохранено!";
                return "Ошибка сохранения! Возможно отсутствуют данные!";
            }
            return tryToAddEvent(message) ? null : "Ошибка! Некорректный формат!";
        }

        private bool saveChanges()
        {
            // сохранить все события, отчистить кэш
            throw new NotImplementedException();
        }

        private bool tryToParseMessage(string message)
        {
            // сохранить в кэше
            throw new NotImplementedException();
        }

        private bool tryToAddEvent(string message)
        {
            throw new NotImplementedException();
        }

        public override string GetDescription()
        {
            return "Create time table";
        }
    }
}
