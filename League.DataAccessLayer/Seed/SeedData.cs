using League.DataAccessLayer.Context;
using League.EntityLayer.Concrete;
using League.EntityLayer.Enums;
using Microsoft.EntityFrameworkCore;

namespace League.DataAccessLayer.Seed
{
    public static class SeedData
    {
        public static async Task InitializeAsync(LeagueContext context)
        {
            await context.Database.MigrateAsync();

            var snapshot = CurrentSeasonSnapshot.Load();
            if (await context.Seasons.AnyAsync(x => x.DataVersion == snapshot.Version))
                return;

            var season = await context.Seasons.Include(x => x.League)
                .SingleOrDefaultAsync(x => x.SeasonName == "2026 / 27 · Demo");
            if (season == null && await context.Seasons.AnyAsync())
                return;

            await using var transaction = await context.Database.BeginTransactionAsync();
            if (season != null)
            {
                // Eski eğitim sezonu bir kez değiştirilir; sonraki açılışlar admin verisini korur.
                await context.Matches.Where(x => x.SeasonId == season.SeasonId).ExecuteDeleteAsync();
                await context.SeasonTeams.Where(x => x.SeasonId == season.SeasonId).ExecuteDeleteAsync();
                await context.Teams.Where(x => !context.SeasonTeams.Any(s => s.TeamId == x.TeamId)
                    && !context.Matches.Any(m => m.HomeTeamId == x.TeamId || m.AwayTeamId == x.TeamId)
                    && !context.MatchEvents.Any(e => e.TeamId == x.TeamId)).ExecuteDeleteAsync();
                await context.Stadiums.Where(x => !context.Matches.Any(m => m.StadiumId == x.StadiumId))
                    .ExecuteDeleteAsync();
            }
            else
            {
                season = new Season
                {
                    League = new EntityLayer.Concrete.League
                    {
                        LeagueName = "Trendyol Süper Lig",
                        Country = "Türkiye"
                    }
                };
                context.Seasons.Add(season);
            }

            season.SeasonName = snapshot.SeasonName;
            season.DataVersion = snapshot.Version;
            season.DataUpdatedAt = snapshot.UpdatedAt;
            season.SourceUrl = snapshot.SourceUrl;
            var teams = snapshot.Teams.ToDictionary(x => x.SourceId, x => new Team
            {
                TeamName = x.Name,
                ShortName = x.ShortName,
                City = x.City,
                Color = x.Color
            });
            var stadiums = snapshot.Stadiums.ToDictionary(x => x.SourceId, x => new Stadium
            {
                StadiumName = x.Name,
                City = x.City
            });
            context.Teams.AddRange(teams.Values);
            context.Stadiums.AddRange(stadiums.Values);
            await context.SaveChangesAsync();

            context.SeasonTeams.AddRange(teams.Values.Select(x => new SeasonTeam
            {
                SeasonId = season.SeasonId,
                TeamId = x.TeamId
            }));
            foreach (var item in snapshot.Matches)
            {
                var match = new Match
                {
                    SeasonId = season.SeasonId,
                    SourceMatchId = item.SourceMatchId,
                    HomeTeamId = teams[item.HomeSourceId].TeamId,
                    AwayTeamId = teams[item.AwaySourceId].TeamId,
                    StadiumId = item.StadiumSourceId.HasValue ? stadiums[item.StadiumSourceId.Value].StadiumId : null,
                    Week = item.Week,
                    MatchDate = item.MatchDate,
                    TimeConfirmed = item.TimeConfirmed,
                    Status = item.HomeScore.HasValue ? MatchStatus.Finished : MatchStatus.NotPlayed,
                    HomeScore = item.HomeScore,
                    AwayScore = item.AwayScore
                };
                if (item.FirstHalfHomeGoals.HasValue && item.FirstHalfAwayGoals.HasValue)
                {
                    match.Statistic = new MatchStatistic
                    {
                        FirstHalfHomeGoals = item.FirstHalfHomeGoals.Value,
                        FirstHalfAwayGoals = item.FirstHalfAwayGoals.Value,
                        SecondHalfHomeGoals = item.HomeScore!.Value - item.FirstHalfHomeGoals.Value,
                        SecondHalfAwayGoals = item.AwayScore!.Value - item.FirstHalfAwayGoals.Value
                    };
                }
                match.Events.AddRange(item.Events.Select(e => new MatchEvent
                {
                    TeamId = teams[e.TeamSourceId].TeamId,
                    ActionType = e.ActionType,
                    Minute = e.Minute,
                    AddedMinute = e.AddedMinute,
                    Description = e.Description
                }));
                context.Matches.Add(match);
            }
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    }
}
