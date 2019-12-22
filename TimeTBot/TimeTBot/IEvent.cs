using System;

namespace TimeTBot
{
    public interface IEvent
    {
        int Id { get; set; }
        string UserId { get; set; }

        string Description { get; set; }

        DateTime NextTime { get; set; }

        // In seconds
        int Freq { get; set; }

        bool IsDone { get; set; }

        IEvent SplitMessage(string message);
    }
}
