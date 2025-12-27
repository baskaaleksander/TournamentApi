using System.ComponentModel.DataAnnotations;

namespace TournamentApi.DTOs;

public class RegisterInput
{
    [Required(ErrorMessage = "Email jest wymagany")]
    [EmailAddress(ErrorMessage = "Nieprawidłowy format email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hasło jest wymagane")]
    [MinLength(8, ErrorMessage = "Hasło musi mieć co najmniej 8 znaków")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Imię jest wymagane")]
    [StringLength(50, ErrorMessage = "Imię nie może przekraczać 50 znaków")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nazwisko jest wymagane")]
    [StringLength(50, ErrorMessage = "Nazwisko nie może przekraczać 50 znaków")]
    public string LastName { get; set; } = string.Empty;
}

