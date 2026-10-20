namespace League.EntityLayer.Concrete
{
    public class Team
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Color { get; set; } = "#177a55";
    }
}
