using System.Collections.Generic;
using System.Linq;

namespace TimeTBot
{
    public class CreateTimeTableCommand<TTimeTableEvent, TEvent, TUser> : Command
        where TTimeTableEvent : class, ITimeTableEvent, new()
        where TEvent : class, IEvent
        where TUser : class, IUser
    {
        private IDbContext<TTimeTableEvent, TEvent, TUser> db;
        private ICacheDictionary<string, Dictionary<string, string>> cacheDictionary;

        public CreateTimeTableCommand(IDbContext<TTimeTableEvent, TEvent, TUser> context,
            ICacheDictionary<string, Dictionary<string, string>> cacheDictionary) : base(@"/tt create")
        {
            db = context;
            this.cacheDictionary = cacheDictionary;
        }

        public override string Execute(string id, string message)
        {
            if (message.Contains(this.Name))
            {
                SetCache(id);
                return "Введите события для расписания в формате \"День недели, время от (в формате \"hh:mm:ss\"), время до, название\"." +
                    "\nВведите \"выход\", чтобы закончить.";
            }
            if (message.ToLower() == "выход")
            {
                ClearCache(id);
                return "Расписание сохранено!";
            }
            return TryToSaveEvent(id, message);
        }

        private void SetCache(string id)
        {
            if (!cacheDictionary.ContainsKey(id))
                cacheDictionary.AddValue(id, new Dictionary<string, string>());
            var dict = cacheDictionary.GetValue(id);
            dict["next"] = this.Name;
        }

        private void ClearCache(string id)
        {
            if (!cacheDictionary.ContainsKey(id))
                return;
            var dict = cacheDictionary.GetValue(id);
            dict.Remove("next");
        }

        private string TryToSaveEvent(string id, string message)
        {
            var ev = new TTimeTableEvent().Parse(message);
            if (ev == null)
                return "Ошибка! Некорректный формат!\n Введите \"выход\", чтобы закончить.";
            var events = db.GetTimeTableEvents(id);
            if (events.FirstOrDefault(e => e.Day == ev.Day && (e.TimeTo > ev.TimeFrom && e.TimeTo < ev.TimeTo
                || e.TimeFrom > ev.TimeFrom && e.TimeFrom < ev.TimeTo)) == null)
                return "Ошибка! Событие пересекается с уже существующим!";
            ev.UserId = id;
            db.AddTimeTableEvent((TTimeTableEvent)ev);
            return "Сохранено успешншо!";
        }

        public override string GetDescription()
        {
            return "Создать расписние";
        }
    }
}
