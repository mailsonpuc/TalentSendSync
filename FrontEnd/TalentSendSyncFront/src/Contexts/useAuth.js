import { useContext } from "react";
import { AuthContext, readStoredAuth } from "./authContext";

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth deve ser usado dentro de AuthProvider");
  }
  return context;
}

export function getStoredToken() {
  return readStoredAuth()?.token || null;
}
