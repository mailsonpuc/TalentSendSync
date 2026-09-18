
using TalentSendSync.Application.DTOs;
using TalentSendSync.Domain.Entities;

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
			CandidaturaId = historicoContato.CandidaturaId
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
			historicoContatoDTO.CandidaturaId);
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
