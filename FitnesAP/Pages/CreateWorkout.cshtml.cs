using FitnesAP.data;
using FitnesAP.Data;
using FitnesAP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitnesAP.Pages
{
    public class CreateWorkoutModel : PageModel
    {
        private readonly ExerciseService _exerciseService;
        private readonly WorkoutService _workoutService;
        private readonly UserService _userService;

        public CreateWorkoutModel(ExerciseService exerciseService, WorkoutService workoutService, UserService userService)
        {
            _exerciseService = exerciseService;
            _workoutService = workoutService;
            _userService = userService;
        }

        public List<Exercise> AllExercises { get; set; } = new List<Exercise>();

        [BindProperty]
        public string WorkoutName { get; set; }

        [BindProperty]
        public List<int> SelectedExerciseIds { get; set; } = new List<int>();

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username)) return RedirectToPage("/Login");

            AllExercises = _exerciseService.GetExercises();
            return Page();
        }

        public IActionResult OnPost()
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username)) return RedirectToPage("/Login");

            var user = _userService.GetUserByUsername(username);
            if (user == null) return RedirectToPage("/Login");

            if (string.IsNullOrEmpty(WorkoutName) || SelectedExerciseIds.Count == 0)
            {
                AllExercises = _exerciseService.GetExercises();
                ModelState.AddModelError("", "Izberi vsaj eno vajo in vpiši ime.");
                return Page();
            }

            var newWorkout = new Workout
            {
                UserId = user.Id,
                Name = WorkoutName,
                // TUKAJ JE KLJUČ: Uporabljamo WorkoutExercise
                Exercises = new List<WorkoutExercise>()
            };

            var allExercises = _exerciseService.GetExercises();

            foreach (var id in SelectedExerciseIds)
            {
                var exercise = allExercises.FirstOrDefault(e => e.Id == id);
                if (exercise != null)
                {
                    // Ustvarimo vajo z ENIM privzetim setom
                    var workoutExercise = new WorkoutExercise
                    {
                        ExerciseId = exercise.Id,
                        Name = exercise.Ime, // Preveri če imaš v Exercise modelu 'Name' ali 'Ime'
                        Sets = new List<WorkoutSet>
                        {
                            new WorkoutSet { Id = 1, Weight = 0, Reps = 0 }
                        }
                    };
                    newWorkout.Exercises.Add(workoutExercise);
                }
            }

            _workoutService.AddWorkout(newWorkout);

            return RedirectToPage("/MyWorkouts");
        }
    }
}