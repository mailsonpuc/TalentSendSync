using System.ComponentModel.DataAnnotations;

namespace TalentSendSync.Domain.Enums;

public enum StatusEnum
{
    Enviado = 1,           // Currículo enviado, aguardando retorno

    [Display(Name = "Em Andamento")]
    EmAndamento = 2,       // Entrou em contato, em fase de entrevistas/testes

    [Display(Name = "Proposta Recebida")]
    PropostaRecebida = 3,  // Recebeu oferta de trabalho
    Aprovado = 4,          // Proposta aceita / Contratado
    Rejeitado = 5,         // Processo encerrado pela empresa
    Cancelado = 6,         // Desistência da sua parte
    SemRetorno = 7
}


