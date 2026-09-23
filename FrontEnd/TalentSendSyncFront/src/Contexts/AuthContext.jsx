import { useState } from "react";
import api from "../Services/Api";
import { AuthContext, STORAGE_KEY, getUserName, readStoredAuth } from "./authContext";

export function AuthProvider({ children }) {
  const [auth, setAuth] = useState(readStoredAuth);

  const login = async ({ email, password }) => {
    const response = await api.post("/Auth/login", { email, password });
    const { accessToken, refreshToken, expiration } = response.data;
    const nextAuth = {
      token: accessToken,
      refreshToken,
      expiration,
      userName: getUserName(accessToken, email),
    };

    localStorage.setItem(STORAGE_KEY, JSON.stringify(nextAuth));
    setAuth(nextAuth);
    return nextAuth;
  };

  const register = async ({ userName, email, password }) => {
    await api.post("/Auth/register", { userName, email, password });
  };

  const logout = () => {
    localStorage.removeItem(STORAGE_KEY);
    setAuth(null);
  };

  return (
    <AuthContext.Provider value={{ auth, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

