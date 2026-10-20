using League.EntityLayer.Concrete;

namespace League.DataAccessLayer.Abstract
{
    public interface IMatchDal
    {
        Task<List<Match>> GetMatchesAsync(int seasonId);
        Task<Match?> GetMatchByIdAsync(int id);
        Task CreateMatchAsync(Match match);
        Task SaveChangesAsync();
        Task<bool> FixtureExistsAsync(int seasonId, int homeTeamId, int awayTeamId);
    }

    public interface ILeagueDal
    {
        Task<List<Season>> GetSeasonsAsync();
        Task<List<Team>> GetTeamsAsync(int seasonId);
        Task<List<Stadium>> GetStadiumsAsync();
    }
}
