using League.EntityLayer.Enums;

namespace League.EntityLayer.Concrete
{
    public class Match
    {
        public int MatchId { get; set; }
        public int SeasonId { get; set; }
        public Season Season { get; set; } = null!;
        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; } = null!;
        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; } = null!;
        public int? StadiumId { get; set; }
        public Stadium? Stadium { get; set; }
        public int Week { get; set; }
        public DateTime MatchDate { get; set; }
        public bool TimeConfirmed { get; set; } = true;
        public int? SourceMatchId { get; set; }
        public MatchStatus Status { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public MatchStatistic? Statistic { get; set; }
        public List<MatchEvent> Events { get; set; } = new();
    }
}
