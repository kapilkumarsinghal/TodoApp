using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Api.Data;
using TodoApp.Api.DataTransfer.Requests;
using TodoApp.Api.DataTransfer.Responses;
using TodoApp.Api.Models;

namespace TodoApp.Api.Services;

public class AccountService : IAccountService
{
    private const int PasswordIterations = 210_000;
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AccountService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AccountResponse?> RegisterAsync(AccountRequest request)
    {
        var email = request.Email.Trim();
        var normalizedEmail = email.ToUpperInvariant();
        if (await _context.Users.AnyAsync(user => user.NormalizedEmail == normalizedEmail)) return null;

        var user = new AppUser { Email = email, NormalizedEmail = normalizedEmail, PasswordHash = HashPassword(request.Password) };
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Habits created before accounts existed belong to the first account that claims this local tracker.
            await _context.Habits.Where(habit => habit.OwnerId == null)
                .ExecuteUpdateAsync(update => update.SetProperty(habit => habit.OwnerId, user.Id));
            await _context.Todos.Where(todo => todo.OwnerId == null)
                .ExecuteUpdateAsync(update => update.SetProperty(todo => todo.OwnerId, user.Id));
            await transaction.CommitAsync();
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            return null;
        }

        return CreateResponse(user);
    }

    public async Task<AccountResponse?> LoginAsync(AccountRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();
        var user = await _context.Users.SingleOrDefaultAsync(item => item.NormalizedEmail == normalizedEmail);
        return user is not null && VerifyPassword(request.Password, user.PasswordHash) ? CreateResponse(user) : null;
    }

    private AccountResponse CreateResponse(AppUser user)
    {
        var jwt = _configuration.GetSection("Jwt");
        var keyText = jwt["Key"] ?? throw new InvalidOperationException("JWT signing key is not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
        };
        var token = new JwtSecurityToken(jwt["Issuer"], jwt["Audience"], claims,
            expires: DateTime.UtcNow.AddHours(12), signingCredentials: credentials);
        return new AccountResponse { AccessToken = new JwtSecurityTokenHandler().WriteToken(token), Email = user.Email };
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, PasswordIterations, HashAlgorithmName.SHA256, 32);
        return $"pbkdf2-sha256${PasswordIterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string encoded)
    {
        var parts = encoded.Split('$');
        if (parts.Length != 4 || parts[0] != "pbkdf2-sha256" || !int.TryParse(parts[1], out var iterations)) return false;
        try
        {
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException) { return false; }
    }
}
