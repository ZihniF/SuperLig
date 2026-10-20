using System.ComponentModel.DataAnnotations;
using League.BusinessLayer.Abstract;
using League.BusinessLayer.Dtos;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace League.WebUI.Controllers
{
    [Authorize, AutoValidateAntiforgeryToken]
    public class AdminController : Controller
    {
        private readonly IMatchService _matchService;
        public AdminController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        public async Task<IActionResult> Index(int? seasonId)
        {
            var seasons = await _matchService.GetSeasonsAsync();
            var season = seasonId.HasValue ? seasons.SingleOrDefault(x => x.SeasonId == seasonId) : seasons.FirstOrDefault();
            if (season == null)
                return NotFound();
            ViewData["Seasons"] = seasons;
            ViewData["SeasonId"] = season.SeasonId;
            return View(new LeaguePageDto
            {
                Season = season,
                Matches = await _matchService.GetMatchesAsync(season.SeasonId)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? seasonId)
        {
            var seasons = await _matchService.GetSeasonsAsync();
            var season = seasonId.HasValue ? seasons.SingleOrDefault(x => x.SeasonId == seasonId) : seasons.FirstOrDefault();
            if (season == null)
                return NotFound();
            await LoadCreateAsync(season.SeasonId);
            return View(new CreateMatchDto
            {
                SeasonId = season.SeasonId
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMatchDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var id = await _matchService.CreateMatchAsync(dto);
                    TempData["Success"] = "Yeni maç fikstüre eklendi.";
                    return RedirectToAction("Result", new { id });
                }
                catch (ValidationException exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty, "Maç kaydedilemedi. Bu eşleşme daha önce eklenmiş olabilir.");
                }
            }

            await LoadCreateAsync(dto.SeasonId);
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Result(int id)
        {
            var match = await LoadMatchAsync(id);
            if (match == null)
                return NotFound();
            var stats = match.Statistic;
            return View(new UpdateMatchResultDto
            {
                MatchId = id,
                Status = match.Status,
                HomeScore = match.HomeScore,
                AwayScore = match.AwayScore,
                FirstHalfHomeGoals = stats?.FirstHalfHomeGoals,
                FirstHalfAwayGoals = stats?.FirstHalfAwayGoals,
                HomePossession = stats?.HomePossession,
                HomeShots = stats?.HomeShots,
                AwayShots = stats?.AwayShots,
                HomeCorners = stats?.HomeCorners,
                AwayCorners = stats?.AwayCorners
            });
        }

        [HttpPost]
        public async Task<IActionResult> Result(int id, UpdateMatchResultDto dto)
        {
            if (id != dto.MatchId)
                return BadRequest();
            if (await LoadMatchAsync(id) == null)
                return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    await _matchService.UpdateMatchResultAsync(dto);
                    TempData["Success"] = "Skor ve istatistikler kaydedildi. Puan durumu güncellendi.";
                    return RedirectToAction("Result", new { id });
                }
                catch (ValidationException exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Event(int id)
        {
            var match = await LoadMatchAsync(id);
            if (match == null)
                return NotFound();
            return View(new CreateMatchEventDto
            {
                MatchId = id,
                TeamId = match.HomeTeamId
            });
        }

        [HttpPost]
        public async Task<IActionResult> Event(int id, CreateMatchEventDto dto)
        {
            if (id != dto.MatchId)
                return BadRequest();
            if (await LoadMatchAsync(id) == null)
                return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    await _matchService.CreateMatchEventAsync(dto);
                    TempData["Success"] = "Maç olayı eklendi.";
                    return RedirectToAction("Event", new { id });
                }
                catch (ValidationException exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> EditEvent(int id, int eventId)
        {
            var match = await LoadMatchAsync(id);
            var matchEvent = match?.Events.SingleOrDefault(x => x.MatchEventId == eventId);
            if (matchEvent == null)
                return NotFound();

            return View("Event", new UpdateMatchEventDto
            {
                MatchId = id,
                MatchEventId = eventId,
                TeamId = matchEvent.TeamId,
                ActionType = matchEvent.ActionType,
                Minute = matchEvent.Minute,
                AddedMinute = matchEvent.AddedMinute,
                Description = matchEvent.Description
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditEvent(int id, int eventId, UpdateMatchEventDto dto)
        {
            if (id != dto.MatchId || eventId != dto.MatchEventId)
                return BadRequest();

            var match = await LoadMatchAsync(id);
            if (match == null || !match.Events.Any(x => x.MatchEventId == eventId))
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _matchService.UpdateMatchEventAsync(dto);
                    TempData["Success"] = "Maç olayı güncellendi.";
                    return RedirectToAction("Event", new { id });
                }
                catch (ValidationException exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            return View("Event", dto);
        }

        private async Task LoadCreateAsync(int seasonId)
        {
            ViewData["Seasons"] = await _matchService.GetSeasonsAsync();
            ViewData["SeasonId"] = seasonId;
            ViewData["Teams"] = await _matchService.GetTeamsAsync(seasonId);
            ViewData["Stadiums"] = await _matchService.GetStadiumsAsync();
        }

        private async Task<MatchDto?> LoadMatchAsync(int id)
        {
            var match = await _matchService.GetMatchByIdAsync(id);
            if (match != null)
            {
                ViewData["Match"] = match;
                ViewData["SeasonId"] = match.SeasonId;
                ViewData["Seasons"] = await _matchService.GetSeasonsAsync();
            }

            return match;
        }
    }
}
