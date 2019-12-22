using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace TimeTBot
{
    public class ApplicationContext<TTimeTableEvent, TEvent, TUser> : DbContext, IDbContext<TTimeTableEvent, TEvent, TUser>
        where TTimeTableEvent : class, ITimeTableEvent
        where TEvent : class, IEvent
        where TUser : class, IUser
    {
        public DbSet<TUser> Users { get; set; }
        public DbSet<TTimeTableEvent> TimeTableEvents { get; set; }
        public DbSet<TEvent> Events { get; set; }
        public DbSet<View> Views { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext<TTimeTableEvent, TEvent, TUser>> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=tcp:timetbot.database.windows.net," +
                "1433;Initial Catalog=TimeTableDB;Persist Security Info=False;" +
                "User ID=ArtemS00@timetbot;Password=3k@zYghJ;" +
                "MultipleActiveResultSets=False;Encrypt=True;" +
                "TrustServerCertificate=False;Connection Timeout=30;");
        }
        
        // Returns true if user is added
        public bool IsUserAdded(string id)
        {
            return Users.Find(id) != null;
        }

        // Try to add user
        public void TryToAddUser(TUser user)
        {
            if (IsUserAdded(user.Id))
                return;
            Users.Add(user);
            Views.Add(new View() { Id = user.Id, Count = 0 });
            SaveChanges();
        }

        // Get user
        public TUser GetUser(string userId)
        {
            if (!IsUserAdded(userId))
                return null;
            return Users.Find(userId);
        }

        // Add TT event
        public void AddTimeTableEvent(TTimeTableEvent ev)
        {
            TimeTableEvents.Add(ev);
            SaveChanges();
        }

        // Get all TT events for this user
        public IQueryable<TTimeTableEvent> GetTimeTableEvents(string userId)
        {
            return TimeTableEvents.Where(u => u.UserId == userId);
        }

        // Remove time table event by name
        public bool TryToRemoveTimeTableEvent(string userId, string eventName)
        {
            var ev = TimeTableEvents
                .Where(e => e.UserId == userId && e.Name.ToLower() == eventName.ToLower())
                .FirstOrDefault();
            if (ev == null)
                return false;
            TimeTableEvents.Remove(ev);
            SaveChanges();
            return true;
        }

        // Remove all time table
        public void RemoveTimeTable(string userId)
        {
            var events = TimeTableEvents
                .Where(e => e.UserId == userId);
            TimeTableEvents.RemoveRange(events);
            SaveChanges();
        }

        // Add event
        public void AddEvent(TEvent ev)
        {
            Events.Add(ev);
            SaveChanges();
        }

        // Get all events for this users
        public IQueryable<TEvent> GetEvents(string userId)
        {
            return Events.Where(e => e.UserId == userId);
        }

        // Get all events
        public IQueryable<TEvent> GetAllEvents()
        {
            return Events;
        }

        // Returns true if removed
        public bool TryToRemoveEvent(string userId, string eventName)
        {
            var ev = Events
                .Where(e => e.UserId == userId && e.Description == eventName)?
                .SingleOrDefault();

            if (ev == null)
                return false;
            
            Events.Remove(ev);
            SaveChanges();
            return true;
        }

        // Count new visit
        public void AddVisit(string userId)
        {
            var stat = Views.Find(userId);
            if (stat == null)
                return;
            stat.Count++;
            SaveChanges();
        }

        // Get count of visits for user
        public int GetCountOfVisits(string userId)
        {
            var stat = Views.Find(userId);
            return stat == null ? 0 : stat.Count;
        }

        // Get all visits (Tuple [ User id | Count of visits ])
        public Tuple<string, int>[] GetAllVisits()
        {
            return Views.Select(st => Tuple.Create(st.Id, st.Count)).ToArray();
        }
    }
}
