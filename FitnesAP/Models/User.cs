using System;

namespace FitnesAP.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User";

        public string? Ime { get; set; }
        public string? Priimek { get; set; }
        public string? Email { get; set; }

        public double? Teza { get; set; }   
        public double? Visina { get; set; } 
        public DateTime? DatumRojstva { get; set; }       
        public int? Starost
        {
            get
            {
                if (DatumRojstva == null) return null;
                var today = DateTime.Today;
                var age = today.Year - DatumRojstva.Value.Year;               
                if (DatumRojstva.Value.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
    }
}
