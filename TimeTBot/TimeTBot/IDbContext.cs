    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTBot
{
    public interface IDbContext<TTimeTableEvent, TEvent, TUser>
        where TTimeTableEvent : class, ITimeTableEvent
        where TEvent : class, IEvent
        where TUser : class, IUser
    {
        bool IsUserAdded(string userId);
        void TryToAddUser(TUser user);
        TUser GetUser(string userId);

        void AddTimeTableEvent(TTimeTableEvent ev);
        IQueryable<TTimeTableEvent> GetTimeTableEvents(string userId);

        void AddEvent(TEvent ev);
        IQueryable<TEvent> GetEvents(string userId);
    }
}
