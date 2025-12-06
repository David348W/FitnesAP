using FitnesAP.Data;
using FitnesAP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitnesAP.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly UserService _userService;

        public ProfileModel(UserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public User TrenutniUser { get; set; }

        public string Sporocilo { get; set; }

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username)) return RedirectToPage("/Login");

            TrenutniUser = _userService.GetUserByUsername(username);
            if (TrenutniUser == null) return RedirectToPage("/Login");

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

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