using FitnesAP.Models;
using System.Text.Json;

namespace FitnesAP.Data
{
    public class ExerciseService
    {
        private readonly string _path = "data/Exercises.json";
        public ExerciseService(string path = "data/Exercises.json")
        {
            _path = path;
        }
        public List<Exercise> GetExercises()
        {
            if (!File.Exists(_path)) return new List<Exercise>();

            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<Exercise>>(json) ?? new List<Exercise>();
        }

        public void AddExercise(Exercise newExercise)
        {
            var exercises = GetExercises();

            // Avtomatsko določimo nov ID (zadnji + 1)
            newExercise.Id = exercises.Count > 0 ? exercises.Max(e => e.Id) + 1 : 1;

            exercises.Add(newExercise);

            var json = JsonSerializer.Serialize(exercises, new JsonSerializerOptions { WriteIndented = true });

            // Preverimo in ustvarimo mapo, če manjka
            var directory = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(_path, json);
        }
    }
}
