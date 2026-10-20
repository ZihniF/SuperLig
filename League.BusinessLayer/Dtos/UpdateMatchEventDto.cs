using System.ComponentModel.DataAnnotations;

namespace League.BusinessLayer.Dtos
{
    public class UpdateMatchEventDto : CreateMatchEventDto
    {
        [Range(1, int.MaxValue)]
        public int MatchEventId { get; set; }
    }
}
