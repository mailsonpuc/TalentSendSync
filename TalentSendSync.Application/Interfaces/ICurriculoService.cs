
using TalentSendSync.Application.DTOs;
using TalentSendSync.Domain.Pagination;

namespace TalentSendSync.Application.Interfaces;

public interface ICurriculoService
{
    Task<PagedList<CurriculoDTO>> GetCurriculosPagedAsync(int pageNumber, int pageSize);
    Task<CurriculoDTO?> GetByIdAsync(Guid? id);
    Task<CurriculoDTO> CreateAsync(CriarCurriculoInput curriculo);
    Task<CurriculoDTO> UpdateAsync(CurriculoDTO curriculo);
    Task<CurriculoDTO> RemoveAsync(CurriculoDTO curriculo);

}
