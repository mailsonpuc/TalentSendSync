import { useState, useEffect, type FormEvent } from "react";
import { historicoContatoApi } from "../../Services/api";
import { candidaturasApi } from "../../Services/api";
import type { HistoricoContato, CreateHistoricoContatoDto, UpdateHistoricoContatoDto, Candidatura } from "../../Types";
import { Modal } from "../../Components/Modal";
import { ConfirmDialog } from "../../Components/ConfirmDialog";
import { PaginationControls } from "../../Components/PaginationControls";
import { getStatusLabel } from "../../Utils/status";

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

export function HistoricoContatoPage() {
  const [items, setItems] = useState<HistoricoContato[]>([]);
  const [candidaturas, setCandidaturas] = useState<Candidatura[]>([]);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<HistoricoContato | null>(null);
  const [deleteId, setDeleteId] = useState<string | null>(null);
  const [formData, setFormData] = useState<CreateHistoricoContatoDto>({
    candidaturaId: "",
    tipoContato: "Email",
    dataContato: new Date().toISOString().slice(0, 16),
    descricao: "",
    responsavel: "",
  });

  const loadItems = async () => {
    setIsLoading(true);
    setError("");
    try {
      const response = await historicoContatoApi.getPaged(pageNumber, pageSize);
      setItems(response.data);
      setTotalPages(response.pagination.totalPages);
      setTotalCount(response.pagination.totalCount);
    } catch (requestError) {
      setError(getErrorMessage(requestError));
    } finally {
      setIsLoading(false);
    }
  };

  const loadCandidaturas = async () => {
    try {
      const response = await candidaturasApi.getPaged(1, 100);
      setCandidaturas(response.data);
    } catch {
      // Ignore error
    }
  };

  useEffect(() => {
    loadItems();
    loadCandidaturas();
  }, [pageNumber]);

  const openCreate = () => {
    setEditingItem(null);
    setFormData({
      candidaturaId: "",
      tipoContato: "Email",
      dataContato: new Date().toISOString().slice(0, 16),
      descricao: "",
      responsavel: "",
    });
    setError("");
    setIsModalOpen(true);
  };

  const openEdit = (item: HistoricoContato) => {
    setEditingItem(item);
    setFormData({
      candidaturaId: item.candidaturaId,
      tipoContato: item.tipoContato,
      dataContato: item.dataContato.slice(0, 16),
      descricao: item.descricao,
      responsavel: item.responsavel ?? "",
    });
    setError("");
    setIsModalOpen(true);
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError("");

    try {
      if (editingItem) {
        const updateData: UpdateHistoricoContatoDto = {
          historicoContatoId: editingItem.historicoContatoId,
          candidaturaId: formData.candidaturaId,
          tipoContato: formData.tipoContato,
          dataContato: formData.dataContato,
          descricao: formData.descricao,
          responsavel: formData.responsavel,
        };
        await historicoContatoApi.update(editingItem.historicoContatoId, updateData);
      } else {
        if (!formData.candidaturaId) {
          setError("Selecione uma candidatura.");
          return;
        }
        await historicoContatoApi.create(formData);
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
      await historicoContatoApi.delete(deleteId);
      setDeleteId(null);
      await loadItems();
    } catch (requestError) {
      setError(getErrorMessage(requestError));
    }
  };

  const getCandidaturaInfo = (candidaturaId: string) => {
    const c = candidaturas.find((cand) => cand.candidaturaId === candidaturaId);
    return c ? `${c.empresa || "Candidatura"} • ${getStatusLabel(c.statusNome ?? c.status)}` : candidaturaId;
  };

  const getTipoContatoClass = (tipo: string) => {
    switch (tipo) {
      case "Email":
        return "bg-blue-900/30 text-blue-400 border border-blue-800/50";
      case "Telefone":
        return "bg-emerald-900/30 text-emerald-400 border border-emerald-800/50";
      case "WhatsApp":
        return "bg-green-900/30 text-green-400 border border-green-800/50";
      case "Presencial":
        return "bg-violet-900/30 text-violet-400 border border-violet-800/50";
      case "LinkedIn":
        return "bg-cyan-900/30 text-cyan-400 border border-cyan-800/50";
      default:
        return "bg-slate-800 text-slate-400 border border-slate-700";
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-slate-100">Histórico de Contato</h2>
          <p className="mt-1 text-sm text-slate-400">Gerenciar histórico de contatos das candidaturas</p>
        </div>
        <button onClick={openCreate} className="rounded-xl bg-gradient-to-r from-indigo-500 to-purple-500 px-4 py-2 text-sm font-semibold text-white shadow-lg shadow-indigo-500/20 transition hover:translate-y-[-1px] hover:shadow-indigo-500/30">
          Novo Contato
        </button>
      </div>

      {error && <div className="rounded-xl border border-rose-900/50 bg-rose-900/20 p-3 text-sm text-rose-400">{error}</div>}

      <div className="space-y-3">
        {isLoading ? (
          <div className="rounded-xl border border-slate-800 bg-slate-900 px-4 py-8 text-center text-sm text-slate-400">Carregando...</div>
        ) : items.length === 0 ? (
          <div className="rounded-xl border border-slate-800 bg-slate-900 px-4 py-8 text-center text-sm text-slate-400">Nenhum histórico de contato encontrado</div>
        ) : (
          items.map((item) => (
            <article
              key={item.historicoContatoId}
              className="block rounded-2xl border border-slate-800 bg-slate-900 p-4 transition duration-200 hover:-translate-y-0.5 hover:border-indigo-500/50 hover:shadow-lg hover:shadow-indigo-500/10"
            >
              <div className="mb-3 flex items-start justify-between gap-3">
                <div>
                  <p className="mb-1 text-[10px] font-semibold uppercase tracking-[0.16em] text-slate-500">Contato</p>
                  <h3 className="text-lg font-semibold leading-6 text-red-600">{getCandidaturaInfo(item.candidaturaId)}</h3>
                </div>
                <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-[10px] font-medium 
                  ${getTipoContatoClass(item.tipoContato)}`}>
                  {item.tipoContato}
                </span>
              </div>

              <div className="space-y-2 text-sm text-slate-300">
                <div>
                  <span className="mr-2 font-semibold text-slate-500">Candidatura:</span>
                  <span className="text-red-600">{getCandidaturaInfo(item.candidaturaId)}</span>

                </div>
                <div>
                  <span className="mr-2 font-semibold text-slate-500">Tipo:</span>
                  <span>{item.tipoContato}</span>
                </div>
                <div>
                  <span className="mr-2 font-semibold text-slate-500">Data:</span>
                  <span>{formatDate(item.dataContato)}</span>
                </div>
                <div>
                  <span className="mr-2 font-semibold text-slate-500">Responsável:</span>
                  <span>{item.responsavel || "Não informado"}</span>
                </div>
                <div>
                  <span className="mr-2 font-semibold text-slate-500">Descrição:</span>
                  <span>{item.descricao || "Sem descrição"}</span>
                </div>
              </div>

              <div className="mt-4 flex justify-end gap-3 border-t border-slate-800 pt-3 text-sm">
                <button onClick={() => openEdit(item)} className="text-indigo-400 hover:text-indigo-300">Editar</button>
                <button onClick={() => setDeleteId(item.historicoContatoId)} className="text-rose-400 hover:text-rose-300">Excluir</button>
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
        label="contato(s)"
      />

      <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title={editingItem ? "Editar Contato" : "Novo Contato"}>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="mb-1.5 block text-sm font-medium text-slate-300">Candidatura</label>
            <select required value={formData.candidaturaId} onChange={(e) => setFormData({ ...formData, candidaturaId: e.target.value })} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-white">
              <option value="">Selecione uma candidatura</option>
              {candidaturas.map((c) => (
                <option key={c.candidaturaId} value={c.candidaturaId}>{c.curriculoNome || `Candidatura ${c.candidaturaId.slice(0, 8)}`} - {c.status}</option>
              ))}
            </select>
          </div>
          <div>
            <label className="mb-1.5 block text-sm font-medium text-slate-300">Tipo de Contato</label>
            <select value={formData.tipoContato} onChange={(e) => setFormData({ ...formData, tipoContato: e.target.value })} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-white">
              <option value="Email">Email</option>
              <option value="Telefone">Telefone</option>
              <option value="WhatsApp">WhatsApp</option>
              <option value="Presencial">Presencial</option>
              <option value="LinkedIn">LinkedIn</option>
              <option value="Outro">Outro</option>
            </select>
          </div>
          <div>
            <label className="mb-1.5 block text-sm font-medium text-slate-300">Data do Contato</label>
            <input type="datetime-local" value={formData.dataContato} onChange={(e) => setFormData({ ...formData, dataContato: e.target.value })} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-white" />
          </div>
          <div>
            <label className="mb-1.5 block text-sm font-medium text-slate-300">Descrição</label>
            <textarea required value={formData.descricao} onChange={(e) => setFormData({ ...formData, descricao: e.target.value })} rows={3} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-white" />
          </div>
          <div>
            <label className="mb-1.5 block text-sm font-medium text-slate-300">Responsável</label>
            <input value={formData.responsavel} onChange={(e) => setFormData({ ...formData, responsavel: e.target.value })} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-white" />
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button type="button" onClick={() => setIsModalOpen(false)} className="rounded-lg border border-slate-700 px-4 py-2 text-sm font-medium text-slate-300 hover:bg-slate-800">Cancelar</button>
            <button type="submit" className="rounded-lg bg-indigo-500 px-4 py-2 text-sm font-semibold text-white hover:bg-indigo-400 transition-colors">Salvar</button>
          </div>
        </form>
      </Modal>

      <ConfirmDialog isOpen={!!deleteId} onClose={() => setDeleteId(null)} onConfirm={handleDelete} title="Excluir Contato" message="Tem certeza que deseja excluir este histórico de contato?" />
    </div>
  );
}

export default HistoricoContatoPage;
