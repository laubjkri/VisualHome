using Microsoft.EntityFrameworkCore;
using System.Configuration;
using VisualHomeBackend.Models;
using Microsoft.Extensions.Configuration;

namespace VisualHomeBackend.Services
{
    public class DatabaseContextService : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DatabaseContextService(DbContextOptions<DatabaseContextService> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // The primary key must be defined like this:
            modelBuilder.Entity<User>().HasKey(u => u.Name);
        }

    }
}
