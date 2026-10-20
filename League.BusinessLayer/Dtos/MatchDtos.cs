using System.ComponentModel.DataAnnotations;
using League.EntityLayer.Enums;

namespace League.BusinessLayer.Dtos
{
    public class CreateMatchDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Sezon seçiniz.")]
        public int SeasonId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Ev sahibi seçiniz.")]
        public int HomeTeamId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Deplasman seçiniz.")]
        public int AwayTeamId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Stadyum seçiniz.")]
        public int StadiumId { get; set; }

        [Range(1, 52, ErrorMessage = "Hafta 1–52 arasında olmalıdır.")]
        public int Week { get; set; } = 1;

        [Required]
        public DateTime MatchDate { get; set; } = DateTime.Today.AddDays(1).AddHours(20);
    }

    public class UpdateMatchResultDto
    {
        [Range(1, int.MaxValue)]
        public int MatchId { get; set; }

        [EnumDataType(typeof(MatchStatus), ErrorMessage = "Geçerli bir maç durumu seçiniz.")]
        public MatchStatus Status { get; set; }

        [Range(0, 99, ErrorMessage = "Skor 0–99 arasında olmalıdır.")]
        public int? HomeScore { get; set; }

        [Range(0, 99, ErrorMessage = "Skor 0–99 arasında olmalıdır.")]
        public int? AwayScore { get; set; }

        [Range(0, 99)]
        public int? FirstHalfHomeGoals { get; set; }

        [Range(0, 99)]
        public int? FirstHalfAwayGoals { get; set; }

        [Range(0, 100)]
        public int? HomePossession { get; set; }

        [Range(0, 100)]
        public int? HomeShots { get; set; }

        [Range(0, 100)]
        public int? AwayShots { get; set; }

        [Range(0, 100)]
        public int? HomeCorners { get; set; }

        [Range(0, 100)]
        public int? AwayCorners { get; set; }
    }

    public class CreateMatchEventDto
    {
        [Range(1, int.MaxValue)]
        public int MatchId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Takım seçiniz.")]
        public int TeamId { get; set; }

        [EnumDataType(typeof(ActionType), ErrorMessage = "Geçerli bir olay türü seçiniz.")]
        public ActionType ActionType { get; set; } = ActionType.Goal;

        [Range(0, 130, ErrorMessage = "Dakika 0–130 arasında olmalıdır.")]
        public int Minute { get; set; }

        [Range(0, 30, ErrorMessage = "Uzatma dakikası 0–30 arasında olmalıdır.")]
        public int AddedMinute { get; set; }

        [Required(ErrorMessage = "Kısa bir açıklama giriniz.")]
        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir.")]
        public string Description { get; set; } = string.Empty;
    }

    public class StandingDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Position { get; set; }
        public int Played { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference => GoalsFor - GoalsAgainst;
        public int Points => Wins * 3 + Draws;
        public List<string> Form { get; set; } = new();
    }
}
