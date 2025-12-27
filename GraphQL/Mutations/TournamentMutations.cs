using HotChocolate.Types;
using TournamentApi.GraphQL;
using TournamentApi.Models;
using TournamentApi.Services;

namespace TournamentApi.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class TournamentMutations
{
    public async Task<Tournament> AddParticipant(
        int tournamentId,
        int userId,
        TournamentService tournamentService)
    {
        return await tournamentService.AddParticipantAsync(tournamentId, userId);
    }

    public async Task<Tournament> StartTournament(
        int tournamentId,
        TournamentService tournamentService)
    {
        return await tournamentService.StartTournamentAsync(tournamentId);
    }

    public async Task<Bracket> GenerateBracket(
        int tournamentId,
        TournamentService tournamentService)
    {
        return await tournamentService.GenerateBracketAsync(tournamentId);
    }

    public async Task<Match> PlayMatch(
        int matchId,
        int winnerId,
        TournamentService tournamentService)
    {
        return await tournamentService.PlayMatchAsync(matchId, winnerId);
    }
}

