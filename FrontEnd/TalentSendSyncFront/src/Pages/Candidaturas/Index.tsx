import { useState, useEffect, type FormEvent, type MouseEvent } from "react";
import { useNavigate } from "react-router-dom";
import { candidaturasApi } from "../../Services/api";
import { curriculosApi } from "../../Services/api";
import type { Candidatura, CreateCandidaturaDto, UpdateCandidaturaDto, Curriculo } from "../../Types";
import { Modal } from "../../Components/Modal";
import { ConfirmDialog } from "../../Components/ConfirmDialog";
import { PaginationControls } from "../../Components/PaginationControls";
import { getStatusLabel, statusOptions } from "../../Utils/status";

function getErrorMessage(error: unknown) {
  if (error && typeof error === "object" && "message" in error) {
    return String(error.message);
  }
  return "Não foi possível concluir a operação.";
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat("pt-BR", {
    dateStyle: "short",
    timeStyle: "short",
  }).format(new Date(value));
}

function normalizeSalaryValue(rawValue: string): number | null {
  if (!rawValue || !rawValue.trim()) return null;

  const value = rawValue.replace(/\s+/g, "").replace(/R\$/g, "").trim();
  if (!value) return null;

  if (value.includes(",") && value.includes(".")) {
    const normalized = value.replace(/\./g, "").replace(",", ".");
    return Number(normalized);
  }

  if (value.includes(",")) {
    return Number(value.replace(",", "."));
  }

  if (value.includes(".")) {
    const parts = value.split(".");
    if (parts.length > 2) {
      return Number(parts.join(""));
    }

    const integerPart = parts[0];
    const decimalPart = parts[1] ?? "";

    if (decimalPart.length === 3 && integerPart.length >= 1) {
      return Number(integerPart + decimalPart);
    }

    if (decimalPart.length <= 2) {
      return Number(value);
    }

    return Number(integerPart + decimalPart);
  }

  return Number(value);
}

export function CandidaturasPage() {
  const navigate = useNavigate();
  const [items, setItems] = useState<Candidatura[]>([]);
  const [curriculos, setCurriculos] = useState<Curriculo[]>([]);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<Candidatura | null>(null);
  const [deleteId, setDeleteId] = useState<string | null>(null);
  const [formData, setFormData] = useState<CreateCandidaturaDto>({
    curriculoId: "",
    empresa: "",
    cargo: "",
    pretensaoSalarial: null,
    linkVaga: "",
    status: "Enviado",
    observacoes: "",
  });

  const loadItems = async () => {
    setIsLoading(true);
    setError("");
    try {
      const response = await candidaturasApi.getPaged(pageNumber, pageSize);
      setItems(response.data);
      setTotalPages(response.pagination.totalPages);
      setTotalCount(response.pagination.totalCount);
    } catch (requestError) {
      setError(getErrorMessage(requestError));
    } finally {
      setIsLoading(false);
    }
  };

  const loadCurriculos = async () => {
    try {
      const response = await curriculosApi.getPaged(1, 100);
      setCurriculos(response.data.filter((c) => c.ativo));
    } catch {
      // Ignore error
    }
  };

  useEffect(() => {
    loadItems();
    loadCurriculos();
  }, [pageNumber]);

  const openCreate = () => {
    setEditingItem(null);
    setFormData({
      curriculoId: "",
      empresa: "",
      cargo: "",
      pretensaoSalarial: null,
      linkVaga: "",
      status: "Enviado",
      observacoes: "",
    });
    setError("");
    setIsModalOpen(true);
  };

  const openEdit = (item: Candidatura) => {
    setEditingItem(item);
    setFormData({
      curriculoId: item.curriculoId,
      empresa: item.empresa ?? "",
      cargo: item.cargo ?? "",
      pretensaoSalarial: item.pretensaoSalarial ?? null,
      linkVaga: item.linkVaga ?? "",
      status: item.status,
      observacoes: item.observacoes ?? "",
    });
    setError("");
    setIsModalOpen(true);
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError("");

    try {
      if (!formData.curriculoId) {
        setError("Selecione um currículo.");
        return;
      }

      if (!formData.empresa.trim()) {
        setError("Informe a empresa.");
        return;
      }

      if (!formData.cargo.trim()) {
        setError("Informe o cargo.");
        return;
      }

      const payload = {
        curriculoId: formData.curriculoId,
        empresa: formData.empresa.trim(),
        cargo: formData.cargo.trim(),
        pretensaoSalarial: formData.pretensaoSalarial !== null && formData.pretensaoSalarial !== undefined && formData.pretensaoSalarial !== "" ? Number(formData.pretensaoSalarial) : null,
        linkVaga: formData.linkVaga?.trim() || null,
        status: formData.status,
        observacoes: formData.observacoes?.trim() || null,
      };

      if (editingItem) {
        const updateData: UpdateCandidaturaDto = {
          candidaturaId: editingItem.candidaturaId,
          ...payload,
        };
        await candidaturasApi.update(editingItem.candidaturaId, updateData);
      } else {
        await candidaturasApi.create(payload as CreateCandidaturaDto);
      }
      setIsModalOpen(false);
      await loadItems();
    } catch (requestError) {
      setError(getErrorMessage(requestError));
    }
  };

  const handleDelete = async () => {
    if (!deleteId) return;
    try {
      await candidaturasApi.delete(deleteId);
      setDeleteId(null);
      await loadItems();
    } catch (requestError) {
      setError(getErrorMessage(requestError));
    }
  };

  const getCurriculoNome = (curriculoId: string) => {
    return curriculos.find((c) => c.curriculoId === curriculoId)?.nome || curriculoId;
  };

  const getObservacoesText = (value?: string | null) => {
    if (!value || !value.trim()) return "Sem observações";
    return value;
  };

  const formatCurrency = (value?: number | null) => {
    if (value === null || value === undefined || Number.isNaN(value)) return "Não informado";
    return new Intl.NumberFormat("pt-BR", {
      style: "currency",
      currency: "BRL",
    }).format(value);
  };

  const getStatusClass = (status: string) => {
    switch (status) {
      case "Aprovado":
        return "bg-emerald-900/30 text-emerald-400 border border-emerald-800/50";
      case "Rejeitado":
        return "bg-rose-900/30 text-rose-400 border border-rose-800/50";
      case "EmAndamento":
        return "bg-sky-900/30 text-sky-400 border border-sky-800/50";
      case "PropostaRecebida":
        return "bg-violet-900/30 text-violet-400 border border-violet-800/50";
      case "Cancelado":
        return "bg-slate-800 text-slate-400 border border-slate-700";
      case "SemRetorno":
        return "bg-amber-900/30 text-amber-400 border border-amber-800/50";
      default:
        return "bg-yellow-900/30 text-yellow-400 border border-yellow-800/50";
    }
  };

  const openDetails = (event: MouseEvent<HTMLElement>, candidaturaId: string) => {
    const target = event.target as HTMLElement;
    if (target.closest("button, a, input, select, textarea")) return;
    navigate(`/candidaturas/${candidaturaId}`);
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-slate-100">Candidaturas</h2>
          <p className="mt-1 text-sm text-slate-400">Gerenciar candidaturas de currículos</p>
        </div>
        <button onClick={openCreate} className="rounded-xl bg-gradient-to-r from-indigo-500 to-purple-500 px-4 py-2 text-sm font-semibold text-white shadow-lg shadow-indigo-500/20 transition hover:translate-y-[-1px] hover:shadow-indigo-500/30">
          Nova Candidatura
        </button>
      </div>

      {error && <div className="rounded-xl border border-rose-900/50 bg-rose-900/20 p-3 text-sm text-rose-400">{error}</div>}

      <div className="space-y-3">
        {isLoading ? (
          <div className="rounded-xl border border-slate-800 bg-slate-900 px-4 py-8 text-center text-sm text-slate-400">Carregando...</div>
        ) : items.length === 0 ? (
          <div className="rounded-xl border border-slate-800 bg-slate-900 px-4 py-8 text-center text-sm text-slate-400">Nenhuma candidatura encontrada</div>
        ) : (
          items.map((item) => (
            <article
              key={item.candidaturaId}
              onClick={(event) => openDetails(event, item.candidaturaId)}
              onKeyDown={(event) => {
                if (event.key === "Enter" || event.key === " ") {
                  event.preventDefault();
                  navigate(`/candidaturas/${item.candidaturaId}`);
                }
              }}
              role="link"
              tabIndex={0}
              className="block cursor-pointer rounded-2xl border border-slate-800 bg-slate-900 p-4 transition duration-200 hover:-translate-y-0.5 hover:border-indigo-500/50 hover:shadow-lg hover:shadow-indigo-500/10 focus:outline-none focus:ring-2 focus:ring-indigo-500/60"
            >
              <div className="mb-3 flex items-start justify-between gap-3">
                <div>
                  <p className="mb-1 text-[10px] font-semibold uppercase tracking-[0.16em] text-slate-500">Candidatura</p>
                  <h3 className="text-lg font-semibold leading-6 text-slate-100">{item.empresa || "Empresa não informada"}</h3>
                </div>
                <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-[10px] font-medium ${getStatusClass(item.status)}`}>
                  {getStatusLabel(item.statusNome ?? item.status)}
                </span>
              </div>

              <div className="space-y-2 text-sm text-slate-300">
                <div>
                  <span className="mr-2 font-semibold text-slate-500">Currículo:</span>
                  <span>{getCurriculoNome(item.curriculoId)}</span>
                </div>
                <div>
                  <span className="mr-2 font-semibold text-slate-500">Cargo:</span>
                  <span>{item.cargo || "Não informado"}</span>
                </div>
                <div>
                  <span className="mr-2 font-semibold text-slate-500">Pretensão:</span>
                  <span>{formatCurrency(item.pretensaoSalarial)}</span>
                </div>
                <div>
                  <span className="mr-2 font-semibold text-slate-500">Data:</span>
                  <span>{formatDate(item.dataCandidatura)}</span>
                </div>
                <div>
                  <span className="mr-2 font-semibold text-slate-500">Vaga:</span>
                  {item.linkVaga ? (
                    <a href={item.linkVaga} target="_blank" rel="noreferrer" className="break-all text-indigo-400 underline decoration-2 underline-offset-2 hover:text-indigo-300">
                      {item.linkVaga}
                    </a>
                  ) : (
                    <span>Não informado</span>
                  )}
                </div>
                <div>
                  <span className="mr-2 font-semibold text-slate-500">Observações:</span>
                  <span>{getObservacoesText(item.observacoes)}</span>
                </div>
              </div>

              <div className="mt-4 flex justify-end gap-3 border-t border-slate-800 pt-3 text-sm">
                <button onClick={() => navigate(`/candidaturas/${item.candidaturaId}`)} className="text-slate-400 hover:text-slate-200">Ver detalhes</button>
                <button onClick={() => openEdit(item)} className="text-indigo-400 hover:text-indigo-300">Editar</button>
                <button onClick={() => setDeleteId(item.candidaturaId)} className="text-rose-400 hover:text-rose-300">Excluir</button>
              </div>
            </article>
          ))
        )}
      </div>

      <PaginationControls
        pageNumber={pageNumber}
        totalPages={totalPages}
        totalCount={totalCount}
        pageSize={pageSize}
        onPageChange={setPageNumber}
        label="candidatura(s)"
      />

      <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title={editingItem ? "Editar Candidatura" : "Nova Candidatura"}>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="mb-1.5 block text-sm font-medium text-slate-300">Currículo</label>
            <select required value={formData.curriculoId} onChange={(e) => setFormData({ ...formData, curriculoId: e.target.value })} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-white">
              <option value="">Selecione um currículo</option>
              {curriculos.map((c) => (
                <option key={c.curriculoId} value={c.curriculoId}>{c.nome}</option>
              ))}
            </select>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="mb-1.5 block text-sm font-medium text-slate-300">Empresa</label>
              <input required value={formData.empresa} onChange={(e) => setFormData({ ...formData, empresa: e.target.value })} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-white" placeholder="Ex: Empresa XPTO" />
            </div>
            <div>
              <label className="mb-1.5 block text-sm font-medium text-slate-300">Cargo</label>
              <input required value={formData.cargo} onChange={(e) => setFormData({ ...formData, cargo: e.target.value })} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-white" placeholder="Ex: Desenvolvedor Frontend" />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="mb-1.5 block text-sm font-medium text-slate-300">Pretensão Salarial</label>
              <input
                type="text"
                inputMode="decimal"
                value={formData.pretensaoSalarial === null || formData.pretensaoSalarial === undefined ? "" : String(formData.pretensaoSalarial)}
                onChange={(e) => {
                  const nextValue = normalizeSalaryValue(e.target.value);
                  setFormData({ ...formData, pretensaoSalarial: nextValue });
                }}
                className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-white"
                placeholder="5500"
              />
            </div>
            <div>
              <label className="mb-1.5 block text-sm font-medium text-slate-300">Status</label>
              <select value={formData.status} onChange={(e) => setFormData({ ...formData, status: e.target.value })} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-white">
                <option value="Enviado">Enviado</option>
                {statusOptions.slice(1).map((option) => (
                  <option key={option.value} value={option.value}>{option.label}</option>
                ))}
              </select>
            </div>
          </div>

          <div>
            <label className="mb-1.5 block text-sm font-medium text-slate-300">Link da vaga</label>
            <input value={formData.linkVaga ?? ""} onChange={(e) => setFormData({ ...formData, linkVaga: e.target.value })} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-white" placeholder="https://..." />
          </div>

          <div>
            <label className="mb-1.5 block text-sm font-medium text-slate-300">Observações</label>
            <textarea value={formData.observacoes} onChange={(e) => setFormData({ ...formData, observacoes: e.target.value })} rows={3} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-white" />
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button type="button" onClick={() => setIsModalOpen(false)} className="rounded-lg border border-slate-700 px-4 py-2 text-sm font-medium text-slate-300 hover:bg-slate-800">Cancelar</button>
            <button type="submit" className="rounded-lg bg-indigo-500 px-4 py-2 text-sm font-semibold text-white hover:bg-indigo-400 transition-colors">Salvar</button>
          </div>
        </form>
      </Modal>

      <ConfirmDialog isOpen={!!deleteId} onClose={() => setDeleteId(null)} onConfirm={handleDelete} title="Excluir Candidatura" message="Tem certeza que deseja excluir esta candidatura?" />
    </div>
  );
}

export default CandidaturasPage;
