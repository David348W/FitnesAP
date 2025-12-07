using FitnesAP.Data;
using FitnesAP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitnesAP.Pages
{
    public class ExercisesModel : PageModel
    {
        private readonly ExerciseService _exerciseService;
        private readonly UserService _userService;      
        private readonly IWebHostEnvironment _environment;

        public ExercisesModel(ExerciseService exerciseService, UserService userService, IWebHostEnvironment environment)
        {
            _exerciseService = exerciseService;
            _userService = userService;
            _environment = environment;
        }

        public List<Exercise> VseVaje { get; set; } = new List<Exercise>();
        public bool IsAdmin { get; set; } = false;

        [BindProperty]
        public Exercise NovaVaja { get; set; }

       
        [BindProperty]
        public IFormFile? SlikaUpload { get; set; }

        public void OnGet()
        {
            VseVaje = _exerciseService.GetExercises();

            var username = HttpContext.Session.GetString("username");
            if (!string.IsNullOrEmpty(username))
            {
                var user = _userService.GetUserByUsername(username);
              
                if (user != null && user.Role == "Admin")
                {
                    IsAdmin = true;
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
           
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
            _exerciseService.AddExercise(NovaVaja);

            return RedirectToPage();
        }
    }
}
