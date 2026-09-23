using TalentSendSync.Application.DTOs;
using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Enums;

namespace TalentSendSync.Application.Mappings;

public static class CadidaturaDTOMappingExtensions
{
    public static CandidaturaDTO? ToCandidaturaDTO(this Candidatura candidatura)
    {
        if (candidatura is null)
            return null;

        return new CandidaturaDTO
        {
            CandidaturaId = candidatura.CandidaturaId,
            Empresa = candidatura.Empresa,
            Cargo = candidatura.Cargo,
            PretensaoSalarial = candidatura.PretensaoSalarial,
            LinkVaga = candidatura.LinkVaga,
            Status = candidatura.Status,
            CurriculoId = candidatura.CurriculoId,
            DataEnvio = candidatura.DataEnvio,
            Observacoes = candidatura.Observacoes,
            CurriculoNome = candidatura.Curriculo?.Nome,
            HistoricosContato = candidatura.HistoricosContato
                .OrderBy(historico => historico.DataContato)
                .Select(historico => new HistoricoContatoResumoDTO
                {
                    HistoricoContatoId = historico.HistoricoContatoId,
                    DataContato = historico.DataContato,
                    TipoContato = historico.TipoContato,
                    Descricao = historico.Descricao,
                    CandidaturaId = historico.CandidaturaId,
                    Responsavel = historico.Responsavel
                })
                .ToList()
        };
    }

    public static Candidatura? ToCandidatura(this CandidaturaCreateDTO candidaturaDTO)
    {
        if (candidaturaDTO is null)
            return null;

        return new Candidatura(
            candidaturaDTO.Empresa,
            candidaturaDTO.Cargo,
            candidaturaDTO.PretensaoSalarial,
            candidaturaDTO.LinkVaga,
            candidaturaDTO.Status,
            candidaturaDTO.CurriculoId,
            candidaturaDTO.Observacoes);
    }

    public static IEnumerable<CandidaturaDTO> ToCandidaturaDTOList(this IEnumerable<Candidatura> candidaturas)
    {
        if (candidaturas is null || !candidaturas.Any())
            return new List<CandidaturaDTO>();

        return candidaturas
            .Select(candidatura => candidatura.ToCandidaturaDTO()!)
            .ToList();
    }
}
