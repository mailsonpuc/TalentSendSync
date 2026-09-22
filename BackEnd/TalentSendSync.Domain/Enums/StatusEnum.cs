using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TalentSendSync.Domain.Enums;

public enum StatusEnum
{
    [Display(Name = "Enviado")]
    Enviado = 1,

    [Display(Name = "Em Andamento")]
    EmAndamento = 2,

    [Display(Name = "Proposta Recebida")]
    PropostaRecebida = 3,

    [Display(Name = "Aprovado")]
    Aprovado = 4,

    [Display(Name = "Rejeitado")]
    Rejeitado = 5,

    [Display(Name = "Cancelado")]
    Cancelado = 6,

    [Display(Name = "Sem retorno")]
    SemRetorno = 7
}

public static class StatusEnumExtensions
{
    public static string GetDisplayName(this StatusEnum status)
    {
        var field = typeof(StatusEnum).GetField(status.ToString());
        return field?.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()?.GetName()
            ?? status.ToString();
    }
}


