using Microsoft.EntityFrameworkCore;
using EventApp.Models;  // This should point to your Event model's namespace

namespace EventApp.Data
{
    public class EventDbContext : DbContext
    {
        public EventDbContext(DbContextOptions<EventDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }  // Event entity
    }
}