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

        public string Name { get; set; }

        // In seconds
        public int TimeFrom { get; set; }

        // In seconds
        public int TimeTo { get; set; }

        public DayType Day { get; set; }
    }
}
