using League.DataAccessLayer.Abstract;
using League.DataAccessLayer.Context;
using League.EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;

namespace League.DataAccessLayer.EntityFramework
{
    public class EfMatchDal : IMatchDal
    {
        private readonly LeagueContext _context;
        public EfMatchDal(LeagueContext context)
        {
            _context = context;
        }

        public Task<List<Match>> GetMatchesAsync(int seasonId) => _context.Matches.AsNoTracking().Include(x => x.HomeTeam).Include(x => x.AwayTeam).Include(x => x.Stadium).Where(x => x.SeasonId == seasonId).OrderBy(x => x.MatchDate).ThenBy(x => x.MatchId).ToListAsync();
        public Task<Match?> GetMatchByIdAsync(int id) => _context.Matches.Include(x => x.HomeTeam).Include(x => x.AwayTeam).Include(x => x.Stadium).Include(x => x.Season).ThenInclude(x => x.League).Include(x => x.Statistic).Include(x => x.Events).ThenInclude(x => x.Team).SingleOrDefaultAsync(x => x.MatchId == id);
        public async Task CreateMatchAsync(Match match)
        {
            _context.Matches.Add(match);
            await _context.SaveChangesAsync();
        }

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
        public Task<bool> FixtureExistsAsync(int seasonId, int homeTeamId, int awayTeamId) => _context.Matches.AnyAsync(x => x.SeasonId == seasonId && x.HomeTeamId == homeTeamId && x.AwayTeamId == awayTeamId);
    }

    public class EfLeagueDal : ILeagueDal
    {
        private readonly LeagueContext _context;
        public EfLeagueDal(LeagueContext context)
        {
            _context = context;
        }

        public Task<List<Season>> GetSeasonsAsync() => _context.Seasons.AsNoTracking().Include(x => x.League).OrderByDescending(x => x.SeasonId).ToListAsync();
        public Task<List<Team>> GetTeamsAsync(int seasonId) => _context.SeasonTeams.AsNoTracking().Where(x => x.SeasonId == seasonId).Select(x => x.Team).OrderBy(x => x.TeamName).ToListAsync();
        public Task<List<Stadium>> GetStadiumsAsync() => _context.Stadiums.AsNoTracking().OrderBy(x => x.StadiumName).ToListAsync();
    }
}
