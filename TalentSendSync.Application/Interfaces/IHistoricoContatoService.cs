using TalentSendSync.Application.DTOs;
using TalentSendSync.Domain.Pagination;

namespace TalentSendSync.Application.Interfaces;

public interface IHistoricoContatoService
{
    Task<PagedList<HistoricoContatoDTO>> GetHistoricosPagedAsync(int pageNumber, int pageSize);
    Task<HistoricoContatoDTO?> GetByIdAsync(Guid? id);
    Task<HistoricoContatoDTO> CreateAsync(HistoricoContatoDTO historicoContato);
    Task<HistoricoContatoDTO> UpdateAsync(HistoricoContatoDTO historicoContato);
    Task<HistoricoContatoDTO> RemoveAsync(HistoricoContatoDTO historicoContato);
}
