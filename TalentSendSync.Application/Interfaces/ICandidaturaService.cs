

using TalentSendSync.Application.DTOs;
using TalentSendSync.Domain.Pagination;

namespace TalentSendSync.Application.Interfaces;

public interface ICandidaturaService
{
    Task<IQueryable<CandidaturaDTO>> GetCandidaturasAsync();
    Task<PagedList<CandidaturaDTO>> GetCandidaturasPagedAsync(int pageNumber, int pageSize);
    Task<CandidaturaDTO?> GetByIdAsync(Guid? id);
    Task<CandidaturaDTO> CreateAsync(CandidaturaDTO candidatura);
    Task<CandidaturaDTO> UpdateAsync(CandidaturaDTO candidatura);
    Task<CandidaturaDTO> RemoveAsync(CandidaturaDTO candidatura);

}
