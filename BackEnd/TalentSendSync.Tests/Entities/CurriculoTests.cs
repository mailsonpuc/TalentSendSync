using TalentSendSync.Domain.Entities;
using Xunit;

namespace TalentSendSync.Tests.Entities;

public class CurriculoTests
{
    [Fact]
    public void Deve_Criar_Curriculo_Com_Dados_Validos()
    {
        var curriculo = CriarCurriculo();

        Assert.NotEqual(Guid.Empty, curriculo.CurriculoId);
        Assert.Equal("Currículo Backend .NET", curriculo.Nome);
        Assert.Equal("curriculo.pdf", curriculo.NomeArquivo);
        Assert.Equal("curriculos/arquivo.pdf", curriculo.StorageKey);
        Assert.Equal("application/pdf", curriculo.ContentType);
        Assert.Equal(1024, curriculo.TamanhoBytes);
        Assert.Equal(1, curriculo.Versao);
        Assert.True(curriculo.Ativo);
        Assert.Empty(curriculo.Candidaturas);
    }

    [Fact]
    public void Deve_Atualizar_Apenas_Dados_Do_Curriculo()
    {
        var curriculo = CriarCurriculo();

        curriculo.UpdateDetails("  Backend .NET 2  ", 2);

        Assert.Equal("Backend .NET 2", curriculo.Nome);
        Assert.Equal("curriculo.pdf", curriculo.NomeArquivo);
        Assert.Equal("curriculos/arquivo.pdf", curriculo.StorageKey);
        Assert.Equal(2, curriculo.Versao);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Nao_Deve_Criar_Curriculo_Sem_Nome(string nome)
    {
        Assert.Throws<ArgumentException>(() =>
            new Curriculo(nome, "curriculo.pdf", "arquivo.pdf", "application/pdf", 1024, 1));
    }

    [Fact]
    public void Nao_Deve_Criar_Curriculo_Com_Tipo_Diferente_De_Pdf()
    {
        Assert.Throws<ArgumentException>(() =>
            new Curriculo("Backend", "curriculo.txt", "arquivo.txt", "text/plain", 1024, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Nao_Deve_Criar_Curriculo_Com_Versao_Invalida(int versao)
    {
        Assert.Throws<ArgumentException>(() =>
            new Curriculo("Backend", "curriculo.pdf", "arquivo.pdf", "application/pdf", 1024, versao));
    }

    private static Curriculo CriarCurriculo()
    {
        return new Curriculo(
            "Currículo Backend .NET",
            "curriculo.pdf",
            "curriculos/arquivo.pdf",
            "application/pdf",
            1024,
            1);
    }
}
