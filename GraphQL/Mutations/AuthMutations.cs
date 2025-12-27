using HotChocolate.Types;
using TournamentApi.DTOs;
using TournamentApi.GraphQL;
using TournamentApi.Services;

namespace TournamentApi.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class AuthMutations
{
    public async Task<AuthPayload> Register(
        RegisterInput input,
        AuthService authService)
    {
        return await authService.RegisterAsync(input);
    }

    public async Task<AuthPayload> Login(
        LoginInput input,
        AuthService authService)
    {
        return await authService.LoginAsync(input);
    }
}

