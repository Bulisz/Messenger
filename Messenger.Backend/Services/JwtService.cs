using Messenger.Backend.MiddlewareConfig;
using Messenger.Backend.Models.AuthDTOs;
using Messenger.Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Messenger.Backend.Abstactions;

namespace Messenger.Backend.Services;

public class JwtService : IJwtService
{
    private readonly IMemoryCache _memoryCache;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOptionsMonitor<JwtTokensOptions> _jwtTokensOptionsMonitor;

    public JwtService(
        IMemoryCache memoryCache,
        UserManager<ApplicationUser> userManager,
        IOptionsMonitor<JwtTokensOptions> jwtTokensOptionsMonitor)
    {
        _memoryCache = memoryCache;
        _userManager = userManager;
        _jwtTokensOptionsMonitor = jwtTokensOptionsMonitor;
    }

    public async Task<TokensDTO> CreateTokensAsync(ApplicationUser user)
    {
        return new TokensDTO
        {
            AccessToken = await CreateAccessTokenAsync(user),
            RefreshToken = CreateRefreshToken(user)
        };
    }

    private async Task<JwtToken> CreateAccessTokenAsync(ApplicationUser user)
    {
        IList<string> roles = await _userManager.GetRolesAsync(user);
        JwtTokenOptions accessTokenOptions = _jwtTokensOptionsMonitor.CurrentValue.AccessTokenOptions;

        var claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
        }
        .Union(roles.Select(role => new Claim(ClaimTypes.Role, role)))
        .ToArray();

        return CreateToken(accessTokenOptions, claims);
    }

    private JwtToken CreateRefreshToken(ApplicationUser user)
    {
        JwtTokenOptions refreshTokenOptions = _jwtTokensOptionsMonitor.CurrentValue.RefreshTokenOptions;

        var claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id)
        };

        JwtToken refreshToken = CreateToken(refreshTokenOptions, claims);

        _memoryCache.Set(refreshToken.Value!, 0, refreshToken.Expiration);

        return refreshToken;
    }

    private static JwtToken CreateToken(JwtTokenOptions jwtTokenOptions, Claim[] claims)
    {
        var expiration = DateTime.UtcNow.AddMinutes(jwtTokenOptions.ExpirationMinutes);

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenOptions.Key)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: jwtTokenOptions.Issuer,
            audience: jwtTokenOptions.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: signingCredentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();

        return new JwtToken
        {
            Value = tokenHandler.WriteToken(token),
            Expiration = expiration
        };
    }

    public async Task<TokensDTO> RenewTokensAsync(string refreshToken)
    {
        // https://stackoverflow.com/questions/29355384/when-is-jwtsecuritytokenhandler-validatetoken-actually-valid

        if (!_memoryCache.TryGetValue(refreshToken, out var _))
        {
            throw new JwtException($"Refresh token is missing: {refreshToken}");
        }

        JwtTokenOptions refreshTokenOptions = _jwtTokensOptionsMonitor.CurrentValue.RefreshTokenOptions;

        ClaimsPrincipal claimsPrincipal;
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshTokenOptions.Key)),
            ValidAudience = refreshTokenOptions.Audience,
            ValidIssuer = refreshTokenOptions.Issuer,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true
        };

        try
        {
            claimsPrincipal = tokenHandler.ValidateToken(
                refreshToken, tokenValidationParameters, out SecurityToken validatedToken);
        }
        catch (SecurityTokenException exception)
        {
            throw new JwtException("JWT token validation failed.", exception);
        }

        ApplicationUser user = await _userManager.GetUserAsync(claimsPrincipal)
            ?? throw new InvalidOperationException(
                $"User not found with id {claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)}");

        return new TokensDTO
        {
            AccessToken = await CreateAccessTokenAsync(user),
            RefreshToken = CreateRefreshToken(user)
        };
    }

    public void ClearRefreshToken(string refreshToken)
    {
        _memoryCache.Remove(refreshToken);
    }


}
