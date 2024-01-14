using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using VisualHomeBackend.Models;
using VisualHomeBackend.Types;

namespace VisualHomeBackend.Services
{
    /// <summary>
    /// Abstraction layer to db
    /// To be run as a scoped service in asp.net
    /// Running my own caching logic here in case i have to move away from EF in the furture.
    /// </summary>
    public class UsersDbService
    {
        private readonly ConcurrentDictionary<Guid, User> _usersById;
        private readonly ConcurrentDictionary<string, User> _usersByName;
        private bool _dbHasBeenRead;
        private UsersDbContext _usersDbContext;

        public UsersDbService(string connectionString) 
        {
            _usersDbContext = new UsersDbContext(connectionString);
            _dbHasBeenRead = false;
            _usersById = new();
            _usersByName = new();
        }

        public async Task CreateUser(User newUser)
        {
            await UpdateCacheIfRequired();

            try
            {
                _usersById[newUser.Id] = newUser;
                _usersByName[newUser.Name] = newUser;
            }
            catch (Exception)
            {
                throw new FailedToUpdateCachedUserException();
            }

            try
            {
                await _usersDbContext.CreateUserAsync(newUser);
            }

            catch (Exception ex) 
            {
                _usersById.TryRemove(newUser.Id, out _);
                _usersByName.TryRemove(newUser.Name, out _);
                throw new FailedToUpdateDbException(ex);
            }            
        }        

        public async Task<User> UpdateUser(User updatedUser)
        {
            if (updatedUser == null)
                throw new UserNullException();

            await UpdateCacheIfRequired();

            if (updatedUser.Id is null)
                throw new IdNullException();


            try
            {
                _usersById[updatedUser.Id] = updatedUser;
                _usersByName[updatedUser.Name] = updatedUser;
            }
            catch (Exception)
            {
                throw new FailedToUpdateCachedUserException();                
            }

            try
            {
                return await _usersDbContext.UpdateUserAsync(updatedUser);
            }
            catch (Exception ex)
            {
                _usersById.TryRemove(updatedUser.Id, out _);
                _usersByName.TryRemove(updatedUser.Name, out _);

                throw new FailedToUpdateDbException(ex);
            }            
        }

        /// <summary>
        /// Gets user from database. Returns null if not found.
        /// </summary>
        public async Task<User?> GetUserById(Guid userId)
        {
            await UpdateCacheIfRequired();

            User? user;
            _usersById.TryGetValue(userId, out user);
            return user;
        }

        /// <summary>
        /// Gets user from database. Returns null if not found.
        /// </summary>
        public async Task<User?> GetUserByName(string username)
        {
            await UpdateCacheIfRequired();

            User? user;
            _usersByName.TryGetValue(username, out user);

            return user;
        }

        private async Task ReadAllUsersFromDb()
        {
            var users = await _usersDbContext.GetAllUsersAsync();

            _usersById.Clear();
            _usersByName.Clear();

            foreach (var u in users) 
            {
                _usersById.TryAdd(u.Id, u);
                _usersByName.TryAdd(u.Name, u);
            }

            _dbHasBeenRead = true;
        }

        private async Task UpdateCacheIfRequired()
        {
            if (!_dbHasBeenRead)
            {
                await ReadAllUsersFromDb();
            }
        }

    }

    public class ItemAlreadyExistsException : Exception { }
    public class FailedToUpdateCachedUserException : Exception { }
    public class FailedToUpdateDbException(Exception ex) : Exception("", ex) { }
    public class UserNullException : Exception { }
    public class IdNullException : Exception { }
    public class NameNullException : Exception { }





}
