using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TalentSendSync.Application.Interfaces;

namespace TalentSendSync.CrossCutting;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string UserId => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("Usuário autenticado não encontrado.");
}