using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Pagination;

namespace TalentSendSync.Domain.Interfaces;

public interface ICandidaturaRepository
{
    Task<IQueryable<Candidatura>> GetCandidaturasAsync(); 
    Task<PagedList<Candidatura>> GetCandidaturasPagedAsync(int pageNumber, int pageSize);
    Task<Candidatura?> GetByIdAsync(Guid? id);
    Task<Candidatura> CreateAsync(Candidatura candidatura);
    Task<Candidatura> UpdateAsync(Candidatura candidatura);
    Task<Candidatura> RemoveAsync(Candidatura candidatura);

}
