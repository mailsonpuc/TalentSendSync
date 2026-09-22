export interface Curriculo {
  curriculoId: string;
  nome: string;
  nomeArquivo: string;
  tamanhoBytes: number;
  versao: number;
  dataCriacao: string;
  ativo: boolean;
  storageKey?: string;
  contentType?: string;
}

export interface CreateCurriculoInput {
  nome: string;
  versao: number;
  arquivo: File;
}

export interface UpdateCurriculoInput {
  nome: string;
  versao: number;
  arquivo?: File;
}

export interface Candidatura {
  candidaturaId: string;
  empresa: string;
  cargo: string;
  curriculoId: string;
  curriculoNome?: string;
  dataCandidatura: string;
  dataEnvio?: string;
  pretensaoSalarial?: number | null;
  linkVaga?: string | null;
  status: string;
  statusNome?: string;
  observacoes?: string;
  historicosContato?: HistoricoContato[];
}

export interface CreateCandidaturaDto {
  curriculoId: string;
  empresa: string;
  cargo: string;
  pretensaoSalarial?: number | null;
  linkVaga?: string | null;
  status: string;
  observacoes?: string;
}

export interface UpdateCandidaturaDto {
  candidaturaId: string;
  curriculoId: string;
  empresa: string;
  cargo: string;
  pretensaoSalarial?: number | null;
  linkVaga?: string | null;
  status: string;
  observacoes?: string;
}

export interface HistoricoContato {
  historicoContatoId: string;
  candidaturaId: string;
  candidaturaStatus?: string;
  candidaturaStatusNome?: string;
  tipoContato: string;
  dataContato: string;
  descricao: string;
  responsavel?: string;
}

export interface CreateHistoricoContatoDto {
  candidaturaId: string;
  tipoContato: string;
  dataContato: string;
  descricao: string;
  responsavel?: string;
}

export interface UpdateHistoricoContatoDto {
  historicoContatoId: string;
  candidaturaId: string;
  tipoContato: string;
  dataContato: string;
  descricao: string;
  responsavel?: string;
}

export interface PaginationMetadata {
  totalCount: number;
  pageSize: number;
  currentPage: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface PagedResponse<T> {
  data: T[];
  pagination: PaginationMetadata;
}

export interface DashboardStatus {
  status: string;
  quantidade: number;
}

export interface DashboardEvolucao {
  mes: string;
  quantidade: number;
}

export interface DashboardCandidatura {
  candidaturaId: string;
  empresa: string;
  cargo: string;
  dataEnvio: string;
  status: string;
  statusNome: string;
}

export interface Dashboard {
  totalCandidaturas: number;
  enviadas: number;
  emAndamento: number;
  propostasRecebidas: number;
  aprovadas: number;
  rejeitadas: number;
  canceladas: number;
  semRetorno: number;
  taxaDeResposta: number;
  taxaDeAprovacao: number;
  status: DashboardStatus[];
  evolucao: DashboardEvolucao[];
  ultimasCandidaturas: DashboardCandidatura[];
}