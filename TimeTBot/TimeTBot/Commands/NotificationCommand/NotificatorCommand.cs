using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TimeTBot.Commands
{
    class NotificatorCommand<TTimeTableEvent, TEvent, TUser> : Command
    where TTimeTableEvent : class, ITimeTableEvent, new()
    where TEvent : class, IEvent, new()
    where TUser : class, IUser
    {
        private IDbContext<TTimeTableEvent, TEvent, TUser> db;
        private static SortedList<DateTime, IEvent> allReminds = new SortedList<DateTime, IEvent>();

        private static CancellationTokenSource tokenSource = new CancellationTokenSource();
        private static CancellationToken token = tokenSource.Token;

        public readonly static TimeSpan rusTime = new TimeSpan(3, 0, 0);

        private readonly ITurnContext<IMessageActivity> bot;

        public NotificatorCommand(IDbContext<TTimeTableEvent, TEvent, TUser> context,
        ITurnContext<IMessageActivity> bot) : base(@"/e create")
        {
            db = context;
            this.bot = bot;
           
            foreach (var ev in db.GetAllEvents())
                allReminds.Add(ev.NextTime, ev);
        }

        public override string Execute(string id, string message)
        {
            IEvent notif;
            try
            {
                notif = new TEvent().SplitMessage(message);
            }
            catch
            {
                return "Wrong format. If you want to create notification, write like: \"e create : DD.MM.YYYY HH.MM - event_name\"";
            }

            if (notif.NextTime < (DateTime.UtcNow + rusTime))
                return "This time has expired.";

            Action<ITurnContext<IMessageActivity>, TEvent> Send = (client, notification) =>
            {
                if (int.TryParse(id, out int _id))
                    client.SendActivityAsync(MessageFactory.Text(notification.Description)).GetAwaiter().GetResult();
            };

            tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;

            lock (allReminds)
            {
                allReminds.Add(notif.NextTime, notif);
                db.AddEvent((TEvent)notif);
            }

            if (allReminds.Count == 1)
                Task.Factory.StartNew(() => SendReminds(token, bot, Send, id));

            return "";
        }

        public override string GetDescription()
        {
            return "Create notification";
        }

        private void SendReminds(CancellationToken ct, ITurnContext<IMessageActivity> client,
        Action<ITurnContext<IMessageActivity>, TEvent> Send, string id)
        {
            while (true)
            {
                if (allReminds.Count == 0)
                    break;

                var notif = allReminds.First();       
                var interval = notif.Value.NextTime - (DateTime.UtcNow + rusTime);

                if (interval <= TimeSpan.Zero)
                {
                    interval = TimeSpan.Zero;
                }

                Task.Delay(interval, ct)
                .ContinueWith(x => Send(client, (TEvent)notif.Value), ct)
                .Wait(ct);

                allReminds.Remove(notif.Key);
                db.TryToRemoveEvent(id, notif.Value.Description);
            }
        }
    }
}
