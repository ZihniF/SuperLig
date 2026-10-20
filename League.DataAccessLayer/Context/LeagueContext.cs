using League.EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;

namespace League.DataAccessLayer.Context
{
    public class LeagueContext : DbContext
    {
        public LeagueContext(DbContextOptions<LeagueContext> options) : base(options)
        {
        }

        public DbSet<EntityLayer.Concrete.League> Leagues => Set<EntityLayer.Concrete.League>();
        public DbSet<Season> Seasons => Set<Season>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<SeasonTeam> SeasonTeams => Set<SeasonTeam>();
        public DbSet<Stadium> Stadiums => Set<Stadium>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<MatchEvent> MatchEvents => Set<MatchEvent>();
        public DbSet<MatchStatistic> MatchStatistics => Set<MatchStatistic>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (Database.IsSqlServer())
                modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Entity<SeasonTeam>().HasIndex(x => new { x.SeasonId, x.TeamId }).IsUnique();
            modelBuilder.Entity<Match>().HasOne(x => x.HomeTeam).WithMany().HasForeignKey(x => x.HomeTeamId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Match>().HasOne(x => x.AwayTeam).WithMany().HasForeignKey(x => x.AwayTeamId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Match>().HasOne(x => x.Statistic).WithOne(x => x.Match).HasForeignKey<MatchStatistic>(x => x.MatchId);
            modelBuilder.Entity<Match>().HasMany(x => x.Events).WithOne(x => x.Match).HasForeignKey(x => x.MatchId);
            modelBuilder.Entity<MatchEvent>().HasOne(x => x.Team).WithMany().HasForeignKey(x => x.TeamId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Match>().HasIndex(x => new { x.SeasonId, x.HomeTeamId, x.AwayTeamId }).IsUnique();
            modelBuilder.Entity<Match>().Property(x => x.Status).IsRequired();
            modelBuilder.Entity<Match>().HasIndex(x => x.SourceMatchId).IsUnique();
            modelBuilder.Entity<Match>().ToTable(t =>
            {
                t.HasCheckConstraint("CK_Match_Teams", "HomeTeamId <> AwayTeamId");
                t.HasCheckConstraint("CK_Match_Status", "Status IN (0,1,2)");
                t.HasCheckConstraint("CK_Match_Scores", "(Status = 0 AND HomeScore IS NULL AND AwayScore IS NULL) OR (Status IN (1,2) AND HomeScore IS NOT NULL AND AwayScore IS NOT NULL AND HomeScore >= 0 AND AwayScore >= 0)");
            });
            modelBuilder.Entity<MatchEvent>().ToTable(t =>
            {
                t.HasCheckConstraint("CK_Event_Minute", "Minute BETWEEN 0 AND 130");
                t.HasCheckConstraint("CK_Event_AddedMinute", "AddedMinute BETWEEN 0 AND 30 AND (AddedMinute = 0 OR Minute IN (45,90,105,120))");
                t.HasCheckConstraint("CK_Event_Action", "ActionType IN (1,2,3,4)");
            });
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
                foreach (var property in entity.GetProperties().Where(x => x.ClrType == typeof(string)))
                    property.SetMaxLength(200);
        }
    }
}
