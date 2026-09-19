
using TalentSendSync.Application.DTOs;
using TalentSendSync.Domain.Entities;

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
            DataEnvio = candidatura.DataEnvio
        };
    }

    public static Candidatura? ToCandidatura(this CandidaturaDTO candidaturaDTO)
    {
        if (candidaturaDTO is null)
            return null;

        return new Candidatura(
            candidaturaDTO.Empresa,
            candidaturaDTO.Cargo,
            candidaturaDTO.PretensaoSalarial,
            candidaturaDTO.LinkVaga,
            candidaturaDTO.Status,
            candidaturaDTO.CurriculoId);
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
