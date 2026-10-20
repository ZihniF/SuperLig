using League.BusinessLayer.Dtos;
using League.EntityLayer.Concrete;

namespace League.BusinessLayer.Mapping
{
    public static class LeagueDtoMapper
    {
        public static TeamDto ToDto(Team team) => new()
        {
            TeamId = team.TeamId,
            TeamName = team.TeamName,
            ShortName = team.ShortName,
            City = team.City,
            Color = team.Color
        };

        public static StadiumDto ToDto(Stadium stadium) => new()
        {
            StadiumId = stadium.StadiumId,
            StadiumName = stadium.StadiumName,
            City = stadium.City
        };

        public static SeasonDto ToDto(Season season) => new()
        {
            SeasonId = season.SeasonId,
            SeasonName = season.SeasonName,
            DataUpdatedAt = season.DataUpdatedAt,
            SourceUrl = season.SourceUrl,
            LeagueId = season.LeagueId,
            League = new LeagueDto
            {
                LeagueId = season.League.LeagueId,
                LeagueName = season.League.LeagueName,
                Country = season.League.Country
            }
        };

        public static MatchDto ToDto(Match match) => new()
        {
            MatchId = match.MatchId,
            SeasonId = match.SeasonId,
            Season = match.Season == null
                ? new SeasonDto { SeasonId = match.SeasonId }
                : ToDto(match.Season),
            HomeTeamId = match.HomeTeamId,
            HomeTeam = ToDto(match.HomeTeam),
            AwayTeamId = match.AwayTeamId,
            AwayTeam = ToDto(match.AwayTeam),
            StadiumId = match.StadiumId,
            Stadium = match.Stadium == null
                ? new StadiumDto { StadiumName = "Stadyum henüz açıklanmadı" }
                : ToDto(match.Stadium),
            Week = match.Week,
            MatchDate = match.MatchDate,
            TimeConfirmed = match.TimeConfirmed,
            SourceMatchId = match.SourceMatchId,
            Status = match.Status,
            HomeScore = match.HomeScore,
            AwayScore = match.AwayScore,
            Statistic = match.Statistic == null ? null : ToDto(match.Statistic),
            Events = match.Events.Select(ToDto).ToList()
        };

        private static MatchStatisticDto ToDto(MatchStatistic statistic) => new()
        {
            MatchStatisticId = statistic.MatchStatisticId,
            MatchId = statistic.MatchId,
            FirstHalfHomeGoals = statistic.FirstHalfHomeGoals,
            FirstHalfAwayGoals = statistic.FirstHalfAwayGoals,
            SecondHalfHomeGoals = statistic.SecondHalfHomeGoals,
            SecondHalfAwayGoals = statistic.SecondHalfAwayGoals,
            HomePossession = statistic.HomePossession,
            HomeShots = statistic.HomeShots,
            AwayShots = statistic.AwayShots,
            HomeCorners = statistic.HomeCorners,
            AwayCorners = statistic.AwayCorners
        };

        private static MatchEventDto ToDto(MatchEvent matchEvent) => new()
        {
            MatchEventId = matchEvent.MatchEventId,
            MatchId = matchEvent.MatchId,
            TeamId = matchEvent.TeamId,
            Team = ToDto(matchEvent.Team),
            ActionType = matchEvent.ActionType,
            Minute = matchEvent.Minute,
            AddedMinute = matchEvent.AddedMinute,
            Description = matchEvent.Description
        };
    }
}
