using TournamentApi.Models;

namespace TournamentApi.DTOs;

public class AuthPayload
{
    public string Token { get; set; } = string.Empty;
    public User? User { get; set; }
}

