using FitnesAP.data;
using FitnesAP.Data;
using FitnesAP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitnesAP.Pages
{
    public class TrackWorkoutModel : PageModel
    {
        private readonly WorkoutService _workoutService;

        public TrackWorkoutModel(WorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        [BindProperty]
        public Workout CurrentWorkout { get; set; }

        public IActionResult OnGet(int id)
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username)) return RedirectToPage("/Login");

            var workout = _workoutService.GetWorkoutById(id);
            if (workout == null) return RedirectToPage("/MyWorkouts");

            CurrentWorkout = workout;
            return Page();
        }

        // Glavni gumb "Zakljuèi trening"
        public IActionResult OnPost()
        {
            // Varnostno preverjanje
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username)) return RedirectToPage("/Login");

            var dbWorkout = _workoutService.GetWorkoutById(CurrentWorkout.Id);

            if (dbWorkout != null)
            {
                // 3. Posodobimo podatke, ki jih je uporabnik vpisal (teže, ponovitve)
                // Kopiramo iz CurrentWorkout (forma) v dbWorkout (baza)
                dbWorkout.Exercises = CurrentWorkout.Exercises;

                // 4. GLAVNI KORAK: Če EndTime še ni nastavljen, ga nastavimo ZDAJ.
                if (dbWorkout.EndTime == null)
                {
                    dbWorkout.EndTime = DateTime.Now;
                    dbWorkout.StartTime = null;
                }

                // 5. Shranimo v JSON
                _workoutService.UpdateWorkout(dbWorkout);

                // Posodobimo model za prikaz, da bo JavaScript takoj videl spremembo
                CurrentWorkout = dbWorkout;
            }

            return RedirectToPage("/MyWorkouts");
        }

        // --- NOVA METODA: Doda set doloèeni vaji ---
        // Uporabimo indeks vaje (exerciseIndex), da vemo, kateri vaji v seznamu dodati set
        public IActionResult OnPostAddSet(int exerciseIndex)
        {
            // Ker imamo [BindProperty], je CurrentWorkout že napolnjen s podatki iz forme
            var exercise = CurrentWorkout.Exercises[exerciseIndex];

            // Doloèimo nov ID seta (max obstojeè + 1)
            int newSetId = exercise.Sets.Count > 0 ? exercise.Sets.Max(s => s.Id) + 1 : 1;

            exercise.Sets.Add(new WorkoutSet
            {
                Id = newSetId,
                Weight = 0,
                Reps = 0
            });

            // Shranimo spremembe (tudi vpisane kile se shranijo)
            _workoutService.UpdateWorkout(CurrentWorkout);

            // Osvežimo stran (ohranimo ID treninga v URL-ju)
            return RedirectToPage(new { id = CurrentWorkout.Id });
        }

        // --- NOVA METODA: Odstrani set ---
        public IActionResult OnPostRemoveSet(int exerciseIndex, int setIndex)
        {
            var exercise = CurrentWorkout.Exercises[exerciseIndex];

            // Odstranimo set na doloèenem indeksu
            if (setIndex >= 0 && setIndex < exercise.Sets.Count)
            {
                exercise.Sets.RemoveAt(setIndex);
            }

            _workoutService.UpdateWorkout(CurrentWorkout);
            return RedirectToPage(new { id = CurrentWorkout.Id });
        }

        // V TrackWorkout.cshtml.cs dodaj tole novo metodo:

        public IActionResult OnPostStartTimer()
        {
            // Èe StartTime še ni nastavljen, ga nastavimo na zdaj
            if (CurrentWorkout.StartTime == null)
            {
                CurrentWorkout.StartTime = DateTime.Now;
                _workoutService.UpdateWorkout(CurrentWorkout);
            }

            // Osvežimo stran
            return RedirectToPage(new { id = CurrentWorkout.Id });
        }
    }
}