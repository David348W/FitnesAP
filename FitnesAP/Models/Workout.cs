using System;
using System.Collections.Generic;

namespace FitnesAP.Models
{
    public class Workout
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }

        // --- NOVO: Kdaj smo pritisnili gumb začni ---
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public List<WorkoutExercise> Exercises { get; set; } = new List<WorkoutExercise>();
    }
}