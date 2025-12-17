using FitnesAP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitnesAP.Pages
{
    public class BodyStatsModel : PageModel
    {
        private readonly UserService _userService;

        public BodyStatsModel(UserService userService)
        {
            _userService = userService;
        }

       
        [BindProperty] public double? InputTeza { get; set; }
        [BindProperty] public double? InputVisina { get; set; }
        [BindProperty] public DateTime? InputDatumRojstva { get; set; }

        public string Sporocilo { get; set; }
        public int? TrenutnaStarost { get; set; } 

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username)) return RedirectToPage("/Login");

            var user = _userService.GetUserByUsername(username);
            if (user == null) return RedirectToPage("/Login");

           
            InputTeza = user.Teza;
            InputVisina = user.Visina;
            InputDatumRojstva = user.DatumRojstva;
            TrenutnaStarost = user.Starost;

            return Page();
        }

        public IActionResult OnPost()
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username)) return RedirectToPage("/Login");

            var userInDb = _userService.GetUserByUsername(username);
            if (userInDb == null) return RedirectToPage("/Login");
            
            if ((InputTeza.HasValue && InputTeza < 0) || (InputVisina.HasValue && InputVisina < 0))
            {
                Sporocilo = "Napaka: Teža in višina ne smeta biti negativni!";
                
                TrenutnaStarost = userInDb.Starost;
                return Page(); 
            }

           
            if (InputDatumRojstva.HasValue && InputDatumRojstva.Value > DateTime.Now)
            {
                Sporocilo = "Napaka: Datum rojstva ne more biti v prihodnosti!";
                TrenutnaStarost = userInDb.Starost;
                return Page();
            }

           
            if ((InputTeza.HasValue && InputTeza > 500) || (InputVisina.HasValue && InputVisina > 300))
            {
                Sporocilo = "Napaka: Preveri vpisane vrednosti (previsoka teža ali višina).";
                TrenutnaStarost = userInDb.Starost;
                return Page();
            }
         
            userInDb.Teza = InputTeza;
            userInDb.Visina = InputVisina;
            userInDb.DatumRojstva = InputDatumRojstva;

            _userService.UpdateUser(userInDb);

            Sporocilo = "Meritve uspešno shranjene!";
            TrenutnaStarost = userInDb.Starost;

            return Page();
        }
    }
}