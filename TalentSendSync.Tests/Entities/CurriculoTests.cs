using TalentSendSync.Domain.Entities;
using Xunit;

namespace TalentSendSync.Tests.Entities;

public class CurriculoTests
{
    [Fact]
    public void Deve_Criar_Curriculo_Com_Dados_Validos()
    {
        // Arrange
        var nome = "Currículo Backend .NET";
        var nomeArquivo = "curriculo-backend-v1.pdf";
        var urlArquivo = "https://google.com/curriculo.pdf";
        var versao = 1;

        // Act
        var curriculo = new Curriculo(
            nome,
            nomeArquivo,
            urlArquivo,
            versao);

        // Assert
        Assert.NotEqual(Guid.Empty, curriculo.CurriculoId);
        Assert.Equal(nome, curriculo.Nome);
        Assert.Equal(nomeArquivo, curriculo.NomeArquivo);
        Assert.Equal(urlArquivo, curriculo.UrlArquivo);
        Assert.Equal(versao, curriculo.Versao);
        Assert.True(curriculo.Ativo);
        Assert.InRange(curriculo.DataCriacao, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);
        Assert.Empty(curriculo.Candidaturas);
    }

    [Fact]
    public void Deve_Remover_Espacos_Dos_Dados_Textuais()
    {
        var curriculo = new Curriculo(
            "  Backend .NET  ",
            "  curriculo.pdf  ",
            "  https://example.com/curriculo.pdf  ",
            2);

        Assert.Equal("Backend .NET", curriculo.Nome);
        Assert.Equal("curriculo.pdf", curriculo.NomeArquivo);
        Assert.Equal("https://example.com/curriculo.pdf", curriculo.UrlArquivo);
    }


    [Fact]
    public void Nao_Deve_Criar_Curriculo_Sem_Nome()
    {
        // Arrange
        var nome = "";
        var nomeArquivo = "curriculo1.pdf";
        var urlArquivo = "https://google.com/curriculo.pdf";
        var versao = 1;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Curriculo(
                nome,
                nomeArquivo,
                urlArquivo,
                versao));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Nao_Deve_Criar_Curriculo_Com_Nome_Vazio(string nome)
    {
        Assert.Throws<ArgumentException>(() =>
            new Curriculo(nome, "curriculo.pdf", "https://example.com/curriculo.pdf", 1));
    }

    [Fact]
    public void Nao_Deve_Criar_Curriculo_Sem_Nome_De_Arquivo()
    {
        Assert.Throws<ArgumentException>(() =>
            new Curriculo("Backend", "", "https://example.com/curriculo.pdf", 1));
    }

    [Fact]
    public void Nao_Deve_Criar_Curriculo_Sem_Url_De_Arquivo()
    {
        Assert.Throws<ArgumentException>(() =>
            new Curriculo("Backend", "curriculo.pdf", "", 1));
    }

    [Fact]
    public void Nao_Deve_Criar_Curriculo_Com_Url_Invalida()
    {
        Assert.Throws<ArgumentException>(() =>
            new Curriculo("Backend", "curriculo.pdf", "url-invalida", 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Nao_Deve_Criar_Curriculo_Com_Versao_Invalida(int versao)
    {
        Assert.Throws<ArgumentException>(() =>
            new Curriculo("Backend", "curriculo.pdf", "https://example.com/curriculo.pdf", versao));
    }

    [Fact]
    public void Nao_Deve_Aceitar_Nome_Com_Mais_De_150_Caracteres()
    {
        Assert.Throws<ArgumentException>(() =>
            new Curriculo(new string('X', 151), "curriculo.pdf", "https://example.com/curriculo.pdf", 1));
    }

    [Fact]
    public void Nao_Deve_Aceitar_Nome_De_Arquivo_Com_Mais_De_255_Caracteres()
    {
        Assert.Throws<ArgumentException>(() =>
            new Curriculo("Backend", new string('X', 256), "https://example.com/curriculo.pdf", 1));
    }
}