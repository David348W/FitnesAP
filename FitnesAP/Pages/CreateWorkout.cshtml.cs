// Datoteka: Pages/CreateWorkout.cshtml.cs
using FitnesAP.data;
using FitnesAP.Data; // Pazi na velike/male črke pri 'Data'
using FitnesAP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitnesAP.Pages
{
    // 1. SPREMEMBA: Ime razreda
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
                ModelState.AddModelError("", "Prosim vpiši ime in izberi vaje.");
                return Page();
            }

            var newWorkout = new Workout
            {
                UserId = user.Id,
                Name = WorkoutName,
                Exercises = new List<Exercise>()
            };

            var allExercises = _exerciseService.GetExercises();
            foreach (var id in SelectedExerciseIds)
            {
                var exercise = allExercises.FirstOrDefault(e => e.Id == id);
                if (exercise != null) newWorkout.Exercises.Add(exercise);
            }

            _workoutService.AddWorkout(newWorkout);

            // 2. SPREMEMBA: Preusmeritev nazaj na nov seznam
            return RedirectToPage("/MyWorkouts");
        }
    }
}