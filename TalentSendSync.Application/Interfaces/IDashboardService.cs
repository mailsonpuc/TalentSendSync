using TalentSendSync.Application.DTOs;

namespace TalentSendSync.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDTO> GetCandidaturasAsync();
}