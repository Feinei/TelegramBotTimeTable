using System.Collections.Generic;

namespace TimeTBot
{
    public class RemoveTimeTableCommand<TTimeTableEvent, TEvent, TUser> : Command
        where TTimeTableEvent : class, ITimeTableEvent
        where TEvent : class, IEvent
        where TUser : class, IUser
    {
        private IDbContext<TTimeTableEvent, TEvent, TUser> db;
        private ICacheDictionary<string, Dictionary<string, string>> cacheDictionary;

        public RemoveTimeTableCommand(IDbContext<TTimeTableEvent, TEvent, TUser> context, 
            ICacheDictionary<string, Dictionary<string, string>> cacheDictionary) : base(@"/tt remove")
        {
            db = context;
            this.cacheDictionary = cacheDictionary;
        }

        public override string Execute(string id, string message)
        {
            if (message.Contains(this.Name))
            {
                SetCache(id);
                return "Введите название события для его удаления. Введите \"удалить\" для удаления всего расписания.";
            }
            ClearCache(id);
            if (message.ToLower() == "удалить")
            {
                RemoveTimeTable(id);
                return "Удалено успешно!";
            }
            return TryToRemoveEvent(id, message) ? "Удалено успешно!" : "Ошибка! Событие не найдено!";
        }

        private void ClearCache(string id)
        {
            if (!cacheDictionary.ContainsKey(id))
                cacheDictionary.AddValue(id, new Dictionary<string, string>());
            var dict = cacheDictionary.GetValue(id);
            dict.Remove("next");
        }

        private void SetCache(string id)
        {
            if (!cacheDictionary.ContainsKey(id))
                cacheDictionary.AddValue(id, new Dictionary<string, string>());
            var dict = cacheDictionary.GetValue(id);
            dict["next"] = this.Name;
        }

        private void RemoveTimeTable(string id)
        {
            db.RemoveTimeTable(id);
        }

        private bool TryToRemoveEvent(string id, string message)
        {
            return db.TryToRemoveTimeTableEvent(id, message);
        }

        public override string GetDescription()
        {
            return "Удалить расписание";
        }
    }
}
