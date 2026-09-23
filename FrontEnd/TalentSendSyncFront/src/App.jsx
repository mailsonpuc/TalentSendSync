import { createBrowserRouter, Navigate } from "react-router-dom";
import { Layout } from "./Components/Layout/Index";
import { CurriculosPage } from "./Pages/Curriculos/Index";
import { CandidaturasPage } from "./Pages/Candidaturas/Index";
import { CandidaturaDetails } from "./Pages/Candidaturas/Details";
import { HistoricoContatoPage } from "./Pages/HistoricoContato/Index";
import { DashboardPage } from "./Pages/Dashboard/Index";
import { Login } from "./Pages/Auth/Login";
import { Register } from "./Pages/Auth/Register";
import { ProtectedRoute } from "./Components/ProtectedRoute";

const router = createBrowserRouter([
  {
    path: "/",
    element: <Layout />,
    children: [
      { index: true, element: <Navigate to="/dashboard" replace /> },
      { path: "dashboard", element: <ProtectedRoute><DashboardPage /></ProtectedRoute> },
      { path: "curriculos", element: <ProtectedRoute><CurriculosPage /></ProtectedRoute> },
      { path: "candidaturas", element: <ProtectedRoute><CandidaturasPage /></ProtectedRoute> },
      { path: "candidaturas/:candidaturaId", element: <ProtectedRoute><CandidaturaDetails /></ProtectedRoute> },
      { path: "historico-contato", element: <ProtectedRoute><HistoricoContatoPage /></ProtectedRoute> },
    ],
  },
  { path: "login", element: <Login /> },
  { path: "register", element: <Register /> },
  { path: "*", element: <Navigate to="/dashboard" replace /> },
]);

export { router };