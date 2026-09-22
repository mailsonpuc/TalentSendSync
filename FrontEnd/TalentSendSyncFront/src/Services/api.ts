import api from "./Api";
import type {
  Curriculo,
  CreateCurriculoInput,
  UpdateCurriculoInput,
  Candidatura,
  CreateCandidaturaDto,
  UpdateCandidaturaDto,
  HistoricoContato,
  CreateHistoricoContatoDto,
  UpdateHistoricoContatoDto,
  PagedResponse,
  Dashboard,
} from "./Types";

const getPaged = async <T,>(endpoint: string, pageNumber: number, pageSize: number): Promise<PagedResponse<T>> => {
  const response = await api.get(`${endpoint}/pagination`, {
    params: { pageNumber, pageSize },
  });

  return {
    data: response.data.items ?? [],
    pagination: {
      totalCount: response.data.totalCount ?? 0,
      pageSize: response.data.pageSize ?? pageSize,
      currentPage: response.data.currentPage ?? pageNumber,
      totalPages: response.data.totalPages ?? 0,
      hasNextPage: (response.data.currentPage ?? pageNumber) < (response.data.totalPages ?? 0),
      hasPreviousPage: (response.data.currentPage ?? pageNumber) > 1,
    },
  };
};

export const curriculosApi = {
  getPaged: (pageNumber: number, pageSize: number): Promise<PagedResponse<Curriculo>> =>
    getPaged<Curriculo>("/Curriculos", pageNumber, pageSize),

  getById: async (id: string): Promise<Curriculo> => {
    const response = await api.get(`/Curriculos/${id}`);
    return response.data;
  },

  create: async (data: CreateCurriculoInput): Promise<Curriculo> => {
    const formData = new FormData();
    formData.append("Nome", data.nome);
    formData.append("Versao", String(data.versao));
    formData.append("Arquivo", data.arquivo);

    const response = await api.post("/Curriculos", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
    return response.data;
  },

  update: async (id: string, data: UpdateCurriculoInput): Promise<Curriculo> => {
    const formData = new FormData();
    formData.append("Nome", data.nome);
    formData.append("Versao", String(data.versao));
    if (data.arquivo) {
      formData.append("Arquivo", data.arquivo);
    }

    const response = await api.put(`/Curriculos/${id}`, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/Curriculos/${id}`);
  },

  download: (id: string): string => {
    return `${api.defaults.baseURL}/Curriculos/${id}/arquivo`;
  },
};

export const candidaturasApi = {
  getPaged: (pageNumber: number, pageSize: number): Promise<PagedResponse<Candidatura>> =>
    getPaged<Candidatura>("/Candidaturas", pageNumber, pageSize),

  getById: async (id: string): Promise<Candidatura> => {
    const response = await api.get(`/Candidaturas/${id}`);
    return response.data;
  },

  create: async (data: CreateCandidaturaDto): Promise<Candidatura> => {
    const response = await api.post("/Candidaturas", data);
    return response.data;
  },

  update: async (id: string, data: UpdateCandidaturaDto): Promise<Candidatura> => {
    const response = await api.put(`/Candidaturas/${id}`, data);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/Candidaturas/${id}`);
  },
};

export const historicoContatoApi = {
  getPaged: (pageNumber: number, pageSize: number): Promise<PagedResponse<HistoricoContato>> =>
    getPaged<HistoricoContato>("/HistoricoContato", pageNumber, pageSize),

  getById: async (id: string): Promise<HistoricoContato> => {
    const response = await api.get(`/HistoricoContato/${id}`);
    return response.data;
  },

  create: async (data: CreateHistoricoContatoDto): Promise<HistoricoContato> => {
    const response = await api.post("/HistoricoContato", data);
    return response.data;
  },

  update: async (id: string, data: UpdateHistoricoContatoDto): Promise<HistoricoContato> => {
    const response = await api.put(`/HistoricoContato/${id}`, data);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/HistoricoContato/${id}`);
  },
};

export const dashboardApi = {
  getCandidaturas: async (): Promise<Dashboard> => {
    const response = await api.get("/Dashboard/Candidaturas");
    return response.data;
  },
};