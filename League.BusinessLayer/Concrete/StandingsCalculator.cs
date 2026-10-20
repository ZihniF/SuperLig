using League.BusinessLayer.Dtos;
using League.EntityLayer.Concrete;
using League.EntityLayer.Enums;

namespace League.BusinessLayer.Concrete
{
    public static class StandingsCalculator
    {
        public static List<StandingDto> Calculate(IEnumerable<Team> teams, IEnumerable<Match> matches)
        {
            var rows = teams.ToDictionary(x => x.TeamId, x => new StandingDto
            {
                TeamId = x.TeamId,
                TeamName = x.TeamName,
                ShortName = x.ShortName,
                Color = x.Color
            });
            foreach (var match in matches.Where(x => x.Status == MatchStatus.Finished && x.HomeScore.HasValue && x.AwayScore.HasValue).OrderBy(x => x.MatchDate).ThenBy(x => x.MatchId))
            {
                if (!rows.TryGetValue(match.HomeTeamId, out var home) || !rows.TryGetValue(match.AwayTeamId, out var away))
                    continue;
                home.Played++;
                away.Played++;
                home.GoalsFor += match.HomeScore!.Value;
                home.GoalsAgainst += match.AwayScore!.Value;
                away.GoalsFor += match.AwayScore.Value;
                away.GoalsAgainst += match.HomeScore.Value;
                if (match.HomeScore > match.AwayScore)
                {
                    home.Wins++;
                    away.Losses++;
                    home.Form.Add("W");
                    away.Form.Add("L");
                }
                else if (match.HomeScore < match.AwayScore)
                {
                    away.Wins++;
                    home.Losses++;
                    away.Form.Add("W");
                    home.Form.Add("L");
                }
                else
                {
                    home.Draws++;
                    away.Draws++;
                    home.Form.Add("D");
                    away.Form.Add("D");
                }
            }

            var result = rows.Values.OrderByDescending(x => x.Points).ThenByDescending(x => x.GoalDifference).ThenByDescending(x => x.GoalsFor).ThenBy(x => x.TeamName, StringComparer.Ordinal).ToList();
            for (var index = 0; index < result.Count; index++)
            {
                result[index].Position = index + 1;
                result[index].Form = result[index].Form.TakeLast(5).ToList();
            }

            return result;
        }
    }
}
