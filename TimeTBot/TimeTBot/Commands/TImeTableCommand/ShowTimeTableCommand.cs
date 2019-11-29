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
            try
            {
                var events = db.GetTimeTableEvents(id)
                                .GroupBy(ev => ev.Day);

                var builder = new StringBuilder();

                foreach (var day in events)
                {
                    builder.Append($"{day.Key}:\n");
                    foreach (var e in day)
                        builder.Append($"[{e.TimeTo.ToString()} - {e.TimeFrom.ToString()}] {e.Name} \n");
                }
                return builder.ToString();
            }
            catch
            {
                return "Ошибка! Расписание не найдено!";
            }
        }

        public override string GetDescription()
        {
            return "Посмотреть расписание.";
        }
    }
}
