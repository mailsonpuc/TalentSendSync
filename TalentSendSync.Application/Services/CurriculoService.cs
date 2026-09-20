using TalentSendSync.Application.DTOs;
using TalentSendSync.Application.Interfaces;
using TalentSendSync.Application.Mappings;
using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Interfaces;
using TalentSendSync.Domain.Pagination;

namespace TalentSendSync.Application.Services;

public class CurriculoService : ICurriculoService
{
    private readonly ICurriculoRepository _curriculoRepository;
    private readonly IArquivoStorage _arquivoStorage;

    public CurriculoService(
        ICurriculoRepository curriculoRepository,
        IArquivoStorage arquivoStorage)
    {
        _curriculoRepository = curriculoRepository;
        _arquivoStorage = arquivoStorage;
    }

    public async Task<PagedList<CurriculoDTO>> GetCurriculosPagedAsync(int pageNumber, int pageSize)
    {
        var curriculos = await _curriculoRepository.GetCurriculosPagedAsync(pageNumber, pageSize);
        var curriculoDtos = curriculos
            .Select(curriculo => curriculo.ToCurriculoDTO()!)
            .ToList();

        return new PagedList<CurriculoDTO>(
            curriculoDtos,
            curriculos.TotalCount,
            curriculos.CurrentPage,
            curriculos.PageSize);
    }

    public async Task<CurriculoDTO?> GetByIdAsync(Guid? id)
    {
        var curriculo = await _curriculoRepository.GetByIdAsync(id);
        return curriculo?.ToCurriculoDTO();
    }

    public async Task<CurriculoDTO> CreateAsync(CriarCurriculoInput curriculo)
    {
        ArgumentNullException.ThrowIfNull(curriculo);

        await ValidatePdfAsync(curriculo);

        var storageKey = $"{Guid.NewGuid():N}.pdf";
        await _arquivoStorage.SalvarAsync(curriculo.Arquivo, storageKey);

        var curriculoEntity = new Curriculo(
            curriculo.Nome,
            curriculo.NomeArquivo,
            storageKey,
            curriculo.ContentType,
            curriculo.TamanhoBytes,
            curriculo.Versao);
        var createdCurriculo = await _curriculoRepository.CreateAsync(curriculoEntity);

        return createdCurriculo.ToCurriculoDTO()!;
    }

    public async Task<CurriculoDTO> UpdateAsync(CurriculoDTO curriculo)
    {
        ArgumentNullException.ThrowIfNull(curriculo);

        var curriculoEntity = await GetEntityByIdAsync(curriculo.CurriculoId);
        curriculoEntity.UpdateDetails(curriculo.Nome, curriculo.Versao);

        var updatedCurriculo = await _curriculoRepository.UpdateAsync(curriculoEntity);
        return updatedCurriculo.ToCurriculoDTO()!;
    }

    public async Task<CurriculoDTO> RemoveAsync(CurriculoDTO curriculo)
    {
        ArgumentNullException.ThrowIfNull(curriculo);

        var curriculoEntity = await GetEntityByIdAsync(curriculo.CurriculoId);
        var removedCurriculo = await _curriculoRepository.RemoveAsync(curriculoEntity);
        await _arquivoStorage.ExcluirAsync(removedCurriculo.StorageKey);

        return removedCurriculo.ToCurriculoDTO()!;
    }

    private static async Task ValidatePdfAsync(CriarCurriculoInput curriculo)
    {
        if (curriculo.TamanhoBytes <= 0)
            throw new ArgumentException("O arquivo deve possuir conteúdo.", nameof(curriculo));

        if (curriculo.TamanhoBytes > 10 * 1024 * 1024)
            throw new ArgumentException("O arquivo não pode ultrapassar 10 MB.", nameof(curriculo));

        if (!string.Equals(curriculo.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase) ||
            !Path.GetExtension(curriculo.NomeArquivo).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Envie um arquivo PDF válido.", nameof(curriculo));
        }

        if (!curriculo.Arquivo.CanSeek)
            throw new ArgumentException("O arquivo não pode ser validado.", nameof(curriculo));

        var assinatura = new byte[5];
        curriculo.Arquivo.Position = 0;
        var bytesLidos = await curriculo.Arquivo.ReadAsync(assinatura);
        curriculo.Arquivo.Position = 0;

        if (bytesLidos < assinatura.Length ||
            assinatura[0] != '%' ||
            assinatura[1] != 'P' ||
            assinatura[2] != 'D' ||
            assinatura[3] != 'F' ||
            assinatura[4] != '-')
        {
            throw new ArgumentException("O conteúdo enviado não é um PDF válido.", nameof(curriculo));
        }
    }

    private async Task<Curriculo> GetEntityByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("O ID do currículo é obrigatório.", nameof(id));

        var curriculo = await _curriculoRepository.GetByIdAsync(id);
        return curriculo ?? throw new KeyNotFoundException("Currículo não encontrado.");
    }
}