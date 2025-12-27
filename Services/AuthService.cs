using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TournamentApi.Data;
using TournamentApi.DTOs;
using TournamentApi.Models;

namespace TournamentApi.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthPayload> LoginAsync(LoginInput input)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == input.Email);

        if (user == null || !VerifyPassword(input.Password, user.PasswordHash))
        {
            throw new InvalidOperationException("Nieprawidłowy email lub hasło.");
        }

        var token = GenerateToken(user);

        return new AuthPayload
        {
            Token = token,
            User = user
        };
    }

    public async Task<AuthPayload> RegisterAsync(RegisterInput input)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == input.Email);

        if (existingUser != null)
        {
            throw new InvalidOperationException($"Użytkownik z emailem {input.Email} już istnieje.");
        }

        var passwordHash = HashPassword(input.Password);
        var user = new User
        {
            Email = input.Email,
            FirstName = input.FirstName,
            LastName = input.LastName,
            PasswordHash = passwordHash
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateToken(user);

        return new AuthPayload
        {
            Token = token,
            User = user
        };
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    private string GenerateToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("Klucz tajny JWT nie jest skonfigurowany");
        var issuer = jwtSettings["Issuer"] ?? "TournamentApi";
        var audience = jwtSettings["Audience"] ?? "TournamentApi";
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

