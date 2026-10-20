using League.BusinessLayer.Dtos;

namespace League.BusinessLayer.Abstract
{
    public interface IMatchService
    {
        Task<List<SeasonDto>> GetSeasonsAsync();
        Task<List<TeamDto>> GetTeamsAsync(int seasonId);
        Task<List<StadiumDto>> GetStadiumsAsync();
        Task<List<MatchDto>> GetMatchesAsync(int seasonId);
        Task<MatchDto?> GetMatchByIdAsync(int id);
        Task<List<StandingDto>> GetStandingsAsync(int seasonId);
        Task<int> CreateMatchAsync(CreateMatchDto dto);
        Task UpdateMatchResultAsync(UpdateMatchResultDto dto);
        Task CreateMatchEventAsync(CreateMatchEventDto dto);
        Task UpdateMatchEventAsync(UpdateMatchEventDto dto);
    }
}
