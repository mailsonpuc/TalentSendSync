using Microsoft.EntityFrameworkCore;
using TalentSendSync.Application.DTOs;
using TalentSendSync.Application.Interfaces;
using TalentSendSync.Domain.Enums;
using TalentSendSync.Domain.Interfaces;

namespace TalentSendSync.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly ICandidaturaRepository _candidaturaRepository;

    public DashboardService(ICandidaturaRepository candidaturaRepository)
    {
        _candidaturaRepository = candidaturaRepository;
    }

    public async Task<DashboardDTO> GetCandidaturasAsync()
    {
        var candidaturas = await _candidaturaRepository.GetCandidaturasAsync();

        var total = await candidaturas.CountAsync();
        var statusCounts = await candidaturas
            .GroupBy(candidatura => candidatura.Status)
            .Select(group => new { Status = group.Key, Quantidade = group.Count() })
            .ToDictionaryAsync(item => item.Status, item => item.Quantidade);

        var totalComContato = await candidaturas
            .CountAsync(candidatura => candidatura.HistoricosContato.Any());

        var evolucao = await candidaturas
            .GroupBy(candidatura => new { candidatura.DataEnvio.Year, candidatura.DataEnvio.Month })
            .Select(group => new
            {
                group.Key.Year,
                group.Key.Month,
                Quantidade = group.Count()
            })
            .ToListAsync();

        var ultimasCandidaturas = await candidaturas
            .OrderByDescending(candidatura => candidatura.DataEnvio)
            .Take(5)
            .Select(candidatura => new DashboardCandidaturaDTO
            {
                CandidaturaId = candidatura.CandidaturaId,
                Empresa = candidatura.Empresa,
                Cargo = candidatura.Cargo,
                DataEnvio = candidatura.DataEnvio,
                Status = candidatura.Status
            })
            .ToListAsync();

        return new DashboardDTO
        {
            TotalCandidaturas = total,
            Enviadas = CountStatus(statusCounts, StatusEnum.Enviado),
            EmAndamento = CountStatus(statusCounts, StatusEnum.EmAndamento),
            PropostasRecebidas = CountStatus(statusCounts, StatusEnum.PropostaRecebida),
            Aprovadas = CountStatus(statusCounts, StatusEnum.Aprovado),
            Rejeitadas = CountStatus(statusCounts, StatusEnum.Rejeitado),
            Canceladas = CountStatus(statusCounts, StatusEnum.Cancelado),
            SemRetorno = CountStatus(statusCounts, StatusEnum.SemRetorno),
            // Considera respondida a candidatura com ao menos um contato registrado.
            TaxaDeResposta = CalculateRate(total, totalComContato),
            // Aprovações sobre o total de candidaturas cadastradas.
            TaxaDeAprovacao = CalculateRate(total, CountStatus(statusCounts, StatusEnum.Aprovado)),
            Status = Enum.GetValues<StatusEnum>()
                .Select(status => new DashboardStatusDTO
                {
                    Status = status.GetDisplayName(),
                    Quantidade = CountStatus(statusCounts, status)
                })
                .ToList(),
            Evolucao = evolucao
                .OrderBy(item => item.Year)
                .ThenBy(item => item.Month)
                .Select(item => new DashboardEvolucaoDTO
                {
                    Mes = $"{item.Year:D4}-{item.Month:D2}",
                    Quantidade = item.Quantidade
                })
                .ToList(),
            UltimasCandidaturas = ultimasCandidaturas
        };
    }

    private static int CountStatus(IReadOnlyDictionary<StatusEnum, int> statusCounts, StatusEnum status)
    {
        return statusCounts.TryGetValue(status, out var count) ? count : 0;
    }

    private static decimal CalculateRate(int denominator, int numerator)
    {
        return denominator == 0 ? 0 : Math.Round((decimal)numerator / denominator * 100, 2);
    }
}