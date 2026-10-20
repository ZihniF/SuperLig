namespace League.BusinessLayer.Dtos
{
    public class TeamDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }
}
