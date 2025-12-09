using FitnesAP.data;
using FitnesAP.Data;
using FitnesAP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitnesAP.Pages
{
    public class MyWorkoutsModel : PageModel
    {
        private readonly WorkoutService _workoutService;
        private readonly UserService _userService;

        public MyWorkoutsModel(WorkoutService workoutService, UserService userService)
        {
            _workoutService = workoutService;
            _userService = userService;
        }

        public List<Workout> UserWorkouts { get; set; } = new List<Workout>();

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username)) return RedirectToPage("/Login");

            var user = _userService.GetUserByUsername(username);
            if (user == null) return RedirectToPage("/Login");

            // Dobimo treninge in jih obrnemo (najnovejši na vrhu)
            UserWorkouts = _workoutService.GetWorkoutsForUser(user.Id);
            UserWorkouts.Reverse();

            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            // Varnostno preverjanje prijave
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username)) return RedirectToPage("/Login");

            // Izbriši trening
            _workoutService.DeleteWorkout(id);

            // Osveži stran
            return RedirectToPage();
        }
    }
}