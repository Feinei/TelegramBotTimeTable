using System.Linq;
using System.Text;

namespace TimeTBot
{
    public class GetTopVisitUsersCommand<TTimeTableEvent, TEvent, TUser> : Command
        where TTimeTableEvent : class, ITimeTableEvent, new()
        where TEvent : class, IEvent
        where TUser : class, IUser
    {
        private IDbContext<TTimeTableEvent, TEvent, TUser> db;

        public GetTopVisitUsersCommand(IDbContext<TTimeTableEvent, TEvent, TUser> db) : base("/s top")
        {
            this.db = db;
        }

        public override string Execute(string id, string message)
        {
            var visits = db.GetAllVisits();
            var builder = new StringBuilder();
            foreach (var visit in visits.OrderByDescending(v => v.Item2).Take(10))
                builder.Append($"{visit.Item2} - {visit.Item1} | ");
            return builder.ToString();
        }

        public override string GetDescription()
        {
            return "Топ активных пользователей";
        }
    }
}
