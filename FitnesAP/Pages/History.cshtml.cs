using FitnesAP.data;
using FitnesAP.Data;
using FitnesAP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitnesAP.Pages
{
    public class HistoryModel : PageModel
    {
        private readonly WorkoutService _workoutService;
        private readonly UserService _userService;

        public HistoryModel(WorkoutService workoutService, UserService userService)
        {
            _workoutService = workoutService;
            _userService = userService;
        }

        public List<Workout> CompletedWorkouts { get; set; } = new List<Workout>();

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username)) return RedirectToPage("/Login");

            var user = _userService.GetUserByUsername(username);
            if (user == null) return RedirectToPage("/Login");

            
            var allWorkouts = _workoutService.GetWorkoutsForUser(user.Id);

          
            CompletedWorkouts = allWorkouts                              
                                .OrderByDescending(w => w.Date) // Najnovejši na vrhu
                                .ToList();

            return Page();
        }
    }
}