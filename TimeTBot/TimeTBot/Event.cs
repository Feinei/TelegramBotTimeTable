using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTBot
{
    public class Event : IEvent
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        // Lenght <= 60
        public string Description { get; set; }

        public DateTime NextTime { get; set; }

        // In seconds
        public int Freq { get; set; }

        public bool IsDone { get; set; }

        public IEvent SplitMessage(string message)
        {
            if (message[message.Length - 1] == '.')
                message.Remove(message.Length - 1);
            var splitedMessage = message.Split(':')[1]
            .Trim()
            .Split('-')
            .Select(x => x.Trim())
            .ToArray();
            var date = DateTime.ParseExact(splitedMessage[0], "dd.MM.yyyy HH.mm", CultureInfo.GetCultureInfo("ru-RU"));

            return new Event
            {
                Description = splitedMessage[1],
                NextTime = date
            };
        }
    }
}