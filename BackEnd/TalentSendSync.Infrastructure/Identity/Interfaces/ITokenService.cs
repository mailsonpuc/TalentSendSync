using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using TalentSendSync.Infrastructure.Identity;
using TalentSendSync.Infrastructure.Identity.Models;

namespace TalentSendSync.Infrastructure.Identity.Interfaces;

public interface ITokenService
{
    Task<TokenDTO> CreateTokenAsync(ApplicationUser user);
    Task<TokenDTO?> RefreshTokenAsync(string refreshToken);
    JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration configuration);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token, IConfiguration configuration);
}