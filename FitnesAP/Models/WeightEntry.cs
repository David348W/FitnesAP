namespace FitnesAP.Models
{
    public class WeightEntry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double Weight { get; set; }
        public DateTime Date {  get; set; } = DateTime.Now;
    }
}
