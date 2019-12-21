using System;
using System.Linq;

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
        bool TryToRemoveTimeTableEvent(string userId, string eventName);
        void RemoveTimeTable(string userId);

        void AddEvent(TEvent ev);
        IQueryable<TEvent> GetEvents(string userId);
        IQueryable<TEvent> GetAllEvents();
        bool TryToRemoveEvent(string userId, string eventName);

        void AddVisit(string userId);
        void AddUserForVisit(string userId, string userName);
        int GetCountOfVisits(string userId);
        // Tuple (user name | count of visits)
        Tuple<string, int>[] GetAllVisits();
    }
}
