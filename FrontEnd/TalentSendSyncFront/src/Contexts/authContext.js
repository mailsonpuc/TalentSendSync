import { createContext } from "react";

export const AuthContext = createContext(null);
export const STORAGE_KEY = "talent-send-sync-auth";

export function readStoredAuth() {
  try {
    return JSON.parse(localStorage.getItem(STORAGE_KEY) || "null");
  } catch {
    return null;
  }
}

export function getUserName(token, fallback) {
  try {
    const payload = JSON.parse(atob(token.split(".")[1]));
    return payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || payload.name || payload.unique_name || fallback;
  } catch {
    return fallback;
  }
}
