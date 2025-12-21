using FitnesAP.data;
using FitnesAP.Data;
using FitnesAP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitnesAP.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly UserService _userService;
        private readonly WorkoutService _workoutService; // Dodaj referenco na WorkoutService

        public ProfileModel(UserService userService, WorkoutService workoutService)
        {
            _userService = userService;
            _workoutService = workoutService;
        }

        [BindProperty]
        public User TrenutniUser { get; set; }
        public int SteviloOpravljenihTreningov { get; set; }
        public string Sporocilo { get; set; }

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username)) return RedirectToPage("/Login");

            TrenutniUser = _userService.GetUserByUsername(username);
            if (TrenutniUser == null) return RedirectToPage("/Login");
            SteviloOpravljenihTreningov = _workoutService.GetCompletedWorkoutsCount(TrenutniUser.Id);


            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            var vsiUporabniki = _userService.GetUsers();
            bool emailJeZaseden = vsiUporabniki.Any(u => u.Email != null && u.Email.ToLower() == TrenutniUser.Email.ToLower() && u.Id != TrenutniUser.Id);
            if (emailJeZaseden)
            {
                ModelState.AddModelError("TrenutniUser.Email", "Napaka, Email je že v uporabi");
                return Page();
            }
            _userService.UpdateUser(TrenutniUser);
            Sporocilo = "Podatki uspešno posodobljeni!";

            if (!string.IsNullOrEmpty(TrenutniUser.Ime))
            {
                HttpContext.Session.SetString("Ime", TrenutniUser.Ime);
            }

            return Page();
        }
    }
}