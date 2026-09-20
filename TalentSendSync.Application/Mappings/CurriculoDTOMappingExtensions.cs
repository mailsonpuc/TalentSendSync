using TalentSendSync.Application.DTOs;
using TalentSendSync.Domain.Entities;

namespace TalentSendSync.Application.Mappings;

public static class CurriculoDTOMappingExtensions
{
	public static CurriculoDTO? ToCurriculoDTO(this Curriculo curriculo)
	{
		if (curriculo is null)
			return null;

		return new CurriculoDTO
		{
			CurriculoId = curriculo.CurriculoId,
			Nome = curriculo.Nome,
			NomeArquivo = curriculo.NomeArquivo,
			StorageKey = curriculo.StorageKey,
			ContentType = curriculo.ContentType,
			TamanhoBytes = curriculo.TamanhoBytes,
			Versao = curriculo.Versao,
			DataCriacao = curriculo.DataCriacao,
			Ativo = curriculo.Ativo
		};
	}

	public static IEnumerable<CurriculoDTO> ToCurriculoDTOList(this IEnumerable<Curriculo> curriculos)
	{
		if (curriculos is null || !curriculos.Any())
			return new List<CurriculoDTO>();

		return curriculos
			.Select(curriculo => curriculo.ToCurriculoDTO()!)
			.ToList();
	}

}
