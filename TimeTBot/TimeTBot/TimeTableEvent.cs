using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTBot
{
    public class TimeTableEvent : ITimeTableEvent
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        // Lenght <= 60
        public string Name { get; set; }

        // In seconds
        public int TimeFrom { get; set; }

        // In seconds
        public int TimeTo { get; set; }

        public DayType Day { get; set; }

        public ITimeTableEvent Parse(string timeTableEvent)
        {
            try
            {
                var data = timeTableEvent.Split(',');
                object day;
                if (!Enum.TryParse(typeof(DayType), data[0], true, out day))
                    return null;
                var timeToData = data[1].Trim().Split(':');
                var timeTo = Convert.ToInt32(timeToData[0]) * 3600 + Convert.ToInt32(timeToData[1]) * 60 + Convert.ToInt32(timeToData[2]);
                var timeFromData = data[1].Trim().Split(':');
                var timeFrom = Convert.ToInt32(timeFromData[0]) * 3600 + Convert.ToInt32(timeFromData[1]) * 60 + Convert.ToInt32(timeFromData[2]);
                return new TimeTableEvent()
                {
                    Day = (DayType)day,
                    Name = data[3].Trim(),
                    TimeFrom = timeFrom,
                    TimeTo = timeTo
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
