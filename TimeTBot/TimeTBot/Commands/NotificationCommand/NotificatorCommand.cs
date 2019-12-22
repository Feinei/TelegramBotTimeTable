using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TimeTBot
{
    public class NotificatorCommand<TTimeTableEvent, TEvent, TUser> : Command
    where TTimeTableEvent : class, ITimeTableEvent, new()
    where TEvent : class, IEvent, new()
    where TUser : class, IUser
    {
        private IDbContext<TTimeTableEvent, TEvent, TUser> db;
        private static SortedList<DateTime, IEvent> allReminds = new SortedList<DateTime, IEvent>();

        private static CancellationTokenSource tokenSource = new CancellationTokenSource();
        private static CancellationToken token = tokenSource.Token;

        public readonly static TimeSpan rusTime = new TimeSpan(3, 0, 0);

        private readonly ITurnContext bot;
        private static bool isWorking = false;

        public NotificatorCommand(IDbContext<TTimeTableEvent, TEvent, TUser> context,
        ITurnContext bot) : base(@"/e create")
        {
            db = context;
            this.bot = bot;
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
                return "Неправильный формат! Сообщение должно быть вида: \"e create : DD.MM.YYYY HH.MM - event_name\"";
            }

            if (notif.NextTime < (DateTime.UtcNow + rusTime))
                return "Это время уже прошло";

            Action<ITurnContext, TEvent> Send = (client, notification) =>
            {
                if (int.TryParse(id, out int _id))
                    client.SendActivityAsync(MessageFactory.Text(notification.Description)).GetAwaiter().GetResult();
            };

            tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;

            lock (allReminds)
            {
                notif.UserId = id;
                allReminds.Add(notif.NextTime, notif);
                db.AddEvent((TEvent)notif);
            }

            if (!isWorking)
            {
                Task.Factory.StartNew(() => SendReminds(token, bot, Send, id));
                isWorking = true;
            }

            return "";
        }

        public override string GetDescription()
        {
            return "Создать напоминание";
        }

        private void SendReminds(CancellationToken ct, ITurnContext client,
        Action<ITurnContext, TEvent> Send, string id)
        {
            while (true)
            {
                var notif = allReminds.First();
                var interval = notif.Value.NextTime - (DateTime.UtcNow + rusTime);

                if (interval <= TimeSpan.Zero)
                {
                    interval = TimeSpan.Zero;
                }
                client.Activity.Conversation.Id = notif.Value.UserId;
                Task.Delay(interval, ct)
                .ContinueWith(x => Send(client, (TEvent)notif.Value), ct)
                .Wait(ct);

                db.TryToRemoveEvent(id, notif.Value.Description);
            }
        }
    }
}