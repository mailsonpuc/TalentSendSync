

using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Pagination;

namespace TalentSendSync.Domain.Interfaces;

public interface ICurriculoRepository
{
    Task<PagedList<Curriculo>> GetCurriculosPagedAsync(int pageNumber, int pageSize);
    Task<Curriculo?> GetByIdAsync(Guid? id);
    Task<Curriculo> CreateAsync(Curriculo curriculo);
    Task<Curriculo> UpdateAsync(Curriculo curriculo);
    Task<Curriculo> RemoveAsync(Curriculo curriculo);
}
