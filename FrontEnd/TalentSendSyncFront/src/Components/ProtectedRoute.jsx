import { Navigate } from "react-router-dom";
import { useAuth } from "../Contexts/useAuth";

export function ProtectedRoute({ children }) {
  const { auth } = useAuth();
  return auth ? children : <Navigate to="/login" replace />;
}
