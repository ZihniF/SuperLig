using System.Diagnostics;
using League.BusinessLayer.Abstract;
using League.EntityLayer.Enums;
using League.BusinessLayer.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace League.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMatchService _matchService;
        public HomeController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        public async Task<IActionResult> Index(int? seasonId)
        {
            var model = await BuildPageAsync(seasonId);
            return model == null ? NotFound() : View(model);
        }

        public async Task<IActionResult> Standings(int? seasonId)
        {
            var model = await BuildPageAsync(seasonId);
            return model == null ? NotFound() : View(model);
        }

        public async Task<IActionResult> Fixtures(int? seasonId, int? week, MatchStatus? status, int? teamId)
        {
            var model = await BuildPageAsync(seasonId);
            if (model == null)
                return NotFound();
            model.Week = week;
            model.Status = status;
            model.TeamId = teamId;
            model.Weeks = model.Matches.Select(x => x.Week).Distinct().Order().ToList();
            model.Matches = model.Matches.Where(x => (!week.HasValue || x.Week == week) && (!status.HasValue || x.Status == status) && (!teamId.HasValue || x.HomeTeamId == teamId || x.AwayTeamId == teamId)).ToList();
            return View(model);
        }

        public async Task<IActionResult> Teams(int? seasonId)
        {
            var model = await BuildPageAsync(seasonId);
            return model == null ? NotFound() : View(model);
        }

        public async Task<IActionResult> Match(int id)
        {
            var match = await _matchService.GetMatchByIdAsync(id);
            if (match == null)
                return NotFound();
            ViewData["Seasons"] = await _matchService.GetSeasonsAsync();
            ViewData["SeasonId"] = match.SeasonId;
            return View(match);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorDto
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
        private async Task<LeaguePageDto?> BuildPageAsync(int? seasonId)
        {
            var seasons = await _matchService.GetSeasonsAsync();
            var season = seasonId.HasValue ? seasons.SingleOrDefault(x => x.SeasonId == seasonId) : seasons.FirstOrDefault();
            if (season == null)
                return null;
            ViewData["Seasons"] = seasons;
            ViewData["SeasonId"] = season.SeasonId;
            return new LeaguePageDto

            {
                Season = season,
                Teams = await _matchService.GetTeamsAsync(season.SeasonId),
                Matches = await _matchService.GetMatchesAsync(season.SeasonId),
                Standings = await _matchService.GetStandingsAsync(season.SeasonId)

            };
        }
    }
}
