import { useEffect, useState, type ChangeEvent, type FormEvent } from "react";
import api from "../../Services/Api";

type Curriculo = {
	curriculoId: string;
	nome: string;
	nomeArquivo: string;
	tamanhoBytes: number;
	versao: number;
	dataCriacao: string;
	ativo: boolean;
};

type PagedResponse = {
	items?: Curriculo[];
	currentPage?: number;
	pageSize?: number;
	totalPages?: number;
	totalCount?: number;
};

const pageSize = 10;

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
	const [pagination, setPagination] = useState({ totalPages: 0, totalCount: 0 });
	const [isLoading, setIsLoading] = useState(true);
	const [error, setError] = useState("");
	const [isModalOpen, setIsModalOpen] = useState(false);
	const [editingItem, setEditingItem] = useState<Curriculo | null>(null);
	const [deleteId, setDeleteId] = useState<string | null>(null);
	const [formData, setFormData] = useState({ nome: "", versao: "1", arquivo: null as File | null });

	const loadItems = async () => {
		setIsLoading(true);
		try {
			const response = await api.get<PagedResponse>("/Curriculos/pagination", {
				params: { pageNumber, pageSize },
			});
			const data = response.data;
			setItems(data.items ?? []);
			setPagination({ totalPages: data.totalPages ?? 0, totalCount: data.totalCount ?? 0 });
		} catch (requestError) {
			setError(getErrorMessage(requestError));
		} finally {
			setIsLoading(false);
		}
	};

	useEffect(() => {
		void loadItems();
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
				await api.put(`/Curriculos/${editingItem.curriculoId}`, data);
			} else {
				if (!formData.arquivo) {
					setError("Selecione um arquivo PDF.");
					return;
				}
				await api.post("/Curriculos", data);
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
			await api.delete(`/Curriculos/${deleteId}`);
			setDeleteId(null);
			await loadItems();
		} catch (requestError) {
			setError(getErrorMessage(requestError));
		}
	};

	return (
		<div className="min-h-screen bg-slate-950 px-6 py-10 text-white">
			<main className="mx-auto max-w-7xl space-y-6">
				<div className="flex items-center justify-between">
					<div>
						<h1 className="text-2xl font-bold">Currículos</h1>
						<p className="mt-1 text-sm text-slate-400">Gerenciar currículos em PDF</p>
					</div>
					<button onClick={openCreate} className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white shadow-md shadow-blue-600/25 hover:bg-blue-500">
						Novo Currículo
					</button>
				</div>

				{error && <div className="rounded-lg border border-red-800 bg-red-950/50 p-3 text-sm text-red-400">{error}</div>}

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
							) : items.map((item) => (
								<tr key={item.curriculoId} className="hover:bg-slate-800/30">
									<td className="px-6 py-4 text-slate-300">{item.nome}</td>
									<td className="px-6 py-4 text-slate-300">{item.nomeArquivo}</td>
									<td className="px-6 py-4 text-slate-300">{item.versao}</td>
									<td className="px-6 py-4 text-slate-300">{formatFileSize(item.tamanhoBytes)}</td>
									<td className="px-6 py-4 text-slate-300">{formatDate(item.dataCriacao)}</td>
									<td className="px-6 py-4"><div className="flex gap-3"><a href={`${api.defaults.baseURL}/Curriculos/${item.curriculoId}/arquivo`} target="_blank" rel="noreferrer" className="text-emerald-400 hover:text-emerald-300">Baixar</a><button onClick={() => openEdit(item)} className="text-blue-400 hover:text-blue-300">Editar</button><button onClick={() => setDeleteId(item.curriculoId)} className="text-red-400 hover:text-red-300">Excluir</button></div></td>
								</tr>
							))}
						</tbody>
					</table>
				</div>

				<div className="flex items-center justify-between text-sm text-slate-400">
					<span>{pagination.totalCount} currículo(s)</span>
					<div className="flex items-center gap-3">
						<button disabled={pageNumber <= 1} onClick={() => setPageNumber((page) => page - 1)} className="rounded-lg border border-slate-700 px-3 py-2 hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-40">Anterior</button>
						<span>Página {pageNumber} de {Math.max(pagination.totalPages, 1)}</span>
						<button disabled={pageNumber >= pagination.totalPages} onClick={() => setPageNumber((page) => page + 1)} className="rounded-lg border border-slate-700 px-3 py-2 hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-40">Próxima</button>
					</div>
				</div>
			</main>

			{isModalOpen && <div className="fixed inset-0 z-10 flex items-center justify-center bg-black/70 p-4"><div className="w-full max-w-lg rounded-xl border border-slate-800 bg-slate-900 p-6"><div className="mb-5 flex items-center justify-between"><h2 className="text-lg font-semibold">{editingItem ? "Editar Currículo" : "Novo Currículo"}</h2><button onClick={() => setIsModalOpen(false)} className="text-2xl leading-none text-slate-400 hover:text-white" aria-label="Fechar">&times;</button></div><form onSubmit={handleSubmit} className="space-y-4"><div><label className="mb-1.5 block text-sm font-medium text-slate-300">Nome</label><input required value={formData.nome} onChange={(event) => setFormData({ ...formData, nome: event.target.value })} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-white" /></div><div><label className="mb-1.5 block text-sm font-medium text-slate-300">Versão</label><input required min="1" type="number" value={formData.versao} onChange={(event) => setFormData({ ...formData, versao: event.target.value })} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-white" /></div><div><label className="mb-1.5 block text-sm font-medium text-slate-300">Arquivo PDF {editingItem && <span className="font-normal text-slate-500">(opcional)</span>}</label><input required={!editingItem} accept="application/pdf,.pdf" type="file" onChange={handleFileChange} className="w-full rounded-lg border border-slate-800 bg-slate-950 px-4 py-2.5 text-sm text-slate-300 file:mr-3 file:rounded-md file:border-0 file:bg-slate-800 file:px-3 file:py-1.5 file:text-sm file:text-slate-200" /></div><div className="flex justify-end gap-3 pt-2"><button type="button" onClick={() => setIsModalOpen(false)} className="rounded-lg border border-slate-700 px-4 py-2 text-sm font-medium text-slate-300 hover:bg-slate-800">Cancelar</button><button type="submit" className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-500">Salvar</button></div></form></div></div>}

			{deleteId && <div className="fixed inset-0 z-20 flex items-center justify-center bg-black/70 p-4"><div className="w-full max-w-md rounded-xl border border-slate-800 bg-slate-900 p-6"><h2 className="text-lg font-semibold">Excluir Currículo</h2><p className="mt-2 text-sm text-slate-400">Tem certeza que deseja excluir este currículo?</p><div className="mt-6 flex justify-end gap-3"><button onClick={() => setDeleteId(null)} className="rounded-lg border border-slate-700 px-4 py-2 text-sm text-slate-300 hover:bg-slate-800">Cancelar</button><button onClick={() => void handleDelete()} className="rounded-lg bg-red-600 px-4 py-2 text-sm font-semibold text-white hover:bg-red-500">Excluir</button></div></div></div>}
		</div>
	);
}

export default CurriculosPage;
