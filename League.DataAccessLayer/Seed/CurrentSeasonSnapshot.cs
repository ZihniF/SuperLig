using System.Text.Json;
using League.EntityLayer.Enums;

namespace League.DataAccessLayer.Seed
{
    public sealed class CurrentSeasonSnapshot
    {
        public string Version { get; set; } = string.Empty;
        public string SeasonName { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string SourceUrl { get; set; } = string.Empty;
        public List<SnapshotTeam> Teams { get; set; } = new();
        public List<SnapshotStadium> Stadiums { get; set; } = new();
        public List<SnapshotMatch> Matches { get; set; } = new();
        public List<SnapshotStanding> ExpectedStandings { get; set; } = new();

        public static CurrentSeasonSnapshot Load()
        {
            using var stream = typeof(CurrentSeasonSnapshot).Assembly
                .GetManifestResourceStream("League.DataAccessLayer.Data.current-season.json")
                ?? throw new InvalidOperationException("Güncel sezon veri dosyası bulunamadı.");
            var snapshot = JsonSerializer.Deserialize<CurrentSeasonSnapshot>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidOperationException("Sezon verisi okunamadı.");
            var teamIds = snapshot.Teams.Select(x => x.SourceId).ToHashSet();
            if (teamIds.Count != 18 || snapshot.Matches.Count != 306
                || snapshot.Matches.Select(x => x.SourceMatchId).Distinct().Count() != 306
                || snapshot.Matches.Any(x => !teamIds.Contains(x.HomeSourceId)
                    || !teamIds.Contains(x.AwaySourceId) || x.HomeSourceId == x.AwaySourceId
                    || x.HomeScore.HasValue != x.AwayScore.HasValue)
                || snapshot.Matches.Sum(x => x.Events.Count) is < 10 or > 20)
                throw new InvalidOperationException("Sezon veri dosyasının bütünlük kontrolü başarısız.");
            return snapshot;
        }
    }

    public sealed class SnapshotTeam
    {
        public int SourceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string OfficialName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public sealed class SnapshotStadium
    {
        public int SourceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public sealed class SnapshotMatch
    {
        public int SourceMatchId { get; set; }
        public int HomeSourceId { get; set; }
        public int AwaySourceId { get; set; }
        public int? StadiumSourceId { get; set; }
        public int Week { get; set; }
        public DateTime MatchDate { get; set; }
        public bool TimeConfirmed { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public int? FirstHalfHomeGoals { get; set; }
        public int? FirstHalfAwayGoals { get; set; }
        public List<SnapshotEvent> Events { get; set; } = new();
    }

    public sealed class SnapshotEvent
    {
        public int TeamSourceId { get; set; }
        public ActionType ActionType { get; set; }
        public int Minute { get; set; }
        public int AddedMinute { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public sealed class SnapshotStanding
    {
        public int SourceId { get; set; }
        public int Played { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int Points { get; set; }
    }
}
