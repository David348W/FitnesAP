using Microsoft.AspNetCore.Identity;

namespace FitnesAP.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Ime { get; set; }
        public double Teza { get; set; }
    }
}
