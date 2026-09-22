import { createBrowserRouter, Navigate } from "react-router-dom";
import { Layout } from "./Components/Layout/Index";
import { CurriculosPage } from "./Pages/Curriculos/Index";
import { CandidaturasPage } from "./Pages/Candidaturas/Index";
import { CandidaturaDetails } from "./Pages/Candidaturas/Details";
import { HistoricoContatoPage } from "./Pages/HistoricoContato/Index";
import { DashboardPage } from "./Pages/Dashboard/Index";

const router = createBrowserRouter([
  {
    path: "/",
    element: <Layout />,
    children: [
      { index: true, element: <Navigate to="/dashboard" replace /> },
      { path: "dashboard", element: <DashboardPage /> },
      { path: "curriculos", element: <CurriculosPage /> },
      { path: "candidaturas", element: <CandidaturasPage /> },
      { path: "candidaturas/:candidaturaId", element: <CandidaturaDetails /> },
      { path: "historico-contato", element: <HistoricoContatoPage /> },
    ],
  },
  { path: "*", element: <Navigate to="/dashboard" replace /> },
]);

export { router };