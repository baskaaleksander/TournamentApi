using Microsoft.EntityFrameworkCore;
using TournamentApi.Data;
using TournamentApi.Models;

namespace TournamentApi.Services;

public class TournamentService
{
    private readonly AppDbContext _context;

    public TournamentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Tournament> CreateTournamentAsync(string name, DateTime startDate)
    {
        var tournament = new Tournament
        {
            Name = name,
            StartDate = startDate,
            Status = TournamentStatus.Planned
        };

        _context.Tournaments.Add(tournament);
        await _context.SaveChangesAsync();

        return tournament;
    }

    public async Task<Tournament> AddParticipantAsync(int tournamentId, int userId)
    {
        var tournament = await _context.Tournaments
            .Include(t => t.Participants)
            .FirstOrDefaultAsync(t => t.Id == tournamentId);

        if (tournament == null)
        {
            throw new InvalidOperationException($"Turniej o id {tournamentId} nie został znaleziony.");
        }

        if (tournament.Status != TournamentStatus.Planned)
        {
            throw new InvalidOperationException("Uczestników można dodawać tylko do turniejów w statusie Zaplanowany.");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"Użytkownik o id {userId} nie został znaleziony.");
        }

        if (tournament.Participants.Any(p => p.Id == userId))
        {
            throw new InvalidOperationException($"Użytkownik {userId} jest już uczestnikiem turnieju {tournamentId}.");
        }

        tournament.Participants.Add(user);
        await _context.SaveChangesAsync();

        return tournament;
    }

    public async Task<Tournament> StartTournamentAsync(int tournamentId)
    {
        var tournament = await _context.Tournaments
            .Include(t => t.Participants)
            .FirstOrDefaultAsync(t => t.Id == tournamentId);

        if (tournament == null)
        {
            throw new InvalidOperationException($"Turniej o id {tournamentId} nie został znaleziony.");
        }

        if (tournament.Status != TournamentStatus.Planned)
        {
            throw new InvalidOperationException("Tylko turnieje w statusie Zaplanowany mogą być rozpoczęte.");
        }

        if (tournament.Participants.Count < 2)
        {
            throw new InvalidOperationException("Turniej musi mieć co najmniej 2 uczestników, aby rozpocząć.");
        }

        tournament.Status = TournamentStatus.Ongoing;
        await _context.SaveChangesAsync();

        return tournament;
    }

    public async Task<Bracket> GenerateBracketAsync(int tournamentId)
    {
        var tournament = await _context.Tournaments
            .Include(t => t.Participants)
            .Include(t => t.Bracket)
            .FirstOrDefaultAsync(t => t.Id == tournamentId);

        if (tournament == null)
        {
            throw new InvalidOperationException($"Turniej o id {tournamentId} nie został znaleziony.");
        }

        if (tournament.Participants.Count < 2)
        {
            throw new InvalidOperationException("Turniej musi mieć co najmniej 2 uczestników, aby wygenerować drabinkę.");
        }

        var participantCount = tournament.Participants.Count;
        if ((participantCount & (participantCount - 1)) != 0)
        {
            throw new InvalidOperationException("Liczba uczestników musi być potęgą liczby 2 dla prawidłowej struktury drabinki.");
        }

        if (tournament.Bracket != null)
        {
            throw new InvalidOperationException("Drabinka już istnieje dla tego turnieju.");
        }

        var bracket = new Bracket
        {
            TournamentId = tournamentId,
            Tournament = tournament
        };

        _context.Brackets.Add(bracket);
        await _context.SaveChangesAsync();

        var participants = tournament.Participants.ToList();
        var shuffled = participants.OrderBy(x => Guid.NewGuid()).ToList();

        for (int i = 0; i < shuffled.Count; i += 2)
        {
            var match = new Match
            {
                BracketId = bracket.Id,
                Bracket = bracket,
                Round = 1,
                Player1Id = shuffled[i].Id,
                Player2Id = shuffled[i + 1].Id
            };

            bracket.Matches.Add(match);
            _context.Matches.Add(match);
        }

        await _context.SaveChangesAsync();

        return bracket;
    }

    public async Task<Match> PlayMatchAsync(int matchId, int winnerId)
    {
        var match = await _context.Matches
            .Include(m => m.Bracket)
                .ThenInclude(b => b!.Tournament)
            .FirstOrDefaultAsync(m => m.Id == matchId);

        if (match == null)
        {
            throw new InvalidOperationException($"Mecz o id {matchId} nie został znaleziony.");
        }

        if (match.WinnerId.HasValue)
        {
            throw new InvalidOperationException("Mecz ma już zwycięzcę.");
        }

        if (winnerId != match.Player1Id && winnerId != match.Player2Id)
        {
            throw new InvalidOperationException("Zwycięzca musi być jednym z graczy meczu.");
        }

        match.WinnerId = winnerId;
        await _context.SaveChangesAsync();

        var bracket = match.Bracket;
        if (bracket == null)
        {
            throw new InvalidOperationException("Mecz nie ma powiązanej drabinki.");
        }

        var currentRound = match.Round;
        var allMatchesInRound = await _context.Matches
            .Where(m => m.BracketId == bracket.Id && m.Round == currentRound)
            .ToListAsync();

        var allComplete = allMatchesInRound.All(m => m.WinnerId.HasValue);

        if (allComplete)
        {
            if (allMatchesInRound.Count == 1)
            {
                bracket.Tournament.Status = TournamentStatus.Completed;
                await _context.SaveChangesAsync();
            }
            else
            {
                await GenerateNextRoundAsync(bracket, currentRound + 1);
            }
        }

        return match;
    }

    private async Task GenerateNextRoundAsync(Bracket bracket, int nextRound)
    {
        var previousRound = nextRound - 1;
        var previousRoundMatches = await _context.Matches
            .Where(m => m.BracketId == bracket.Id && m.Round == previousRound)
            .ToListAsync();

        var winners = previousRoundMatches
            .Where(m => m.WinnerId.HasValue)
            .Select(m => m.WinnerId!.Value)
            .ToList();

        for (int i = 0; i < winners.Count; i += 2)
        {
            var match = new Match
            {
                BracketId = bracket.Id,
                Bracket = bracket,
                Round = nextRound,
                Player1Id = winners[i],
                Player2Id = winners[i + 1]
            };

            bracket.Matches.Add(match);
            _context.Matches.Add(match);
        }

        await _context.SaveChangesAsync();
    }
}

