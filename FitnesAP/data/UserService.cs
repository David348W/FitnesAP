using FitnesAP.Models;
using System.Text.Json;

namespace FitnesAP.Data
{
    public class UserService
    {
       
        private readonly string _path = "data/Users.json";
        public UserService(string path = "data/Users.json")
        {
            _path = path;
        }
        

        public List<User> GetUsers()
        {
            if (!File.Exists(_path)) return new List<User>();

            var json = File.ReadAllText(_path);
            
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        public void SaveUsers(List<User> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            
            var directory = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(_path, json);
        }

      

        public bool CheckLogin(string username, string password)
        {
            return GetUsers().Any(u => u.Username == username && u.Password == password);
        }

        public bool Register(string username, string password)
        {
            var users = GetUsers();

            if (users.Any(u => u.Username == username))
                return false; 

            var newUser = new User
            {
            
                Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1,
                Username = username,
                Password = password,
                Ime = "",      
                Priimek = "",  
                Email = ""     
            };

            users.Add(newUser);
            SaveUsers(users);

            return true;
        }

        

        public User? GetUserByUsername(string username)
        {
            var users = GetUsers();
            return users.FirstOrDefault(u => u.Username == username);
        }

        public void UpdateUser(User userToUpdate)
        {
            var users = GetUsers();

            
            var existingUser = users.FirstOrDefault(u => u.Id == userToUpdate.Id);

            if (existingUser != null)
            {
              
                existingUser.Ime = userToUpdate.Ime;
                existingUser.Priimek = userToUpdate.Priimek;
                existingUser.Email = userToUpdate.Email;

                
                if (!string.IsNullOrEmpty(userToUpdate.Password))
                {
                    existingUser.Password = userToUpdate.Password;
                }

               
                SaveUsers(users);
            }
        }
    }
}