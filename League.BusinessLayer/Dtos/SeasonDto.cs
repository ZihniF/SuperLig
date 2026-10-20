namespace League.BusinessLayer.Dtos
{
    public class SeasonDto
    {
        public int SeasonId { get; set; }
        public string SeasonName { get; set; } = string.Empty;
        public DateTime? DataUpdatedAt { get; set; }
        public string? SourceUrl { get; set; }
        public int LeagueId { get; set; }
        public LeagueDto League { get; set; } = new();
    }

    public class LeagueDto
    {
        public int LeagueId { get; set; }
        public string LeagueName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
