import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../Contexts/useAuth";

export function Login() {
  const [formData, setFormData] = useState({ email: "", password: "" });
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError("");
    setIsLoading(true);

    try {
      await login(formData);
      navigate("/dashboard", { replace: true });
    } catch (requestError) {
      setError(requestError.message || "Não foi possível fazer login.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <AuthShell
      title="Acesse sua conta"
      subtitle="Entre para acompanhar seus talentos e oportunidades."
      footer={<>Ainda não tem uma conta? <Link to="/register">Cadastre-se</Link></>}
    >
      {error && <ErrorMessage message={error} />}
      <form onSubmit={handleSubmit} className="space-y-5">
        <Field label="E-mail" id="email">
          <input id="email" type="email" required autoComplete="email" value={formData.email}
            onChange={(event) => setFormData({ ...formData, email: event.target.value })}
            placeholder="voce@exemplo.com" className="auth-input" />
        </Field>
        <Field label="Senha" id="password">
          <div className="relative">
            <input id="password" type={showPassword ? "text" : "password"} required autoComplete="current-password"
              value={formData.password} onChange={(event) => setFormData({ ...formData, password: event.target.value })}
              placeholder="Digite sua senha" className="auth-input pr-12" />
            <button type="button" onClick={() => setShowPassword(!showPassword)} className="auth-password-toggle" aria-label={showPassword ? "Ocultar senha" : "Mostrar senha"}>
              {showPassword ? "Ocultar" : "Mostrar"}
            </button>
          </div>
        </Field>
        <button type="submit" disabled={isLoading} className="auth-submit">
          {isLoading ? "Entrando..." : "Entrar"}
        </button>
      </form>
    </AuthShell>
  );
}

function AuthShell({ title, subtitle, footer, children }) {
  return (
    <main className="auth-page">
      <section className="auth-panel">
        <Link to="/" className="auth-brand"><span>TS</span> TalentSendSync</Link>
        <div className="mb-8 mt-12">
          <p className="auth-eyebrow">Bem-vindo de volta</p>
          <h1 className="auth-title">{title}</h1>
          <p className="auth-subtitle">{subtitle}</p>
        </div>
        {children}
        <p className="auth-footer">{footer}</p>
      </section>
      <div className="auth-aside"><span>Organize seu próximo grande passo.</span><strong>Seu talento,<br />no ritmo certo.</strong></div>
    </main>
  );
}

function Field({ label, id, children }) {
  return <label htmlFor={id} className="auth-label">{label}{children}</label>;
}

function ErrorMessage({ message }) {
  return <div className="auth-error" role="alert">{message}</div>;
}
