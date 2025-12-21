using FitnesAP.Data;
using FitnesAP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitnesAP.Pages
{
    
    public class MyExercisesModel : PageModel
    {
        private readonly ExerciseService _exerciseService;
        private readonly IWebHostEnvironment _environment;
     
        public MyExercisesModel(ExerciseService exerciseService, IWebHostEnvironment environment)
        {
            _exerciseService = exerciseService;
            _environment = environment;
        }

        public List<Exercise> MojeVaje { get; set; } = new List<Exercise>();

        [BindProperty]
        public Exercise NovaVaja { get; set; }

        [BindProperty]
        public IFormFile? SlikaUpload { get; set; }

        public void OnGet()
        {
            var username = HttpContext.Session.GetString("username");         
            if (string.IsNullOrEmpty(username))
            {
                Response.Redirect("/Login");
                return;
            }

          
            var vse = _exerciseService.GetExercises(); //          
            MojeVaje = vse.Where(e => e.CreatedBy == username).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username)) return RedirectToPage("/Login");

            if (SlikaUpload != null)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + SlikaUpload.FileName;
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "slike");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await SlikaUpload.CopyToAsync(fileStream);
                }
                NovaVaja.SlikaUrl = "/slike/" + uniqueFileName;
            }
            else
            {
                NovaVaja.SlikaUrl = "";
            }
           
            NovaVaja.CreatedBy = username;        //

            _exerciseService.AddExercise(NovaVaja);

            return RedirectToPage();
        }
    }
}
