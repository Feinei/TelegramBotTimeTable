using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTBot.Commands.NotificationCommand
{
    class ShowAllNotificationsCommand<TTimeTableEvent, TEvent, TUser> : Command
    where TTimeTableEvent : class, ITimeTableEvent, new()
    where TEvent : class, IEvent
    where TUser : class, IUser
    {
        private IDbContext<TTimeTableEvent, TEvent, TUser> db;

        public ShowAllNotificationsCommand(IDbContext<TTimeTableEvent, TEvent, TUser> context) : base(@"/e show")
        {
            db = context;
        }

        public override string Execute(string id, string message)
        {
            var events = db.GetEvents(id)
            .OrderBy(ev => ev.NextTime);

            if (events.Count() == 0)
                return "List is empty";

            else
            {
                var resultStr = new StringBuilder();
                foreach (var ev in events)
                {
                    resultStr.Append(ev.NextTime.ToString() + " - " + ev.Description);
                    resultStr.Append(Environment.NewLine);
                }
                return resultStr.ToString();
            }
        }

        public override string GetDescription()
        {
            return "Show all notifications";
        }
    }
}
