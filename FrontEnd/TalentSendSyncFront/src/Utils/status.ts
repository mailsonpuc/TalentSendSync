const statusLabels: Record<string, string> = {
  Enviado: "Enviado",
  EmAndamento: "Em Andamento",
  "Em Andamento": "Em Andamento",
  PropostaRecebida: "Proposta Recebida",
  "Proposta Recebida": "Proposta Recebida",
  Aprovado: "Aprovado",
  Rejeitado: "Rejeitado",
  Cancelado: "Cancelado",
  SemRetorno: "Sem retorno",
  "Sem retorno": "Sem retorno",
};

export function getStatusLabel(status?: string | null): string {
  if (!status) return "Não informado";
  return statusLabels[status] ?? status;
}

export const statusOptions = [
  { value: "Enviado", label: "Enviado" },
  { value: "EmAndamento", label: "Em Andamento" },
  { value: "PropostaRecebida", label: "Proposta Recebida" },
  { value: "Aprovado", label: "Aprovado" },
  { value: "Rejeitado", label: "Rejeitado" },
  { value: "Cancelado", label: "Cancelado" },
  { value: "SemRetorno", label: "Sem retorno" },
];