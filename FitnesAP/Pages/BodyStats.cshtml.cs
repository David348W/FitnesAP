using FitnesAP.data;
using FitnesAP.Data;
using FitnesAP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FitnesAP.Pages
{
    public class BodyStatsModel : PageModel
    {
        private readonly UserService _userService;
        private readonly WeightHistoryService _weightHistoryService;

        public BodyStatsModel(UserService userService, WeightHistoryService weightHistoryService)
        {
            _userService = userService;
            _weightHistoryService = weightHistoryService;
        }

        [BindProperty] public double? InputTeza { get; set; }
        [BindProperty] public double? InputVisina { get; set; }
        [BindProperty] public DateTime? InputDatumRojstva { get; set; }

        public string Sporocilo { get; set; }
        public int? TrenutnaStarost { get; set; }      
        public string ChartLabelsJson { get; set; } = "[]";
        public string ChartDataJson { get; set; } = "[]";

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
            NaloziPodatkeZaGraf(user.Id);

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
                NaloziPodatkeZaGraf(userInDb.Id); 
                return Page();
            }

          
            if (InputDatumRojstva.HasValue && InputDatumRojstva.Value > DateTime.Now)
            {
                Sporocilo = "Napaka: Datum rojstva ne more biti v prihodnosti!";
                TrenutnaStarost = userInDb.Starost;
                NaloziPodatkeZaGraf(userInDb.Id);
                return Page();
            }

         
            if ((InputTeza.HasValue && InputTeza > 500) || (InputVisina.HasValue && InputVisina > 300))
            {
                Sporocilo = "Napaka: Preveri vpisane vrednosti (previsoka teža ali višina).";
                TrenutnaStarost = userInDb.Starost;
                NaloziPodatkeZaGraf(userInDb.Id);
                return Page();
            }

           
            userInDb.Teza = InputTeza;
            userInDb.Visina = InputVisina;
            userInDb.DatumRojstva = InputDatumRojstva;

            _userService.UpdateUser(userInDb);

            if (InputTeza.HasValue)
            {
                _weightHistoryService.AddEntry(userInDb.Id, InputTeza.Value);
            }

            Sporocilo = "Meritve in zgodovina uspešno shranjene!";
            TrenutnaStarost = userInDb.Starost;          
            NaloziPodatkeZaGraf(userInDb.Id);

            return Page();
        }     
        private void NaloziPodatkeZaGraf(int userId)
        {
            var history = _weightHistoryService.GetHistoryForUsers(userId);

            if (history != null && history.Count > 0)
            {               
                var datumi = history.Select(h => h.Date.ToString("dd.MM.yyyy")).ToList();
                var teze = history.Select(h => h.Weight).ToList();

                ChartLabelsJson = JsonSerializer.Serialize(datumi);
                ChartDataJson = JsonSerializer.Serialize(teze);
            }
            else
            {
                ChartLabelsJson = "[]";
                ChartDataJson = "[]";
            }
        }
    }
}