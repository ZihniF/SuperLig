using League.EntityLayer.Enums;

namespace League.WebUI.Helpers
{
    public static class DisplayText
    {
        public static string Time(DateTime date, bool confirmed) =>
            confirmed ? $"{date:HH:mm} TSİ" : "Saat açıklanmadı";

        public static string Minute(int minute, int addedMinute) =>
            addedMinute > 0 ? $"{minute}+{addedMinute}" : minute.ToString();

        public static string Percentage(int? value) => value.HasValue ? $"%{value}" : "—";

        public static string Status(MatchStatus status) => status switch
        {
            MatchStatus.Live => "Devam ediyor",
            MatchStatus.Finished => "Bitti",
            _ => "Oynanmadı"
        };

        public static string Action(ActionType action) => action switch
        {
            ActionType.Goal => "Gol",
            ActionType.YellowCard => "Sarı kart",
            ActionType.RedCard => "Kırmızı kart",
            _ => "Oyuncu değişikliği"
        };
    }
}
