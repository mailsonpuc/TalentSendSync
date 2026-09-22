import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { HiArrowLeft, HiExternalLink } from "react-icons/hi";
import { candidaturasApi } from "../../Services/api";
import type { Candidatura, HistoricoContato } from "../../Types";
import { getStatusLabel } from "../../Utils/status";

function getErrorMessage(error: unknown) {
  if (error && typeof error === "object" && "message" in error) {
    return String(error.message);
  }
  return "Não foi possível carregar a candidatura.";
}

function formatDate(value: string, withTime = false) {
  return new Intl.DateTimeFormat("pt-BR", {
    dateStyle: "short",
    ...(withTime ? { timeStyle: "short" } : {}),
  }).format(new Date(value));
}

function formatCurrency(value?: number | null) {
  if (value === null || value === undefined || Number.isNaN(value)) return "Não informado";
  return new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" }).format(value);
}

function eventIcon(tipo: string) {
  switch (tipo) {
    case "Email":
      return "✉";
    case "Whatsapp":
      return "◉";
    case "Link":
      return "↗";
    case "Linkedin":
      return "in";
    default:
      return "•";
  }
}

function HistoryItem({ item }: { item: HistoricoContato }) {
  return (
    <li className="relative pl-10">
      <span className="absolute left-0 top-0 flex h-7 w-7 items-center justify-center rounded-full border border-indigo-400/30 bg-indigo-500/10 text-xs font-semibold text-indigo-300" aria-hidden="true">
        {eventIcon(item.tipoContato)}
      </span>
      <p className="text-xs font-semibold uppercase tracking-[0.12em] text-slate-500">{formatDate(item.dataContato)}</p>
      <p className="mt-1 text-sm font-medium text-slate-100">{item.descricao}</p>
      <p className="mt-1 text-xs text-slate-500">{item.tipoContato}{item.responsavel ? ` · ${item.responsavel}` : ""}</p>
    </li>
  );
}

export function CandidaturaDetails() {
  const { candidaturaId } = useParams<{ candidaturaId: string }>();
  const navigate = useNavigate();
  const [item, setItem] = useState<Candidatura | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!candidaturaId) {
      setError("Candidatura não informada.");
      setIsLoading(false);
      return;
    }

    candidaturasApi.getById(candidaturaId)
      .then(setItem)
      .catch((requestError: unknown) => setError(getErrorMessage(requestError)))
      .finally(() => setIsLoading(false));
  }, [candidaturaId]);

  if (isLoading) return <div className="rounded-xl border border-slate-800 bg-slate-900 px-4 py-8 text-center text-sm text-slate-400">Carregando...</div>;
  if (error || !item) return (
    <div className="space-y-4">
      <button onClick={() => navigate("/candidaturas")} className="inline-flex items-center gap-2 text-sm text-slate-400 hover:text-slate-100"><HiArrowLeft /> Voltar para candidaturas</button>
      <div className="rounded-xl border border-rose-900/50 bg-rose-900/20 p-4 text-sm text-rose-300">{error || "Candidatura não encontrada."}</div>
    </div>
  );

  const history = [...(item.historicosContato ?? [])].sort((first, second) => new Date(first.dataContato).getTime() - new Date(second.dataContato).getTime());

  return (
    <div className="space-y-6">
      <Link to="/candidaturas" className="inline-flex items-center gap-2 text-sm text-slate-400 transition hover:text-slate-100"><HiArrowLeft /> Voltar para candidaturas</Link>

      <section className="rounded-2xl border border-slate-800 bg-slate-900 p-5 sm:p-6">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <div>
            <p className="mb-1 text-[10px] font-semibold uppercase tracking-[0.16em] text-slate-500">Candidatura</p>
            <h2 className="text-2xl font-bold text-slate-100">{item.empresa}</h2>
            <p className="mt-1 text-base text-slate-300">{item.cargo}</p>
          </div>
          <span className="inline-flex w-fit rounded-full border border-amber-800/50 bg-amber-900/30 px-3 py-1 text-xs font-medium text-amber-300">{getStatusLabel(item.statusNome ?? item.status)}</span>
        </div>
      </section>

      <section className="rounded-2xl border border-slate-800 bg-slate-900 p-5 sm:p-6">
        <h3 className="text-lg font-semibold text-slate-100">Dados da candidatura</h3>
        <dl className="mt-4 grid gap-4 text-sm sm:grid-cols-2">
          <div><dt className="text-slate-500">Currículo</dt><dd className="mt-1 text-slate-200">{item.curriculoNome || "Não informado"}</dd></div>
          <div><dt className="text-slate-500">Pretensão salarial</dt><dd className="mt-1 text-slate-200">{formatCurrency(item.pretensaoSalarial)}</dd></div>
          <div><dt className="text-slate-500">Data de envio</dt><dd className="mt-1 text-slate-200">{formatDate(item.dataEnvio ?? item.dataCandidatura, true)}</dd></div>
          <div><dt className="text-slate-500">Vaga</dt><dd className="mt-1">{item.linkVaga ? <a href={item.linkVaga} target="_blank" rel="noreferrer" className="inline-flex max-w-full items-center gap-1 break-all text-indigo-400 underline underline-offset-2 hover:text-indigo-300">Ver vaga <HiExternalLink className="shrink-0" /></a> : <span className="text-slate-200">Não informado</span>}</dd></div>
        </dl>
      </section>

      <section className="rounded-2xl border border-slate-800 bg-slate-900 p-5 sm:p-6">
        <h3 className="text-lg font-semibold text-slate-100">Histórico</h3>
        {history.length === 0 ? <p className="mt-4 text-sm text-slate-500">Nenhum acontecimento registrado.</p> : <ol className="mt-5 space-y-6">{history.map((historyItem) => <HistoryItem key={historyItem.historicoContatoId} item={historyItem} />)}</ol>}
      </section>

      <section className="rounded-2xl border border-slate-800 bg-slate-900 p-5 sm:p-6">
        <h3 className="text-lg font-semibold text-slate-100">Observações</h3>
        <p className="mt-3 whitespace-pre-wrap text-sm leading-6 text-slate-300">{item.observacoes?.trim() || "Sem observações."}</p>
      </section>
    </div>
  );
}

export default CandidaturaDetails;