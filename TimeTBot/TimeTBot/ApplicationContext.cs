using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public ApplicationContext(DbContextOptions<ApplicationContext<TTimeTableEvent, TEvent, TUser>> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=tcp:adas-server.database.windows.net," +
                "1433;Initial Catalog=ADAS-DB;Persist Security Info=False;" +
                "User ID=ArtemS00@adas-server;Password=3k@zYghJ;" +
                "MultipleActiveResultSets=False;Encrypt=True;" +
                "TrustServerCertificate=False;Connection Timeout=30;");
        }
        
        // Return true if user is added
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
    }
}
