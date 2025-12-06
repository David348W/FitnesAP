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
    }
}
