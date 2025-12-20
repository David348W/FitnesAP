using FitnesAP.data;
using FitnesAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnesAp_Tests
{
    [TestClass]
    public class ExerciseProgressTest
    {
        private string _workoutPath;
        private WorkoutService _workoutService;
        [TestInitialize]
        public void Setup()
        {
            _workoutPath = $"test_progress_workouts_{Guid.NewGuid()}.json";
            _workoutService = new WorkoutService(_workoutPath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_workoutPath)) File.Delete(_workoutPath);
        }
        private void UstvariTrening(int userId, DateTime datum, int exerciseId, double[] teze)  // Za lazji sim
        {
            var workout = new Workout
            {
                UserId = userId,
                Date = datum,
                Name = "Test Trening",
                Exercises = new List<WorkoutExercise>()
            };

            var exercise = new WorkoutExercise
            {
                ExerciseId = exerciseId,
                Sets = new List<WorkoutSet>()
            };

            foreach (var teza in teze)
            {
                exercise.Sets.Add(new WorkoutSet { Weight = teza, Reps = 10 });
            }

            workout.Exercises.Add(exercise);
            _workoutService.AddWorkout(workout);
        }
        [TestMethod]
        public void IzracunNapredka_NajdeMaxTezoZaVsakTrening()
        {
           
            int userId = 1;
            int vajaId = 10;    
            
            UstvariTrening(userId, new DateTime(2025, 1, 5), vajaId, new[] { 50.0, 60.0, 55.0 });         //60
            UstvariTrening(userId, new DateTime(2025, 1, 10), vajaId, new[] { 65.0, 65.0, 70.0 });        //70
            UstvariTrening(userId, new DateTime(2025, 1, 15), 999, new[] { 100.0 });             // X
           
            var workouts = _workoutService.GetWorkoutsForUser(userId);
            var rezultati = new List<double>();

            foreach (var workout in workouts.OrderBy(w => w.Date))
            {
                var exerciseInWorkout = workout.Exercises.FirstOrDefault(e => e.ExerciseId == vajaId);

                if (exerciseInWorkout != null && exerciseInWorkout.Sets.Any())
                {
                    double maxWeight = exerciseInWorkout.Sets.Max(s => s.Weight);
                    rezultati.Add(maxWeight);
                }
            }
          
            Assert.AreEqual(2, rezultati.Count, "Najti mora točno 2 treninga s to vajo.");
            Assert.AreEqual(60.0, rezultati[0], "Prvi max lift mora biti 60kg.");
            Assert.AreEqual(70.0, rezultati[1], "Drugi max lift mora biti 70kg.");
        }
        [TestMethod]
        public void IzracunNapredka_IgnoriraPrazneSete()
        {
            int userId = 1;
            int vajaId = 5;
            var w = new Workout { UserId = userId, Date = DateTime.Now, Exercises = new List<WorkoutExercise>() };
            var we = new WorkoutExercise { ExerciseId = vajaId, Sets = new List<WorkoutSet>() };

            we.Sets.Add(new WorkoutSet { Weight = 0, Reps = 0 });
            w.Exercises.Add(we);

            _workoutService.AddWorkout(w);

            var workouts = _workoutService.GetWorkoutsForUser(userId);
            bool nasliPodatke = false;

            foreach (var workout in workouts)
            {
                var ex = workout.Exercises.FirstOrDefault(x => x.ExerciseId == vajaId);
                if (ex != null && ex.Sets.Any())
                {
                    double max = ex.Sets.Max(s => s.Weight);
                    if (max > 0) nasliPodatke = true;
                }
            }
            Assert.IsFalse(nasliPodatke, "Če je teža 0, se ne sme prikazati na grafu.");     

        }
    }
}
