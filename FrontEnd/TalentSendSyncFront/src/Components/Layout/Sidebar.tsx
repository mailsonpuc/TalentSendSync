import { NavLink } from "react-router-dom";
import {
  HiHome,
  HiDocumentText,
  HiUserGroup,
  HiBriefcase,
  HiClipboardList,
  HiSparkles,
  HiChartPie,
} from "react-icons/hi";

const menuItems = [
  { path: "/dashboard", label: "Dashboard", icon: HiChartPie },
  { path: "/curriculos", label: "Currículos", icon: HiDocumentText },
  { path: "/candidaturas", label: "Candidaturas", icon: HiBriefcase },
  { path: "/historico-contato", label: "Histórico de Contato", icon: HiClipboardList },
  { path: "/analise-curriculo", label: "Analisa Curriculo por ia", icon: HiSparkles },
];

export function Sidebar() {
  return (
    <aside className="flex h-full w-64 flex-col border-r border-slate-800 bg-slate-900">
      <div className="flex items-center gap-3 px-6 py-5">
        <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-gradient-to-tr from-indigo-500 to-purple-500 font-bold text-white shadow-lg shadow-indigo-500/30">TS</div>
        <span className="text-xl font-bold text-slate-100">TalentSendSync</span>
      </div>
      <nav className="flex-1 space-y-1 px-3 py-4">
        {menuItems.map((item) => (
          <NavLink
            key={item.path}
            to={item.path}
            className={({ isActive }) =>
              `flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium transition-all duration-200 ${
                isActive
                  ? "bg-indigo-500/15 text-indigo-300"
                  : "text-slate-400 hover:bg-slate-800/50 hover:text-slate-200"
              }`
            }
          >
            <item.icon className="h-5 w-5" />
            {item.label}
          </NavLink>
        ))}
      </nav>
      <div className="border-t border-slate-800 px-6 py-4">
        <p className="text-xs text-slate-500">TalentSendSync v1.0</p>
      </div>
    </aside>
  );
}