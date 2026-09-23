using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Pagination;

namespace TalentSendSync.Domain.Interfaces;

public interface ICandidaturaRepository
{
    Task<IQueryable<Candidatura>> GetCandidaturasAsync(string userId); 
    Task<PagedList<Candidatura>> GetCandidaturasPagedAsync(int pageNumber, int pageSize, string userId);
    Task<Candidatura?> GetByIdAsync(Guid? id, string userId);
    Task<Candidatura> CreateAsync(Candidatura candidatura);
    Task<Candidatura> UpdateAsync(Candidatura candidatura);
    Task<Candidatura> RemoveAsync(Candidatura candidatura);

}
