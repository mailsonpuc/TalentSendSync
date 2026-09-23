import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../Contexts/useAuth";

export function Register() {
  const [formData, setFormData] = useState({ userName: "", email: "", password: "", confirmPassword: "" });
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");
  const { register } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError("");
    if (formData.password !== formData.confirmPassword) {
      setError("As senhas não coincidem.");
      return;
    }
    setIsLoading(true);
    try {
      await register(formData);
      navigate("/login", { replace: true, state: { registered: true } });
    } catch (requestError) {
      setError(requestError.message || "Não foi possível criar sua conta.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <main className="auth-page">
      <section className="auth-panel">
        <Link to="/" className="auth-brand"><span>TS</span> TalentSendSync</Link>
        <div className="mb-8 mt-12">
          <p className="auth-eyebrow">Comece agora</p>
          <h1 className="auth-title">Crie sua conta</h1>
          <p className="auth-subtitle">Tenha tudo para gerenciar seus talentos em um só lugar.</p>
        </div>
        {error && <div className="auth-error" role="alert">{error}</div>}
        <form onSubmit={handleSubmit} className="space-y-5">
          <label htmlFor="userName" className="auth-label">Nome de usuário
            <input id="userName" type="text" required minLength={3} maxLength={30} autoComplete="username" value={formData.userName}
              onChange={(event) => setFormData({ ...formData, userName: event.target.value })} placeholder="Como podemos chamar você?" className="auth-input" />
          </label>
          <label htmlFor="email" className="auth-label">E-mail
            <input id="email" type="email" required autoComplete="email" value={formData.email}
              onChange={(event) => setFormData({ ...formData, email: event.target.value })} placeholder="voce@exemplo.com" className="auth-input" />
          </label>
          <label htmlFor="password" className="auth-label">Senha
            <div className="relative">
              <input id="password" type={showPassword ? "text" : "password"} required minLength={6} autoComplete="new-password" value={formData.password}
                onChange={(event) => setFormData({ ...formData, password: event.target.value })} placeholder="Mínimo de 6 caracteres" className="auth-input pr-12" />
              <button type="button" onClick={() => setShowPassword(!showPassword)} className="auth-password-toggle" aria-label={showPassword ? "Ocultar senha" : "Mostrar senha"}>{showPassword ? "Ocultar" : "Mostrar"}</button>
            </div>
          </label>
          <label htmlFor="confirmPassword" className="auth-label">Confirme sua senha
            <input id="confirmPassword" type={showPassword ? "text" : "password"} required autoComplete="new-password" value={formData.confirmPassword}
              onChange={(event) => setFormData({ ...formData, confirmPassword: event.target.value })} placeholder="Repita sua senha" className="auth-input" />
          </label>
          <button type="submit" disabled={isLoading} className="auth-submit">{isLoading ? "Criando conta..." : "Criar conta"}</button>
        </form>
        <p className="auth-footer">Já possui uma conta? <Link to="/login">Entrar</Link></p>
      </section>
      <div className="auth-aside"><span>Uma visão mais clara começa aqui.</span><strong>Menos ruído.<br />Mais movimento.</strong></div>
    </main>
  );
}
