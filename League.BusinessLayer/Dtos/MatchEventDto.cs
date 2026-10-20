using League.EntityLayer.Enums;

namespace League.BusinessLayer.Dtos
{
    public class MatchEventDto
    {
        public int MatchEventId { get; set; }
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public TeamDto Team { get; set; } = new();
        public ActionType ActionType { get; set; }
        public int Minute { get; set; }
        public int AddedMinute { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
