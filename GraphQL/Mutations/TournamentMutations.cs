using System.ComponentModel.DataAnnotations;
using HotChocolate.Types;
using TournamentApi.DTOs;
using TournamentApi.GraphQL;
using TournamentApi.Models;
using TournamentApi.Services;

namespace TournamentApi.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class TournamentMutations
{
    public async Task<Tournament> CreateTournament(
        CreateTournamentInput input,
        TournamentService tournamentService)
    {
        return await tournamentService.CreateTournamentAsync(input.Name, input.StartDate);
    }

    public async Task<Tournament> AddParticipant(
        [Range(1, int.MaxValue, ErrorMessage = "ID turnieju musi być liczbą całkowitą dodatnią")] int tournamentId,
        [Range(1, int.MaxValue, ErrorMessage = "ID użytkownika musi być liczbą całkowitą dodatnią")] int userId,
        TournamentService tournamentService)
    {
        return await tournamentService.AddParticipantAsync(tournamentId, userId);
    }

    public async Task<Tournament> StartTournament(
        [Range(1, int.MaxValue, ErrorMessage = "ID turnieju musi być liczbą całkowitą dodatnią")] int tournamentId,
        TournamentService tournamentService)
    {
        return await tournamentService.StartTournamentAsync(tournamentId);
    }

    public async Task<Bracket> GenerateBracket(
        [Range(1, int.MaxValue, ErrorMessage = "ID turnieju musi być liczbą całkowitą dodatnią")] int tournamentId,
        TournamentService tournamentService)
    {
        return await tournamentService.GenerateBracketAsync(tournamentId);
    }

    public async Task<Match> PlayMatch(
        PlayMatchInput input,
        TournamentService tournamentService)
    {
        return await tournamentService.PlayMatchAsync(input.MatchId, input.WinnerId);
    }
}

