using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace League.DataAccessLayer.Context
{
    public class LeagueContextFactory : IDesignTimeDbContextFactory<LeagueContext>
    {
        public LeagueContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__LeagueConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                var configurationPaths = new[]
                {
                    Path.Combine(Directory.GetCurrentDirectory(), "League.WebUI", "appsettings.json"),
                    Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"),
                    Path.Combine(Directory.GetCurrentDirectory(), "..", "League.WebUI", "appsettings.json")
                };

                var configurationPath = configurationPaths.FirstOrDefault(File.Exists)
                    ?? throw new InvalidOperationException("League.WebUI/appsettings.json bulunamadı.");

                using var configuration = JsonDocument.Parse(File.ReadAllText(configurationPath));
                connectionString = configuration.RootElement
                    .GetProperty("ConnectionStrings")
                    .GetProperty("LeagueConnection")
                    .GetString();
            }

            var options = new DbContextOptionsBuilder<LeagueContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new LeagueContext(options);
        }
    }
}
