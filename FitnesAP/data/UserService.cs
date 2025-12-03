using FitnesAP.Models;
using System.Text.Json;

namespace FitnesAP.Data
{
    public class UserService
    {
        private readonly string _path = "data/Users.json";

        public List<User> GetUsers()
        {
            if (!File.Exists(_path)) return new List<User>();

            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<User>>(json);
        }

        public void SaveUsers(List<User> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_path, json);
        }

        public bool CheckLogin(string username, string password)
        {
            return GetUsers().Any(u =>
                u.Username == username &&
                u.Password == password);
        }

        public bool Register(string username, string password)
        {
            var users = GetUsers();

            if (users.Any(u => u.Username == username))
                return false; // user already exists

            var newUser = new User
            {
                Id = users.Count + 1,
                Username = username,
                Password = password
            };

            users.Add(newUser);
            SaveUsers(users);

            return true;
        }
    }
}

