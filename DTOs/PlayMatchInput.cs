using System.ComponentModel.DataAnnotations;

namespace TournamentApi.DTOs;

public class PlayMatchInput
{
    [Required(ErrorMessage = "ID meczu jest wymagane")]
    [Range(1, int.MaxValue, ErrorMessage = "ID meczu musi być liczbą całkowitą dodatnią")]
    public int MatchId { get; set; }

    [Required(ErrorMessage = "ID zwycięzcy jest wymagane")]
    [Range(1, int.MaxValue, ErrorMessage = "ID zwycięzcy musi być liczbą całkowitą dodatnią")]
    public int WinnerId { get; set; }
}

