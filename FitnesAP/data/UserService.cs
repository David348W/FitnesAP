using FitnesAP.Models;
using System.Text.Json;

namespace FitnesAP.Data
{
    public class UserService
    {
        // Pazi, da imaš mapo "data" ustvarjeno v projektu
        private readonly string _path = "data/Users.json";

        // --- OSNOVNE METODE (Branje in Pisanje) ---

        public List<User> GetUsers()
        {
            if (!File.Exists(_path)) return new List<User>();

            var json = File.ReadAllText(_path);
            // Če je datoteka prazna, vrnemo prazen seznam
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        public void SaveUsers(List<User> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Ustvari mapo, če slučajno ne obstaja
            var directory = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(_path, json);
        }

        // --- METODE ZA PRIJAVO IN REGISTRACIJO ---

        public bool CheckLogin(string username, string password)
        {
            return GetUsers().Any(u => u.Username == username && u.Password == password);
        }

        public bool Register(string username, string password)
        {
            var users = GetUsers();

            if (users.Any(u => u.Username == username))
                return false; // Uporabnik že obstaja

            var newUser = new User
            {
                // Določimo ID (če je seznam prazen, je ID 1, sicer max + 1)
                Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1,
                Username = username,
                Password = password,
                Ime = "",      // Prazno na začetku
                Priimek = "",  // Prazno na začetku
                Email = ""     // Prazno na začetku
            };

            users.Add(newUser);
            SaveUsers(users);

            return true;
        }

        // --- NOVE METODE ZA PROFIL (To sva dodala nazadnje) ---

        public User? GetUserByUsername(string username)
        {
            var users = GetUsers();
            return users.FirstOrDefault(u => u.Username == username);
        }

        public void UpdateUser(User userToUpdate)
        {
            var users = GetUsers();

            // Najdemo obstoječega uporabnika v seznamu po ID-ju
            var existingUser = users.FirstOrDefault(u => u.Id == userToUpdate.Id);

            if (existingUser != null)
            {
                // Posodobimo podatke
                existingUser.Ime = userToUpdate.Ime;
                existingUser.Priimek = userToUpdate.Priimek;
                existingUser.Email = userToUpdate.Email;

                // Če je uporabnik vpisal novo geslo, ga posodobimo
                if (!string.IsNullOrEmpty(userToUpdate.Password))
                {
                    existingUser.Password = userToUpdate.Password;
                }

                // Shranimo celoten seznam nazaj v JSON
                SaveUsers(users);
            }
        }
    }
}