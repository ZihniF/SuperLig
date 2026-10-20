using League.EntityLayer.Enums;

namespace League.EntityLayer.Concrete
{
    public class MatchEvent
    {
        public int MatchEventId { get; set; }
        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;
        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;
        public ActionType ActionType { get; set; }
        public int Minute { get; set; }
        public int AddedMinute { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
