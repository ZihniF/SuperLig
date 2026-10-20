namespace League.EntityLayer.Concrete
{
    public class SeasonTeam
    {
        public int SeasonTeamId { get; set; }
        public int SeasonId { get; set; }
        public Season Season { get; set; } = null!;
        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;
    }
}
