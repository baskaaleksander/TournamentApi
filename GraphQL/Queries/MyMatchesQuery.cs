using System.Linq;
using System.Security.Claims;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using TournamentApi.Data;
using TournamentApi.GraphQL;
using TournamentApi.Models;

namespace TournamentApi.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class MyMatchesQuery
{
    [Authorize]
    public IQueryable<Match> MyMatches(
        ClaimsPrincipal claimsPrincipal,
        [Service] AppDbContext context)
    {
        var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Enumerable.Empty<Match>().AsQueryable();
        }

        return context.Matches
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Winner)
            .Include(m => m.Bracket)
            .Where(m => m.Player1Id == userId || m.Player2Id == userId);
    }
}

