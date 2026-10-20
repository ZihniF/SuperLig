using League.EntityLayer.Enums;

namespace League.BusinessLayer.Dtos
{
    public class MatchDto
    {
        public int MatchId { get; set; }
        public int SeasonId { get; set; }
        public SeasonDto Season { get; set; } = new();
        public int HomeTeamId { get; set; }
        public TeamDto HomeTeam { get; set; } = new();
        public int AwayTeamId { get; set; }
        public TeamDto AwayTeam { get; set; } = new();
        public int? StadiumId { get; set; }
        public StadiumDto Stadium { get; set; } = new();
        public int Week { get; set; }
        public DateTime MatchDate { get; set; }
        public bool TimeConfirmed { get; set; }
        public int? SourceMatchId { get; set; }
        public MatchStatus Status { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public MatchStatisticDto? Statistic { get; set; }
        public List<MatchEventDto> Events { get; set; } = new();
    }
}
