using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TalentSendSync.Infrastructure.Identity;
using TalentSendSync.Infrastructure.Identity.Interfaces;
using TalentSendSync.Infrastructure.Identity.Models;

namespace TalentSendSync.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ITokenService tokenService,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration,
    ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }




    //[EnableRateLimiting("loginRateLimit")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userName = request.UserName;
        var email = request.Email;
        var password = request.Password;

        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return BadRequest(new ResponseDTO { Status = "Error", Message = "Username, email and password are required." });
        }

        var emailExists = await _userManager.FindByEmailAsync(email);
        if (emailExists is not null)
        {
            return Conflict(new ResponseDTO { Status = "Error", Message = "Email already exists." });
        }

        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(error => error.Description));
            return BadRequest(new ResponseDTO { Status = "Error", Message = errors });
        }

        return Ok(new ResponseDTO { Status = "Success", Message = "User created successfully." });
    }





    //[EnableRateLimiting("loginRateLimit")]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var email = request.Email;
        var password = request.Password;
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return BadRequest(new ResponseDTO { Status = "Error", Message = "Email and password are required." });
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return Unauthorized(new ResponseDTO { Status = "Error", Message = "Invalid email or password." });
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName ?? email),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            authClaims.Add(new Claim(ClaimTypes.Email, user.Email));
        }
        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var accessToken = _tokenService.GenerateAccessToken(authClaims, _configuration);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshValidity = _configuration.GetValue("Jwt:RefreshTokenValidityInMinutes", 60);
        if (refreshValidity <= 0)
        {
            refreshValidity = 60;
        }

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshValidity);
        await _userManager.UpdateAsync(user);

        return Ok(new TokenDTO
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
            RefreshToken = refreshToken,
            Expiration = accessToken.ValidTo
        });
    }





    [Authorize(Roles = "admin")]
    [HttpPost("CreateRole")]
    public async Task<IActionResult> CreateRole([FromQuery] string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return BadRequest(new ResponseDTO { Status = "Error", Message = "Role name is required." });
        }

        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return BadRequest(new ResponseDTO { Status = "Error", Message = "Role already exists." });
        }

        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
        if (!result.Succeeded)
        {
            _logger.LogWarning("Unable to create role {RoleName}.", roleName);
            return BadRequest(new ResponseDTO { Status = "Error", Message = "Unable to create role." });
        }

        _logger.LogInformation("Role {RoleName} created.", roleName);
        return Ok(new ResponseDTO { Status = "Success", Message = $"Role {roleName} added successfully." });
    }






    /// <summary>
    /// Adiciona um usuário existente a uma regra do sistema, requer admin.
    /// </summary>
    /// <param name="email">E-mail do usuário que receberá a função.</param>
    /// <param name="roleName">Nome da função que será atribuída ao usuário.</param>
    /// <returns>Confirmação da atribuição da função.</returns>
    /// <response code="200">Usuário adicionado à função com sucesso.</response>
    /// <response code="400">Usuário ou função não encontrados, ou erro ao atribuir a função.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">O usuário autenticado não possui a função admin.</response>
    [Authorize(Roles = "admin")]
    [HttpPost("AddUserToRole")]
    public async Task<IActionResult> AddUserToRole([FromQuery] string email, [FromQuery] string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return BadRequest(new ResponseDTO { Status = "Error", Message = "Unable to find user." });
        }

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return BadRequest(new ResponseDTO { Status = "Error", Message = "Role does not exist." });
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Unable to add user {Email} to role {RoleName}.", email, roleName);
            return BadRequest(new ResponseDTO { Status = "Error", Message = "Unable to add user to role." });
        }

        return Ok(new ResponseDTO { Status = "Success", Message = $"User {email} added to role {roleName}." });
    }





    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenDTO? tokenModel)
    {
        if (tokenModel is null || string.IsNullOrWhiteSpace(tokenModel.AccessToken) || string.IsNullOrWhiteSpace(tokenModel.RefreshToken))
        {
            return BadRequest("Invalid client request.");
        }

        var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AccessToken, _configuration);
        if (principal is null)
        {
            return BadRequest("Invalid access token/refresh token.");
        }

        var username = principal.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest("Invalid access token/refresh token.");
        }

        var user = await _userManager.FindByNameAsync(username);
        if (user is null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            if (user is not null)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }

            return BadRequest("Invalid access token/refresh token.");
        }

        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims, _configuration);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var refreshValidity = _configuration.GetValue("Jwt:RefreshTokenValidityInMinutes", 60);
        if (refreshValidity <= 0)
        {
            refreshValidity = 60;
        }

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshValidity);
        await _userManager.UpdateAsync(user);

        return Ok(new TokenDTO
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            RefreshToken = newRefreshToken,
            Expiration = newAccessToken.ValidTo
        });
    }





    [Authorize(Roles = "admin")]
    [HttpPost("revoke/{username}")]
    public async Task<IActionResult> Revoke([FromRoute] string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user is null)
        {
            return BadRequest("Invalid user name.");
        }

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);
        return NoContent();
    }

}
