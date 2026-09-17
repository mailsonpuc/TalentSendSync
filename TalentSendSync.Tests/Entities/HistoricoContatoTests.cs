using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Enums;
using Xunit;

namespace TalentSendSync.Tests.Entities;

public class HistoricoContatoTests
{
    [Fact]
    public void Deve_Criar_Historico_Com_Dados_Validos()
    {
        var candidaturaId = Guid.NewGuid();
        var dataContato = DateTime.UtcNow.AddDays(-1);

        var historico = new HistoricoContato(
            dataContato,
            TipoContatoEnum.Email,
            "  Envio de follow-up  ",
            candidaturaId);

        Assert.Equal(dataContato, historico.DataContato);
        Assert.Equal(TipoContatoEnum.Email, historico.TipoContato);
        Assert.Equal("Envio de follow-up", historico.Descricao);
        Assert.Equal(candidaturaId, historico.CandidaturaId);
    }

    [Fact]
    public void Nao_Deve_Criar_Historico_Com_Data_Padrao()
    {
        Assert.Throws<ArgumentException>(() =>
            new HistoricoContato(default, TipoContatoEnum.Email, "Contato", Guid.NewGuid()));
    }

    [Fact]
    public void Nao_Deve_Criar_Historico_Com_Data_Futura()
    {
        Assert.Throws<ArgumentException>(() =>
            new HistoricoContato(DateTime.UtcNow.AddMinutes(1), TipoContatoEnum.Email, "Contato", Guid.NewGuid()));
    }

    [Fact]
    public void Nao_Deve_Criar_Historico_Com_Tipo_Invalido()
    {
        Assert.Throws<ArgumentException>(() =>
            new HistoricoContato(DateTime.UtcNow, (TipoContatoEnum)999, "Contato", Guid.NewGuid()));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Nao_Deve_Criar_Historico_Com_Descricao_Vazia(string descricao)
    {
        Assert.Throws<ArgumentException>(() =>
            new HistoricoContato(DateTime.UtcNow, TipoContatoEnum.Email, descricao, Guid.NewGuid()));
    }

    [Fact]
    public void Nao_Deve_Aceitar_Descricao_Com_Mais_De_1000_Caracteres()
    {
        Assert.Throws<ArgumentException>(() =>
            new HistoricoContato(DateTime.UtcNow, TipoContatoEnum.Email, new string('X', 1001), Guid.NewGuid()));
    }

    [Fact]
    public void Nao_Deve_Criar_Historico_Sem_Candidatura()
    {
        Assert.Throws<ArgumentException>(() =>
            new HistoricoContato(DateTime.UtcNow, TipoContatoEnum.Email, "Contato", Guid.Empty));
    }
}
