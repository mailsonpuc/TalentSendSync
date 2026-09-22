import { useState, useEffect, type ChangeEvent, type FormEvent } from "react";
import { curriculosApi } from "../../Services/api";
import type { Curriculo } from "../../Types";
import { Modal } from "../../Components/Modal";
import { ConfirmDialog } from "../../Components/ConfirmDialog";
import { PaginationControls } from "../../Components/PaginationControls";

function getErrorMessage(error: unknown) {
  if (error && typeof error === "object" && "message" in error) {
    return String(error.message);
  }
  return "Não foi possível concluir a operação.";
}

function formatFileSize(bytes: number) {
  if (bytes < 1024) return `${bytes} B`;
  return `${(bytes / 1024 / 1024).toFixed(2)} MB`;
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat("pt-BR", {
    dateStyle: "short",
    timeStyle: "short",
  }).format(new Date(value));
}

export function CurriculosPage() {
  const [items, setItems] = useState<Curriculo[]>([]);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<Curriculo | null>(null);
  const [deleteId, setDeleteId] = useState<string | null>(null);
  const [formData, setFormData] = useState({ nome: "", versao: "1", arquivo: null as File | null });

  const loadItems = async () => {
    setIsLoading(true);
    setError("");
    try {
      const response = await curriculosApi.getPaged(pageNumber, pageSize);
      setItems(response.data);
      setTotalPages(response.pagination.totalPages);
      setTotalCount(response.pagination.totalCount);
    } catch (requestError) {
      setError(getErrorMessage(requestError));
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadItems();
  }, [pageNumber]);

  const openCreate = () => {
    setEditingItem(null);
    setFormData({ nome: "", versao: "1", arquivo: null });
    setError("");
    setIsModalOpen(true);
  };

  const openEdit = (item: Curriculo) => {
    setEditingItem(item);
    setFormData({ nome: item.nome, versao: String(item.versao), arquivo: null });
    setError("");
    setIsModalOpen(true);
  };

  const handleFileChange = (event: ChangeEvent<HTMLInputElement>) => {
    setFormData((current) => ({ ...current, arquivo: event.target.files?.[0] ?? null }));
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError("");
    const data = new FormData();
    data.append("Nome", formData.nome);
    data.append("Versao", formData.versao);
    if (formData.arquivo) data.append("Arquivo", formData.arquivo);

    try {
      if (editingItem) {
        await curriculosApi.update(editingItem.curriculoId, {
          nome: formData.nome,
          versao: Number(formData.versao),
          arquivo: formData.arquivo ?? undefined,
        });
      } else {
        if (!formData.arquivo) {
          setError("Selecione um arquivo PDF.");
          return;
        }
        await curriculosApi.create({
          nome: formData.nome,
          versao: Number(formData.versao),
          arquivo: formData.arquivo,
        });
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
      await curriculosApi.delete(deleteId);
      setDeleteId(null);
      await loadItems();
    } catch (requestError) {
      setError(getErrorMessage(requestError));
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-slate-100">Currículos</h2>
          <p className="mt-1 text-sm text-slate-400">Gerenciar currículos em PDF</p>
        </div>
        <button onClick={openCreate} className="rounded-lg bg-indigo-500 px-4 py-2 text-sm font-semibold text-white shadow-md shadow-indigo-500/25 hover:bg-indigo-400 transition-colors">
          Novo Currículo
        </button>
      </div>

      {error && <div className="rounded-lg border border-rose-900/50 bg-rose-900/20 p-3 text-sm text-rose-400">{error}</div>}

      <div className="overflow-hidden rounded-xl border border-slate-800 bg-slate-900">
        <table className="w-full text-left text-sm">
          <thead className="border-b border-slate-800 bg-slate-800/50">
            <tr>
              <th className="px-6 py-3 font-medium text-slate-300">Nome</th>
              <th className="px-6 py-3 font-medium text-slate-300">Arquivo</th>
              <th className="px-6 py-3 font-medium text-slate-300">Versão</th>
              <th className="px-6 py-3 font-medium text-slate-300">Tamanho</th>
              <th className="px-6 py-3 font-medium text-slate-300">Criado em</th>
              <th className="px-6 py-3 font-medium text-slate-300">Ações</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-800">
            {isLoading ? (
              <tr><td colSpan={6} className="px-6 py-8 text-center text-slate-400">Carregando...</td></tr>
            ) : items.length === 0 ? (
              <tr><td colSpan={6} className="px-6 py-8 text-center text-slate-400">Nenhum currículo encontrado</td></tr>
            ) : (
              items.map((item) => (
                <tr key={item.curriculoId} className="hover:bg-slate-800/30">
                  <td className="px-6 py-4 text-slate-300">{item.nome}</td>
                  <td className="px-6 py-4 text-slate-300">{item.nomeArquivo}</td>
                  <td className="px-6 py-4 text-slate-300">{item.versao}</td>
                  <td className="px-6 py-4 text-slate-300">{formatFileSize(item.tamanhoBytes)}</td>
                  <td className="px-6 py-4 text-slate-300">{formatDate(item.dataCriacao)}</td>
                  <td className="px-6 py-4">
                    <div className="flex gap-3">
                      <a href={curriculosApi.download(item.curriculoId)} target="_blank" rel="noreferrer" className="text-emerald-400 hover:text-emerald-300">Baixar</a>
                      <button onClick={() => openEdit(item)} className="text-indigo-400 hover:text-indigo-300">Editar</button>
                      <button onClick={() => setDeleteId(item.curriculoId)} className="text-rose-400 hover:text-rose-300">Excluir</button>
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      <PaginationControls
        pageNumber={pageNumber}
        totalPages={totalPages}
        totalCount={totalCount}
        pageSize={pageSize}
        onPageChange={setPageNumber}
        label="currículo(s)"
      />

      <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title={editingItem ? "Editar Currículo" : "Novo Currículo"}>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="mb-1.5 block text-sm font-medium text-slate-300">Nome</label>
            <input required value={formData.nome} onChange={(event) => setFormData({ ...formData, nome: event.target.value })} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-white" />
          </div>
          <div>
            <label className="mb-1.5 block text-sm font-medium text-slate-300">Versão</label>
            <input required min="1" type="number" value={formData.versao} onChange={(event) => setFormData({ ...formData, versao: event.target.value })} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-white" />
          </div>
          <div>
            <label className="mb-1.5 block text-sm font-medium text-slate-300">Arquivo PDF {editingItem && <span className="font-normal text-slate-500">(opcional)</span>}</label>
            <input required={!editingItem} accept="application/pdf,.pdf" type="file" onChange={handleFileChange} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-slate-300 file:mr-3 file:rounded-md file:border-0 file:bg-slate-800 file:px-3 file:py-1.5 file:text-sm file:text-slate-200" />
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button type="button" onClick={() => setIsModalOpen(false)} className="rounded-lg border border-slate-700 px-4 py-2 text-sm font-medium text-slate-300 hover:bg-slate-800">Cancelar</button>
            <button type="submit" className="rounded-lg bg-indigo-500 px-4 py-2 text-sm font-semibold text-white hover:bg-indigo-400 transition-colors">Salvar</button>
          </div>
        </form>
      </Modal>

      <ConfirmDialog isOpen={!!deleteId} onClose={() => setDeleteId(null)} onConfirm={handleDelete} title="Excluir Currículo" message="Tem certeza que deseja excluir este currículo?" />
    </div>
  );
}

export default CurriculosPage;