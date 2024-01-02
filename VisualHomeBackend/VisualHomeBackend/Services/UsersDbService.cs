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
        private readonly ConcurrentDictionary<string, User> _users;
        private bool _dbHasBeenRead;
        private UsersDbContext _usersDbContext;

        public UsersDbService(string connectionString) 
        {
            _usersDbContext = new UsersDbContext(connectionString);
            _dbHasBeenRead = false;
            _users = new();            
        }

        public async Task<ApiResponse> AddUser(User newUser)
        {

            DbWriteResponse response = await _usersDbContext.AddUserAsync(newUser);

            switch (response)
            {
                case DbWriteResponse.Success:
                    _users.TryAdd(newUser.Name, newUser);
                    return new ApiResponse() { Success = true, Message = "User added." };

                case DbWriteResponse.AlreadyExistsError:
                    return new ApiResponse() { Success = false, Message = "User already exists." };

                case DbWriteResponse.UnknownError:
                    return new ApiResponse() { Success = false, Message = "An unknown error occured." };

                case DbWriteResponse.ConcurrencyError:
                    return new ApiResponse() { Success = false, Message = "Concurrency error occured." };

                default:
                    return new ApiResponse() { Success = false, Message = "An unknown result occured." };
            }
        }
        

        private void UpdateUser(User updatedUser)
        {
            // TODO: Update user in memory and DB

        }

        public async Task<User?> GetUser(string username)
        {
            if (!_dbHasBeenRead)
            {
                await ReadAllUsersFromDb();                
            }

            User? user;
            _users.TryGetValue(username, out user);
            return user;
        }

        private async Task ReadAllUsersFromDb()
        {
            var users = await _usersDbContext.GetAllUsersAsync();

            foreach (var u in users) 
            {
                _users.TryAdd(u.Name, u);
            }

            _dbHasBeenRead = true;
        }

        public async Task<int> CheckUser(User user)
        {
            User? dbUser = await GetUser(user.Name);
            if (dbUser == null)
            {
                return -1; // User not found                
            }

            if (dbUser.Password != user.Password) 
            {
                return -2; // Wrong password
            }

            return 0; // User found and pw ok
        }



    }

    public class ItemAlreadyExistsException : Exception { }




}
