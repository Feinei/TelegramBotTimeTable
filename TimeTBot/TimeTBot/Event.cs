﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTBot
{
    public class Event : IEvent
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string Description { get; set; }

        public DateTime NextTime { get; set; }

        // In seconds
        public int Freq { get; set; }

        public bool IsDone { get; set; }
    }
}
