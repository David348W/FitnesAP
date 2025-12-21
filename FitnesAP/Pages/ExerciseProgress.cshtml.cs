using FitnesAP.data;
using FitnesAP.Data;
using FitnesAP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FitnesAP.Pages
{
    public class ExerciseProgressModel : PageModel
    {
        private readonly WorkoutService _workoutService;
        private readonly ExerciseService _exerciseService;
        private readonly UserService _userService;
        public ExerciseProgressModel(WorkoutService workoutService, ExerciseService exerciseService, UserService userService)
        {
            _workoutService = workoutService;
            _exerciseService = exerciseService;
            _userService = userService;
        }

        public List<Exercise> AllExercises { get; set; }
        public Exercise SelectedExercise { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedExerciseID {  get; set; }

        public string ChartLabelsJson { get; set; } = "[]";
        public string ChartDataJson { get; set; } = "[]";

        public bool ImaPodatke { get; set; } = false;
        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username)) return RedirectToPage("/Login");

            var user = _userService.GetUserByUsername(username);
            if (user == null) return RedirectToPage("/Login");

            int userId = user.Id;
            var vseVaje = _exerciseService.GetExercises();
            AllExercises = vseVaje.Where(e =>
                string.IsNullOrEmpty(e.CreatedBy) ||
                e.CreatedBy == username
            ).ToList();

            if (SelectedExerciseID.HasValue)
            {
                SelectedExercise = AllExercises.FirstOrDefault(x => x.Id == SelectedExerciseID.Value);
                if(SelectedExercise != null)
                {
                    var workouts = _workoutService.GetWorkoutsForUser(userId);
                    var dataPoints = new List<Double>();
                    var labels = new List<String>();

                    foreach(var workout in workouts.OrderBy(w => w.Date))
                    {
                        var exercisesWorkout = workout.Exercises
                            .FirstOrDefault(x => x.ExerciseId == SelectedExerciseID.Value);

                        if (exercisesWorkout != null && exercisesWorkout.Sets.Any()) 
                        {
                            double maxWeight = exercisesWorkout.Sets.Max(s => s.Weight);

                            if(maxWeight > 0)
                            {
                                labels.Add(workout.Date.ToString("dd.MM.yyyy"));
                                dataPoints.Add(maxWeight);

                            }
                        }
                    }
                    if (dataPoints.Count > 0)
                    {
                        ImaPodatke = true;
                        ChartLabelsJson = JsonSerializer.Serialize(labels);
                        ChartDataJson = JsonSerializer.Serialize(dataPoints);
                    }
                }
            }
            return Page();
        }
        public IActionResult OnPost()
        {
            return RedirectToPage(new { SelectedExerciseId = SelectedExerciseID });
        }
    }
}
