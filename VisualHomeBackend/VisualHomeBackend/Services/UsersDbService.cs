using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Collections.Concurrent;
using VisualHomeBackend.Models;
using VisualHomeBackend.Types;

namespace VisualHomeBackend.Services
{
    /// <summary>
    /// Abstraction layer to db
    /// </summary>
    public class UsersDbService
    {
        private readonly ConcurrentDictionary<string, User> _users;
        private bool _dbHasBeenRead;
        private DatabaseContextService _databaseContextService;

        public UsersDbService(DatabaseContextService databaseContextService) 
        {
            _databaseContextService = databaseContextService;
            _dbHasBeenRead = false;
            _users = new();            
        }

        public ApiResponse AddUser(User newUser)
        {
            try
            {
                if(!_users.TryAdd(newUser.Name, newUser)) throw new ItemAlreadyExistsException();
                _databaseContextService.Users.Add(newUser);
                _databaseContextService.SaveChanges();
            }

            catch (DbUpdateConcurrencyException)
            {
                return new ApiResponse() { Success = false, Message = "A concurrency error occured." };
            }

            catch (DbUpdateException ex) 
            {
                if (ex.InnerException is MySqlException mySqlException)
                {
                    // Error code 1062 indicates a duplicate entry (unique constraint violation) in MySQL.
                    if (mySqlException.Number == 1062)
                    {
                        throw new ItemAlreadyExistsException();
                    }
                }

                else throw;
            }

            catch (ItemAlreadyExistsException) 
            {
                return new ApiResponse() { Success = false, Message = "User already exists." };
            }

            catch (Exception)
            {

                return new ApiResponse() { Success = false, Message = "An unknown error occured." };
            }

            return new ApiResponse() { Success = true, Message = "User added." };
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
            foreach (var u in _databaseContextService.Users) 
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
