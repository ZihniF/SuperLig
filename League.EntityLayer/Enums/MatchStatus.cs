using System.ComponentModel.DataAnnotations;

namespace League.EntityLayer.Enums
{
    public enum MatchStatus
    {
        [Display(Name = "Oynanmadı")]
        NotPlayed = 0,
        [Display(Name = "Devam ediyor")]
        Live = 1,
        [Display(Name = "Bitti")]
        Finished = 2
    }

    public enum ActionType
    {
        [Display(Name = "Gol")]
        Goal = 1,
        [Display(Name = "Sarı kart")]
        YellowCard = 2,
        [Display(Name = "Kırmızı kart")]
        RedCard = 3,
        [Display(Name = "Oyuncu değişikliği")]
        Substitution = 4
    }
}
