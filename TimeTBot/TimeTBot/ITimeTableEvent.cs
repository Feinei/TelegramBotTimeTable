using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTBot
{
    public interface ITimeTableEvent
    {
        int Id { get; set; }
        string UserId { get; set; }

        string Name { get; set; }

        // In seconds
        int TimeFrom { get; set; }

        // In seconds
        int TimeTo { get; set; }

        DayType Day { get; set; }
    }
}
