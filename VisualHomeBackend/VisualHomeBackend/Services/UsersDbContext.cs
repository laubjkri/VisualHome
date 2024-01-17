using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Extensions.Configuration;
//using MySql.Data.MySqlClient;
using VisualHomeBackend.Types;
using System.Collections.ObjectModel;
using System.Collections.Immutable;
using System.Reflection.Emit;
using Npgsql;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using VisualHomeBackend.Models.User;

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
            contextOptionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); // means new objects with the same Id can be used for updating
            _context = new UsersDbContextInternal(contextOptionsBuilder.Options);
        }

        public async Task<ReadOnlyCollection<User>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return users.AsReadOnly();
            }

            catch (PostgresException ex)
            {
                if (ex.SqlState == "42501")
                {
                    throw new AccessDeniedException("");
                }

                throw;
            }
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

        public async Task<User> UpdateUserAsync(User user)
        {
            try            
            {
                var updated = _context.Users.Update(user).Entity;                
                await _context.SaveChangesAsync();
                return updated;
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
                // The primary key must be defined like this (if it is not called id):
                modelBuilder.Entity<User>().HasKey(u => u.Id);
            }
        }
    }

    public class DbConcurrencyException : Exception { }

    public class DbUpdateException : Exception         
    {
        public DbUpdateException(string message) : base(message) { }
    }

    public class AccessDeniedException : Exception
    {
        public static string DefaultMessage { get; } = "Access to database was denied.";
        public AccessDeniedException(string message) : base(message)         
        {       
        }
    }




}
