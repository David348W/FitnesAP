using FitnesAP.Models;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace FitnesAP.data
{
    public class WeightHistoryService
    {
        private readonly string _path = "data/WeightHistory.json";

        public WeightHistoryService(string path = "data/WeightHistory.json") 
        {
            _path = path;
        }

        public List <WeightEntry> GetHistoryForUsers(int userId)
        {
            if(!File.Exists(_path))
            {
                return new List<WeightEntry>();
            }
            var json = File.ReadAllText(_path);
            var allEntries = JsonSerializer.Deserialize<List<WeightEntry>>(json) ?? new List<WeightEntry>();

            return allEntries.Where(x => x.UserId == userId).OrderBy(x => x.Date).ToList();

        }
        public void AddEntry(int userId, double weight)
        {
            var entries = new List<WeightEntry>();
            if(File.Exists(_path))
            {
                var json = File.ReadAllText(_path);
                entries = JsonSerializer.Deserialize<List<WeightEntry>>(json) ?? new List<WeightEntry>();
            }

            var newEntry = new WeightEntry
            {
                Id = entries.Count > 0 ? entries.Max(e => e.Id) + 1 : 1,
                UserId = userId,
                Weight = weight,
                Date = DateTime.Now
            };

            entries.Add(newEntry);

            var newJson = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });

            var dir = Path.GetDirectoryName(_path);
            if(!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(_path, newJson);
        }

    }
}
