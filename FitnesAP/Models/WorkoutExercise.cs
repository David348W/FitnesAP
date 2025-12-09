namespace FitnesAP.Models
{
    public class WorkoutExercise
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; } // Ime vaje kopiramo sem, da je lažje

        // Vsaka vaja ima seznam serij (npr. 3 serije)
        public List<WorkoutSet> Sets { get; set; } = new List<WorkoutSet>();
    }
}
