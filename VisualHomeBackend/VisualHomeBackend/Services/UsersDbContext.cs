using Microsoft.EntityFrameworkCore;
using System.Configuration;
using VisualHomeBackend.Models;
using Microsoft.Extensions.Configuration;
//using MySql.Data.MySqlClient;
using VisualHomeBackend.Types;
using System.Collections.ObjectModel;
using System.Collections.Immutable;
using System.Reflection.Emit;

namespace VisualHomeBackend.Services
{

    /// <summary>
    /// The DbContext service enables the use of EntityFramwork.
    /// It has a lot of nice features for interacting with a db such as caching to reduce db calls.
    /// This class is composed of the base class instead of inheriting to encapsulate its non-relevant members and exceptions.
    /// TODO: Find out what happens if connection string is bad. Update: asp.net forwards the exception in the http response
    /// </summary>
    public class UsersDbContext
    {
        private UsersDbContextInternal _context;

        public UsersDbContext(string connectionString)
        {
            DbContextOptionsBuilder<UsersDbContextInternal> contextOptionsBuilder = new();
            contextOptionsBuilder.UseNpgsql(connectionString);
            contextOptionsBuilder.UseSnakeCaseNamingConvention(); // Convert from C# naming convention to PostgreSQL made available by EFCore.NamingConventions
            _context = new UsersDbContextInternal(contextOptionsBuilder.Options);
        }

        public async Task<ReadOnlyCollection<User>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.AsReadOnly();
        }

        public async Task CreateUserAsync(User user)
        {
            // The responses here are made so that any dependencies to Microsoft.EntityFrameworkCore
            // are kept within this class
            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();                
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbConcurrencyException();
            }

            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                throw new DbUpdateException(ex.Message);
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);

            try
            {
                await _context.SaveChangesAsync();                
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbConcurrencyException();
            }

            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                throw new DbUpdateException(ex.Message);
            }
        }


        /// <summary>
        /// This class is made to hide all methods that are not relevant to users of UserDbContext
        /// </summary>
        private class UsersDbContextInternal : DbContext
        {
            public DbSet<User> Users { get; set; } // Will be overwritten by a concrete type by EF using reflection

            public UsersDbContextInternal(DbContextOptions<UsersDbContextInternal> options) : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // The primary key must be defined like this:
                modelBuilder.Entity<User>().HasKey(u => u.Name);                
            }
        }
    }

    public class DbConcurrencyException : Exception { }

    public class DbUpdateException : Exception         
    {
        public DbUpdateException(string message) : base(message) { }
    }




}
