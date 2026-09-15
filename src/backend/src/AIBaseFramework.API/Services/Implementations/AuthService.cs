// Auth Service Implementation
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AIBaseFramework.API.Domain.Entities;
using AIBaseFramework.API.Infrastructure.Data;
using AIBaseFramework.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AIBaseFramework.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext dbContext,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if email exists
        if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
        {
            throw new Exception("Email đã được sử dụng");
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create user
        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            FullName = request.FullName,
            Department = request.Department,
            Role = UserRole.Student
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("User registered: {Email}", request.Email);

        // Generate tokens
        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

        if (user == null)
        {
            throw new Exception("Email hoặc password không đúng");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new Exception("Email hoặc password không đúng");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("User logged in: {Email}", request.Email);

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var tokenHash = ComputeHash(refreshToken);
        var storedToken = await _dbContext.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash
                && t.ExpiresAt > DateTime.UtcNow
                && t.RevokedAt == null);

        if (storedToken == null)
        {
            throw new Exception("Token không hợp lệ hoặc đã hết hạn");
        }

        // Revoke old token
        storedToken.RevokedAt = DateTime.UtcNow;

        // Generate new tokens
        var response = await GenerateAuthResponseAsync(storedToken.User!);

        // Save revoked token
        await _dbContext.SaveChangesAsync();

        return response;
    }

    public async Task LogoutAsync(Guid userId)
    {
        var tokens = await _dbContext.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("User logged out: {UserId}", userId);
    }

    public async Task<UserDto?> GetCurrentUserAsync(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        
        if (user == null)
            return null;

        return new UserDto(user.Id, user.Email, user.FullName, user.Department, user.Role.ToString());
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenHash = ComputeHash(refreshToken);

        // Save refresh token
        var storedToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.RefreshTokens.Add(storedToken);
        await _dbContext.SaveChangesAsync();

        return new AuthResponse(
            accessToken,
            refreshToken,
            new UserDto(user.Id, user.Email, user.FullName, user.Department, user.Role.ToString()));
    }

    private string GenerateAccessToken(User user)
    {
        var secret = _configuration["JwtConfig:Secret"] ?? throw new Exception("JWT Secret not configured");
        var issuer = _configuration["JwtConfig:Issuer"] ?? "AIBaseFrameworkAPI";
        var audience = _configuration["JwtConfig:Audience"] ?? "AIBaseFrameworkClient";
        var expiryMinutes = int.Parse(_configuration["JwtConfig:ExpiryMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("userId", user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private static string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
