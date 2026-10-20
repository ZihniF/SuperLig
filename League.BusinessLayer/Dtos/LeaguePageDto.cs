using League.EntityLayer.Enums;

namespace League.BusinessLayer.Dtos
{
    public class LeaguePageDto
    {
        public SeasonDto Season { get; set; } = new();
        public List<TeamDto> Teams { get; set; } = new();
        public List<MatchDto> Matches { get; set; } = new();
        public List<StandingDto> Standings { get; set; } = new();
        public List<int> Weeks { get; set; } = new();
        public int? Week { get; set; }
        public MatchStatus? Status { get; set; }
        public int? TeamId { get; set; }
    }
}
