using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Enums;
using Xunit;

namespace TalentSendSync.Tests.Entities;

public class CandidaturaTests
{
    [Fact]
    public void Deve_Criar_Candidatura_Com_Dados_Validos()
    {
        // Arrange
        var empresa = "Google";
        var cargo = "Desenvolvedor Backend";
        var pretensaoSalarial = 5600m;
        var linkVaga = "https://google.com/vaga";
        var status = StatusEnum.Enviado;
        
        // Act & Assert
        var candidatura = new Candidatura(empresa, cargo, pretensaoSalarial, linkVaga, status);
    }
    
    
    
    [Fact]
    public void Nao_Deve_Criar_Candidatura_Sem_Empresa()
    {
        // Arrange
        var empresa = "";
        var cargo = "Desenvolvedor Backend";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Candidatura(empresa, cargo, 5600m, "https://google.com/vaga", StatusEnum.Enviado));
    }
    
    
    [Fact]
    public void Nao_Deve_Criar_Candidatura_Sem_Cargo()
    {
        // Arrange
        var empresa = "Google";
        var cargo = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Candidatura(
                empresa,
                cargo,
                5600m,
                "https://google.com/vaga",
                StatusEnum.Enviado));
    }

    
    
    [Fact]
    public void Nao_Deve_Aceitar_Pretensao_Salarial_Negativa()
    {
        // Arrange
        var pretensaoSalarial = -100m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Candidatura(
                "Google",
                "Desenvolvedor Backend",
                pretensaoSalarial,
                "https://google.com/vaga",
                StatusEnum.Enviado));
    }
    
    
    
    [Fact]
    public void Nao_Deve_Aceitar_Link_De_Vaga_Invalido()
    {
        // Arrange
        var linkVaga = "link-invalido";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Candidatura(
                "Google",
                "Desenvolvedor Backend",
                5600m,
                linkVaga,
                StatusEnum.Enviado));
    }
    
    
    [Fact]
    public void Nome_Empresa_deve_possuir_no_maximo_200_caracteres()
    {
        // Arrange
        var empresa = new string('X', 201);
        var cargo = "Desenvolvedor Backend";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Candidatura(
                empresa,
                cargo,
                5600m,
                "https://google.com/vaga",
                StatusEnum.Enviado));
    }
    
    [Fact]
    public void Cargo_deve_possuir_no_maximo_150_caracteres()
    {
        // Arrange
        var cargo = new string('X', 151);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Candidatura(
                "Google inc",
                cargo,
                5600m,
                "https://google.com/vaga",
                StatusEnum.Enviado));
    }
    
    [Fact]
    public void Cargo_com_150_caracteres_deve_ser_aceito()
    {
        // Arrange
        var cargo = new string('X', 150);

        // Act
        var candidatura = new Candidatura(
            "Google inc",
            cargo,
            5600m,
            "https://google.com/vaga",
            StatusEnum.Enviado);

        // Assert
        Assert.Equal(150, candidatura.Cargo.Length);
    }
}
