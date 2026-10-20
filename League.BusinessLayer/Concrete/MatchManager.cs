using System.ComponentModel.DataAnnotations;
using League.BusinessLayer.Abstract;
using League.BusinessLayer.Dtos;
using League.BusinessLayer.Mapping;
using League.DataAccessLayer.Abstract;
using League.EntityLayer.Concrete;
using League.EntityLayer.Enums;

namespace League.BusinessLayer.Concrete
{
    public class MatchManager : IMatchService
    {
        private readonly IMatchDal _matchDal;
        private readonly ILeagueDal _leagueDal;
        public MatchManager(IMatchDal matchDal, ILeagueDal leagueDal)
        {
            _matchDal = matchDal;
            _leagueDal = leagueDal;
        }

        public async Task<List<SeasonDto>> GetSeasonsAsync() =>
            (await _leagueDal.GetSeasonsAsync()).Select(LeagueDtoMapper.ToDto).ToList();

        public async Task<List<TeamDto>> GetTeamsAsync(int seasonId) =>
            (await _leagueDal.GetTeamsAsync(seasonId)).Select(LeagueDtoMapper.ToDto).ToList();

        public async Task<List<StadiumDto>> GetStadiumsAsync() =>
            (await _leagueDal.GetStadiumsAsync()).Select(LeagueDtoMapper.ToDto).ToList();

        public async Task<List<MatchDto>> GetMatchesAsync(int seasonId) =>
            (await _matchDal.GetMatchesAsync(seasonId)).Select(LeagueDtoMapper.ToDto).ToList();

        public async Task<MatchDto?> GetMatchByIdAsync(int id)
        {
            var match = await _matchDal.GetMatchByIdAsync(id);
            return match == null ? null : LeagueDtoMapper.ToDto(match);
        }
        public async Task<List<StandingDto>> GetStandingsAsync(int seasonId) => StandingsCalculator.Calculate(await _leagueDal.GetTeamsAsync(seasonId), await _matchDal.GetMatchesAsync(seasonId));
        public async Task<int> CreateMatchAsync(CreateMatchDto dto)
        {
            Validate(dto);
            Require((await _leagueDal.GetSeasonsAsync()).Any(x => x.SeasonId == dto.SeasonId), "Sezon bulunamadı.");
            var teams = await _leagueDal.GetTeamsAsync(dto.SeasonId);
            Require(dto.HomeTeamId != dto.AwayTeamId, "Ev sahibi ve deplasman aynı takım olamaz.");
            Require(teams.Any(x => x.TeamId == dto.HomeTeamId) && teams.Any(x => x.TeamId == dto.AwayTeamId), "Takımlar seçilen sezona kayıtlı olmalıdır.");
            Require((await _leagueDal.GetStadiumsAsync()).Any(x => x.StadiumId == dto.StadiumId), "Stadyum bulunamadı.");
            Require(dto.MatchDate.Year >= 2000 && dto.MatchDate.Year <= 2100, "Geçerli bir maç tarihi giriniz (2000–2100).");
            Require(!await _matchDal.FixtureExistsAsync(dto.SeasonId, dto.HomeTeamId, dto.AwayTeamId), "Bu sezon için aynı ev sahibi ve deplasman eşleşmesi zaten var.");
            var match = new Match

            {
                SeasonId = dto.SeasonId,
                HomeTeamId = dto.HomeTeamId,
                AwayTeamId = dto.AwayTeamId,
                StadiumId = dto.StadiumId,
                Week = dto.Week,
                MatchDate = dto.MatchDate,
                Status = MatchStatus.NotPlayed

            };
            await _matchDal.CreateMatchAsync(match);
            return match.MatchId;
        }

        public async Task UpdateMatchResultAsync(UpdateMatchResultDto dto)
        {
            Validate(dto);
            var match = await _matchDal.GetMatchByIdAsync(dto.MatchId) ?? throw new ValidationException("Maç bulunamadı.");
            if (dto.Status == MatchStatus.NotPlayed)
            {
                Require(dto.HomeScore == null && dto.AwayScore == null, "Oynanmamış maçın skor alanlarını boş bırakınız.");
                Require(match.Events.Count == 0, "Olay kaydı bulunan maç oynanmadı durumuna alınamaz.");
                match.Statistic = null;
            }
            else
            {
                Require(dto.HomeScore.HasValue && dto.AwayScore.HasValue, "Devam eden veya bitmiş maçın iki skoru da girilmelidir.");
                Require(dto.FirstHalfHomeGoals.HasValue == dto.FirstHalfAwayGoals.HasValue, "İlk yarı skorunu iki takım için de girin veya ikisini de boş bırakın.");
                Require((!dto.FirstHalfHomeGoals.HasValue || dto.FirstHalfHomeGoals <= dto.HomeScore)
                    && (!dto.FirstHalfAwayGoals.HasValue || dto.FirstHalfAwayGoals <= dto.AwayScore), "İlk yarı golleri toplam skordan büyük olamaz.");
                var stats = match.Statistic ?? new MatchStatistic

                {
                    MatchId = match.MatchId

                };
                stats.FirstHalfHomeGoals = dto.FirstHalfHomeGoals;
                stats.FirstHalfAwayGoals = dto.FirstHalfAwayGoals;
                stats.SecondHalfHomeGoals = dto.HomeScore!.Value - dto.FirstHalfHomeGoals;
                stats.SecondHalfAwayGoals = dto.AwayScore!.Value - dto.FirstHalfAwayGoals;
                stats.HomePossession = dto.HomePossession;
                stats.HomeShots = dto.HomeShots;
                stats.AwayShots = dto.AwayShots;
                stats.HomeCorners = dto.HomeCorners;
                stats.AwayCorners = dto.AwayCorners;
                match.Statistic = stats;
            }

            match.Status = dto.Status;
            match.HomeScore = dto.HomeScore;
            match.AwayScore = dto.AwayScore;
            await _matchDal.SaveChangesAsync();
        }

        public async Task CreateMatchEventAsync(CreateMatchEventDto dto)
        {
            Validate(dto);
            var match = await _matchDal.GetMatchByIdAsync(dto.MatchId) ?? throw new ValidationException("Maç bulunamadı.");
            ValidateEventForMatch(match, dto);
            match.Events.Add(new MatchEvent
            {
                MatchId = dto.MatchId,
                TeamId = dto.TeamId,
                Minute = dto.Minute,
                AddedMinute = dto.AddedMinute,
                ActionType = dto.ActionType,
                Description = dto.Description.Trim()
            });
            await _matchDal.SaveChangesAsync();
        }

        public async Task UpdateMatchEventAsync(UpdateMatchEventDto dto)
        {
            Validate(dto);
            var match = await _matchDal.GetMatchByIdAsync(dto.MatchId)
                ?? throw new ValidationException("Maç bulunamadı.");
            var matchEvent = match.Events.SingleOrDefault(x => x.MatchEventId == dto.MatchEventId)
                ?? throw new ValidationException("Bu maça ait olay bulunamadı.");
            ValidateEventForMatch(match, dto);

            matchEvent.TeamId = dto.TeamId;
            matchEvent.ActionType = dto.ActionType;
            matchEvent.Minute = dto.Minute;
            matchEvent.AddedMinute = dto.AddedMinute;
            matchEvent.Description = dto.Description.Trim();
            await _matchDal.SaveChangesAsync();
        }

        private static void ValidateEventForMatch(Match match, CreateMatchEventDto dto)
        {
            Require(match.Status != MatchStatus.NotPlayed, "Olay kaydı için önce maçı başlatın veya sonucunu girin.");
            Require(dto.TeamId == match.HomeTeamId || dto.TeamId == match.AwayTeamId, "Olayın takımı bu maçta yer almalıdır.");
            Require(dto.AddedMinute == 0 || new[] { 45, 90, 105, 120 }.Contains(dto.Minute), "Uzatma dakikası için ana dakika 45, 90, 105 veya 120 olmalıdır.");
            Require(!string.IsNullOrWhiteSpace(dto.Description), "Açıklama boş olamaz.");
        }

        private static void Validate(object dto) => Validator.ValidateObject(dto, new ValidationContext(dto), true);
        private static void Require(bool condition, string message)
        {
            if (!condition)
                throw new ValidationException(message);
        }
    }
}
