namespace FitnesAP.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string SlikaUrl { get; set; } 
        public string VideoUrl { get; set; } 
        public string? Opis { get; set; }     
    }
}
