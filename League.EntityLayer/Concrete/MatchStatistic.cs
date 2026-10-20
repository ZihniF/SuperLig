namespace League.EntityLayer.Concrete
{
    public class MatchStatistic
    {
        public int MatchStatisticId { get; set; }
        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;
        public int? FirstHalfHomeGoals { get; set; }
        public int? FirstHalfAwayGoals { get; set; }
        public int? SecondHalfHomeGoals { get; set; }
        public int? SecondHalfAwayGoals { get; set; }
        public int? HomePossession { get; set; }
        public int? HomeShots { get; set; }
        public int? AwayShots { get; set; }
        public int? HomeCorners { get; set; }
        public int? AwayCorners { get; set; }
    }
}
