import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Cell, Legend, Pie, PieChart, ResponsiveContainer, Tooltip, Bar, BarChart, CartesianGrid, XAxis, YAxis } from "recharts";
import { HiArrowRight, HiChartPie, HiCheckCircle, HiClock, HiDocumentText, HiTrendingUp } from "react-icons/hi";
import { dashboardApi } from "../../Services/api";
import type { Dashboard } from "../../Types";
import { getStatusLabel } from "../../Utils/status";

const statusColors: Record<string, string> = {
  Aprovado: "#34d399",
  Rejeitado: "#fb7185",
  "Em Andamento": "#38bdf8",
  "Proposta Recebida": "#fbbf24",
  "Sem retorno": "#94a3b8",
  Enviado: "#818cf8",
  Cancelado: "#64748b",
};

function getErrorMessage(error: unknown) {
  if (error && typeof error === "object" && "message" in error) {
    return String(error.message);
  }
  return "Não foi possível carregar os dados do dashboard.";
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat("pt-BR", { dateStyle: "short" }).format(new Date(value));
}

function formatRate(value: number) {
  return `${new Intl.NumberFormat("pt-BR", { maximumFractionDigits: 2 }).format(value)}%`;
}

function statusClass(status: string) {
  switch (getStatusLabel(status)) {
    case "Aprovado": return "border-emerald-800/50 bg-emerald-900/30 text-emerald-300";
    case "Rejeitado": return "border-rose-800/50 bg-rose-900/30 text-rose-300";
    case "Em Andamento": return "border-sky-800/50 bg-sky-900/30 text-sky-300";
    case "Proposta Recebida": return "border-amber-800/50 bg-amber-900/30 text-amber-300";
    default: return "border-slate-700 bg-slate-800 text-slate-300";
  }
}

export function DashboardPage() {
  const [dashboard, setDashboard] = useState<Dashboard | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    dashboardApi.getCandidaturas()
      .then(setDashboard)
      .catch((requestError: unknown) => setError(getErrorMessage(requestError)))
      .finally(() => setIsLoading(false));
  }, []);

  if (isLoading) return <div className="rounded-2xl border border-slate-800 bg-slate-900 px-4 py-12 text-center text-sm text-slate-400">Carregando dashboard...</div>;
  if (error || !dashboard) return <div className="rounded-2xl border border-rose-900/50 bg-rose-900/20 p-4 text-sm text-rose-300">{error || "Não foi possível carregar o dashboard."}</div>;

  const cards = [
    { label: "Total de candidaturas", value: dashboard.totalCandidaturas, icon: HiDocumentText, tone: "text-indigo-300" },
    { label: "Enviadas", value: dashboard.enviadas, icon: HiTrendingUp, tone: "text-violet-300" },
    { label: "Em andamento", value: dashboard.emAndamento, icon: HiClock, tone: "text-sky-300" },
    { label: "Propostas recebidas", value: dashboard.propostasRecebidas, icon: HiChartPie, tone: "text-amber-300" },
    { label: "Aprovadas", value: dashboard.aprovadas, icon: HiCheckCircle, tone: "text-emerald-300" },
    { label: "Rejeitadas", value: dashboard.rejeitadas, icon: HiDocumentText, tone: "text-rose-300" },
  ];

  return (
    <div className="space-y-6">
      <div>
        <p className="text-xs font-semibold uppercase tracking-[0.18em] text-indigo-400">Visão geral</p>
        <h2 className="mt-1 text-2xl font-bold text-slate-100">Dashboard</h2>
        <p className="mt-1 text-sm text-slate-400">Acompanhe o andamento da sua busca por emprego.</p>
      </div>

      <section className="grid gap-3 sm:grid-cols-2 xl:grid-cols-3">
        {cards.map(({ label, value, icon: Icon, tone }) => (
          <article key={label} className="rounded-2xl border border-slate-800 bg-slate-900 p-4 shadow-lg shadow-black/10">
            <div className="flex items-start justify-between gap-3">
              <p className="text-sm text-slate-400">{label}</p>
              <Icon className={`h-5 w-5 ${tone}`} />
            </div>
            <p className="mt-3 text-3xl font-bold text-slate-100">{value}</p>
          </article>
        ))}
      </section>

      <section className="grid gap-6 xl:grid-cols-[1.15fr_0.85fr]">
        <article className="rounded-2xl border border-slate-800 bg-slate-900 p-5">
          <div className="flex items-center justify-between gap-3">
            <div>
              <h3 className="font-semibold text-slate-100">Candidaturas por status</h3>
              <p className="mt-1 text-xs text-slate-500">Distribuição atual dos processos.</p>
            </div>
            <HiChartPie className="h-5 w-5 text-indigo-300" />
          </div>
          <div className="mt-4 h-72">
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie data={dashboard.status} dataKey="quantidade" nameKey="status" innerRadius="58%" outerRadius="82%" paddingAngle={2}>
                  {dashboard.status.map((item) => <Cell key={item.status} fill={statusColors[item.status] ?? "#64748b"} />)}
                </Pie>
                <Tooltip contentStyle={{ backgroundColor: "#0f172a", borderColor: "#334155", borderRadius: "12px" }} />
                <Legend wrapperStyle={{ color: "#cbd5e1", fontSize: "12px" }} />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </article>

        <article className="rounded-2xl border border-slate-800 bg-slate-900 p-5">
          <h3 className="font-semibold text-slate-100">Indicadores</h3>
          <div className="mt-5 grid gap-4 sm:grid-cols-2 xl:grid-cols-1">
            <div className="border-l-2 border-sky-400 pl-4"><p className="text-sm text-slate-400">Taxa de resposta</p><p className="mt-1 text-3xl font-bold text-slate-100">{formatRate(dashboard.taxaDeResposta)}</p></div>
            <div className="border-l-2 border-emerald-400 pl-4"><p className="text-sm text-slate-400">Taxa de aprovação</p><p className="mt-1 text-3xl font-bold text-slate-100">{formatRate(dashboard.taxaDeAprovacao)}</p></div>
            <div className="border-l-2 border-slate-500 pl-4"><p className="text-sm text-slate-400">Sem retorno</p><p className="mt-1 text-3xl font-bold text-slate-100">{dashboard.semRetorno}</p></div>
          </div>
        </article>
      </section>

      <article className="rounded-2xl border border-slate-800 bg-slate-900 p-5">
        <div className="flex items-center justify-between gap-3">
          <div><h3 className="font-semibold text-slate-100">Evolução das candidaturas</h3><p className="mt-1 text-xs text-slate-500">Candidaturas cadastradas por mês.</p></div>
          <HiTrendingUp className="h-5 w-5 text-indigo-300" />
        </div>
        <div className="mt-4 h-64">
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={dashboard.evolucao} margin={{ top: 8, right: 8, left: -20, bottom: 0 }}>
              <CartesianGrid strokeDasharray="3 3" stroke="#1e293b" />
              <XAxis dataKey="mes" stroke="#94a3b8" fontSize={12} />
              <YAxis allowDecimals={false} stroke="#94a3b8" fontSize={12} />
              <Tooltip contentStyle={{ backgroundColor: "#0f172a", borderColor: "#334155", borderRadius: "12px" }} />
              <Bar dataKey="quantidade" name="Candidaturas" fill="#818cf8" radius={[5, 5, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </article>

      <article className="rounded-2xl border border-slate-800 bg-slate-900 p-5">
        <div className="flex items-center justify-between gap-3"><h3 className="font-semibold text-slate-100">Últimas candidaturas</h3><Link to="/candidaturas" className="inline-flex items-center gap-1 text-sm text-indigo-300 hover:text-indigo-200">Ver todas <HiArrowRight /></Link></div>
        <div className="mt-4 divide-y divide-slate-800">
          {dashboard.ultimasCandidaturas.length === 0 ? <p className="py-6 text-sm text-slate-500">Nenhuma candidatura encontrada.</p> : dashboard.ultimasCandidaturas.map((item) => (
            <Link key={item.candidaturaId} to={`/candidaturas/${item.candidaturaId}`} className="flex flex-col gap-2 py-4 transition hover:bg-slate-800/30 sm:flex-row sm:items-center sm:justify-between sm:px-2">
              <div><p className="font-medium text-slate-100">{item.empresa}</p><p className="mt-1 text-sm text-slate-400">{item.cargo} · {formatDate(item.dataEnvio)}</p></div>
              <span className={`inline-flex w-fit rounded-full border px-2.5 py-1 text-xs font-medium ${statusClass(item.statusNome ?? item.status)}`}>{getStatusLabel(item.statusNome ?? item.status)}</span>
            </Link>
          ))}
        </div>
      </article>
    </div>
  );
}