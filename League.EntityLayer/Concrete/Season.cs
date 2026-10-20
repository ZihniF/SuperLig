namespace League.EntityLayer.Concrete
{
    public class Season
    {
        public int SeasonId { get; set; }
        public string SeasonName { get; set; } = string.Empty;
        public string? DataVersion { get; set; }
        public DateTime? DataUpdatedAt { get; set; }
        public string? SourceUrl { get; set; }
        public int LeagueId { get; set; }
        public League League { get; set; } = null!;
    }
}
