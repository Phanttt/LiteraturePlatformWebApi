namespace LiteraturePlatformWebApi.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int CompositionId { get; set; }
        public Composition? Composition { get; set; }
        public int Rate { get; set; }
    }
}
