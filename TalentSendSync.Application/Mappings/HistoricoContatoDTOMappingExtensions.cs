
using TalentSendSync.Application.DTOs;
using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Enums;

namespace TalentSendSync.Application.Mappings;

public static class HistoricoContatoDTOMappingExtensions
{
	public static HistoricoContatoDTO? ToHistoricoContatoDTO(this HistoricoContato historicoContato)
	{
		if (historicoContato is null)
			return null;

		return new HistoricoContatoDTO
		{
			HistoricoContatoId = historicoContato.HistoricoContatoId,
			DataContato = historicoContato.DataContato,
			TipoContato = historicoContato.TipoContato,
			Descricao = historicoContato.Descricao,
			CandidaturaId = historicoContato.CandidaturaId,
			Responsavel = historicoContato.Responsavel,
			CandidaturaStatus = historicoContato.Candidatura?.Status.ToString(),
			CandidaturaStatusNome = historicoContato.Candidatura?.Status.GetDisplayName(),
			Candidatura = historicoContato.Candidatura is null
				? null
				: new CandidaturaHistoricoDTO
				{
					CandidaturaId = historicoContato.Candidatura.CandidaturaId,
					Empresa = historicoContato.Candidatura.Empresa,
					Cargo = historicoContato.Candidatura.Cargo,
					DataEnvio = historicoContato.Candidatura.DataEnvio,
					PretensaoSalarial = historicoContato.Candidatura.PretensaoSalarial,
					LinkVaga = historicoContato.Candidatura.LinkVaga,
					Status = historicoContato.Candidatura.Status,
					CurriculoId = historicoContato.Candidatura.CurriculoId,
					Curriculo = historicoContato.Candidatura.Curriculo?.ToCurriculoDTO(),
					HistoricosContato = historicoContato.Candidatura.HistoricosContato
						.Select(contato => new HistoricoContatoResumoDTO
						{
							HistoricoContatoId = contato.HistoricoContatoId,
							DataContato = contato.DataContato,
							TipoContato = contato.TipoContato,
							Descricao = contato.Descricao,
							CandidaturaId = contato.CandidaturaId,
							Responsavel = contato.Responsavel
						})
						.ToList()
				}
		};
	}

	public static HistoricoContato? ToHistoricoContato(this HistoricoContatoDTO historicoContatoDTO)
	{
		if (historicoContatoDTO is null)
			return null;

		return new HistoricoContato(
			historicoContatoDTO.DataContato,
			historicoContatoDTO.TipoContato,
			historicoContatoDTO.Descricao,
			historicoContatoDTO.CandidaturaId,
			historicoContatoDTO.Responsavel);
	}

	public static HistoricoContato? ToHistoricoContato(this HistoricoContatoCreateDTO historicoContatoDTO)
	{
		if (historicoContatoDTO is null)
			return null;

		return new HistoricoContato(
			historicoContatoDTO.DataContato,
			historicoContatoDTO.TipoContato,
			historicoContatoDTO.Descricao,
			historicoContatoDTO.CandidaturaId,
			historicoContatoDTO.Responsavel);
	}

	public static IEnumerable<HistoricoContatoDTO> ToHistoricoContatoDTOList(
		this IEnumerable<HistoricoContato> historicosContato)
	{
		if (historicosContato is null || !historicosContato.Any())
			return new List<HistoricoContatoDTO>();

		return historicosContato
			.Select(historicoContato => historicoContato.ToHistoricoContatoDTO()!)
			.ToList();
	}

}
