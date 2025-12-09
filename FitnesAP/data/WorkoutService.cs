using FitnesAP.Models;
using System.Text.Json;

namespace FitnesAP.data
{
    public class WorkoutService
    {
        private readonly string _path = "data/Workouts.json";

        public WorkoutService(string path = "data/Workouts.json")
        {
            _path = path;
        }

        // Vrne vse treninge za določenega uporabnika
        public List<Workout> GetWorkoutsForUser(int userId)
        {
            if (!File.Exists(_path)) return new List<Workout>();

            var json = File.ReadAllText(_path);
            var allWorkouts = JsonSerializer.Deserialize<List<Workout>>(json) ?? new List<Workout>();

            return allWorkouts.Where(w => w.UserId == userId).ToList();
        }

        // Shrani nov trening
        public void AddWorkout(Workout workout)
        {
            List<Workout> workouts;

            if (File.Exists(_path))
            {
                var json = File.ReadAllText(_path);
                workouts = JsonSerializer.Deserialize<List<Workout>>(json) ?? new List<Workout>();
            }
            else
            {
                workouts = new List<Workout>();
            }

            // Določimo nov ID
            workout.Id = workouts.Count > 0 ? workouts.Max(w => w.Id) + 1 : 1;

            workouts.Add(workout);

            var newJson = JsonSerializer.Serialize(workouts, new JsonSerializerOptions { WriteIndented = true });

            // Preveri mapo
            var directory = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(_path, newJson);
        }

        public void DeleteWorkout(int id)
        {
            if (!File.Exists(_path)) return;

            // 1. Preberi vse
            var json = File.ReadAllText(_path);
            var workouts = JsonSerializer.Deserialize<List<Workout>>(json) ?? new List<Workout>();

            // 2. Najdi trening in ga odstrani
            var workoutToRemove = workouts.FirstOrDefault(w => w.Id == id);
            if (workoutToRemove != null)
            {
                workouts.Remove(workoutToRemove);

                // 3. Shrani nazaj
                var newJson = JsonSerializer.Serialize(workouts, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_path, newJson);
            }
        }

        public void UpdateWorkout(Workout updatedWorkout)
        {
            if (!File.Exists(_path)) return;

            var json = File.ReadAllText(_path);
            var workouts = JsonSerializer.Deserialize<List<Workout>>(json) ?? new List<Workout>();

            // Poiščemo starega in ga zamenjamo z novim
            var existingIndex = workouts.FindIndex(w => w.Id == updatedWorkout.Id);
            if (existingIndex != -1)
            {
                workouts[existingIndex] = updatedWorkout;

                var newJson = JsonSerializer.Serialize(workouts, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_path, newJson);
            }
        }

        // Prav tako potrebujemo metodo GetWorkoutById za nalaganje posameznega treninga
        public Workout? GetWorkoutById(int id)
        {
            if (!File.Exists(_path)) return null;
            var json = File.ReadAllText(_path);
            var workouts = JsonSerializer.Deserialize<List<Workout>>(json) ?? new List<Workout>();
            return workouts.FirstOrDefault(w => w.Id == id);
        }
    }
}
