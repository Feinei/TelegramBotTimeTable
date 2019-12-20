using System;
using System.Linq;
using System.Text;

namespace TimeTBot
{
    public class ShowTimeTableCommand<TTimeTableEvent, TEvent, TUser> : Command
        where TTimeTableEvent : class, ITimeTableEvent
        where TEvent : class, IEvent
        where TUser : class, IUser
    {
        private IDbContext<TTimeTableEvent, TEvent, TUser> db;

        public ShowTimeTableCommand(IDbContext<TTimeTableEvent, TEvent, TUser> context) : base(@"/tt show")
        {
            db = context;
        }

        public override string Execute(string id, string message)
        {
            var builder = new StringBuilder();

            try
            {
                var events = db.GetTimeTableEvents(id)
                                .GroupBy(ev => ev.Day);

                foreach (var day in events)
                {
                    builder.Append($"{day.Key}:\n");
                    foreach (var e in day)
                    {
                        var timeFrom = TimeSpan.FromSeconds(e.TimeFrom);
                        var timeTo = TimeSpan.FromSeconds(e.TimeTo);
                        builder.Append($"({timeFrom.Hours}:{timeFrom.Minutes}:{timeFrom.Seconds} - " +
                            $"{timeTo.Hours}:{timeTo.Minutes}:{timeTo.Seconds}) {e.Name} \n");
                    }
                }
            }
            catch { }

            if (builder.Length == 0)
                return "Ошибка! Расписание не найдено!";
            return builder.ToString();
        }

        public override string GetDescription()
        {
            return "Посмотреть расписание";
        }
    }
}
