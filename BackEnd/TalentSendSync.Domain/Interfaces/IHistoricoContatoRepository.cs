using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Pagination;

namespace TalentSendSync.Domain.Interfaces;

public interface IHistoricoContatoRepository
{
    Task<PagedList<HistoricoContato>> GetHistoricosPagedAsync(int pageNumber, int pageSize);
    Task<HistoricoContato?> GetByIdAsync(Guid? id);
    Task<HistoricoContato> CreateAsync(HistoricoContato historicoContato);
    Task<HistoricoContato> UpdateAsync(HistoricoContato historicoContato);
    Task<HistoricoContato> RemoveAsync(HistoricoContato historicoContato);
}
