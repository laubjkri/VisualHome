using System.Collections.Concurrent;
using VisualHomeBackend.Models;

namespace VisualHomeBackend.Services
{
    public class UsersDbService
    {
        private readonly ConcurrentDictionary<string, User> users;
        private bool dbHasBeenRead;

        public UsersDbService() 
        {
            dbHasBeenRead = false;
            users = new();            
            AddUser("admin", "pass"); // TODO: Remove when DB has been implemented
            users["admin"].IsAdmin = true;
        }

        public void AddUser(string username, string password)
        {
            // TODO: Add functionality to report error if user already exists
            User newUser = new User();
            newUser.Username = username;
            newUser.Password = password;
            users.TryAdd(username, newUser);
        }

        private void UpdateUser(User updatedUser)
        {
            // TODO: Update user in memory and DB

        }

        public async Task<User?> GetUser(string username)
        {
            if (!dbHasBeenRead)
            {
                await ReadAllUsersFromDb();                
            }

            User? user;
            users.TryGetValue(username, out user);
            return user;
        }

        private async Task ReadAllUsersFromDb()
        {
            // TODO: Implement DB
            dbHasBeenRead = true;
        }

        public async Task<int> CheckUser(User user)
        {
            User? dbUser = await GetUser(user.Username);
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
}
