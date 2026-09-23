

using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Pagination;

namespace TalentSendSync.Domain.Interfaces;

public interface ICurriculoRepository
{
    Task<PagedList<Curriculo>> GetCurriculosPagedAsync(int pageNumber, int pageSize, string userId);
    Task<Curriculo?> GetByIdAsync(Guid? id, string userId);
    Task<Curriculo> CreateAsync(Curriculo curriculo);
    Task<Curriculo> UpdateAsync(Curriculo curriculo);
    Task<Curriculo> RemoveAsync(Curriculo curriculo);
}
