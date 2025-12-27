using System.ComponentModel.DataAnnotations;
using TournamentApi.Attributes;

namespace TournamentApi.DTOs;

public class CreateTournamentInput
{
    [Required(ErrorMessage = "Nazwa turnieju jest wymagana")]
    [StringLength(100, ErrorMessage = "Nazwa turnieju nie może przekraczać 100 znaków")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Data rozpoczęcia jest wymagana")]
    [FutureDate(ErrorMessage = "Data rozpoczęcia musi być w przyszłości")]
    public DateTime StartDate { get; set; }
}

