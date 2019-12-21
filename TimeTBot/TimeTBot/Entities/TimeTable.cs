using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTBot
{
    public class TimeTable<TEvent>
        where TEvent : ITimeTableEvent
    {
        public Dictionary<DayType, List<TEvent>> Table { get; }

        public TimeTable(Dictionary<DayType, List<TEvent>> table)
        {
            Table = table;
        }
    }
}
