using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Pagination;

namespace TalentSendSync.Domain.Interfaces;

public interface IHistoricoContatoRepository
{
    Task<PagedList<HistoricoContato>> GetHistoricosPagedAsync(int pageNumber, int pageSize, string userId);
    Task<HistoricoContato?> GetByIdAsync(Guid? id, string userId);
    Task<HistoricoContato> CreateAsync(HistoricoContato historicoContato);
    Task<HistoricoContato> UpdateAsync(HistoricoContato historicoContato);
    Task<HistoricoContato> RemoveAsync(HistoricoContato historicoContato);
}
