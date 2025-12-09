using System;
using System.Collections.Generic;

namespace FitnesAP.Models
{
    public class Workout
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Kdo je naredil trening
        public string Name { get; set; } // Npr. "Push Day

        // Seznam vaj, ki so v tem treningu
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
