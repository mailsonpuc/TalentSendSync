# TalentSendSync BackEnd

API REST para gerenciamento de currículos, candidaturas e histórico de contatos. O projeto utiliza ASP.NET Core, Entity Framework Core com SQL Server, ASP.NET Identity e autenticação por JWT.

## Pré-requisitos

- .NET SDK 10.0 ou superior
- SQL Server acessível pela aplicação
- Entity Framework Core CLI, caso seja necessário criar ou aplicar migrations:

```bash
dotnet tool install --global dotnet-ef
```

## Executando localmente

Na raiz do repositório, restaure as dependências, configure o banco e inicie a API:

```bash
dotnet restore TalentSendSync.slnx
dotnet build TalentSendSync.slnx
dotnet run --project TalentSendSync.API
```

Em ambiente de desenvolvimento, a documentação interativa fica disponível na raiz da aplicação, normalmente em `https://localhost:xxxx/`. A porta exata é exibida no terminal e também está definida em `TalentSendSync.API/Properties/launchSettings.json`.

## Configuração

O arquivo `appsettings.json` contém as chaves esperadas, mas os valores sensíveis devem ser fornecidos por user-secrets, variáveis de ambiente ou outro mecanismo seguro.

Exemplo usando user-secrets:

```bash
dotnet user-secrets init --project TalentSendSync.API

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=TalentSendSync;Trusted_Connection=True;TrustServerCertificate=True" --project TalentSendSync.API

dotnet user-secrets set "Jwt:SecretKey" "chave-local-com-pelo-menos-32-caracteres" --project TalentSendSync.API
```

Principais configurações:

| Chave | Finalidade |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | String de conexão do SQL Server. |
| `Jwt:SecretKey` | Chave usada para assinar os tokens JWT. |
| `Jwt:Issuer` | Emissor esperado pelo token. |
| `Jwt:Audience` | Público esperado pelo token. |
| `Jwt:ExpireMinutes` | Tempo de validade do access token. |
| `Jwt:RefreshTokenValidityInMinutes` | Tempo de validade do refresh token. |

## Banco de dados

As migrations ficam em `TalentSendSync.Infrastructure/Migrations`. Para criar uma nova migration:

```bash
dotnet ef migrations add NomeDaMigration \
   --project TalentSendSync.Infrastructure \
   --startup-project TalentSendSync.API \
   --output-dir Migrations
```

Para aplicar as migrations no banco configurado:

```bash
dotnet ef database update \
   --project TalentSendSync.Infrastructure \
   --startup-project TalentSendSync.API
```

O endpoint `GET /health` verifica a saúde da aplicação e do banco de dados.

## Autenticação

1. Crie uma conta com `POST /api/Auth/register`.
2. Faça login em `POST /api/Auth/login` para obter `accessToken` e `refreshToken`.
3. Envie o access token nas rotas protegidas:

```http
Authorization: Bearer <accessToken>
```

4. Renove a sessão com `POST /api/Auth/refresh-token` quando necessário.

O login possui limitação de requisições. As rotas de criação e atribuição de roles exigem a role `admin`.

## Endpoints principais

Todas as rotas abaixo, exceto as de autenticação e saúde, exigem um token JWT válido.

### Autenticação

| Método | Rota | Descrição |
| --- | --- | --- |
| `POST` | `/api/Auth/register` | Cadastra um usuário. |
| `POST` | `/api/Auth/login` | Autentica e retorna os tokens. |
| `POST` | `/api/Auth/refresh-token` | Gera novos tokens. |
| `POST` | `/api/Auth/CreateRole?roleName=admin` | Cria uma role; requer `admin`. |
| `POST` | `/api/Auth/AddUserToRole?email=...&roleName=...` | Atribui uma role; requer `admin`. |

### Currículos

| Método | Rota | Descrição |
| --- | --- | --- |
| `GET` | `/api/Curriculos/pagination?pageNumber=1&pageSize=10` | Lista currículos paginados. |
| `GET` | `/api/Curriculos/{id}` | Consulta um currículo. |
| `POST` | `/api/Curriculos` | Cria um currículo com `multipart/form-data`; aceita PDF de até 10 MB. |
| `GET` | `/api/Curriculos/{id}/arquivo` | Baixa o arquivo PDF. |
| `PUT` | `/api/Curriculos/{id}` | Atualiza os dados ou substitui o arquivo. |
| `DELETE` | `/api/Curriculos/{id}` | Remove um currículo. |

### Candidaturas

| Método | Rota | Descrição |
| --- | --- | --- |
| `GET` | `/api/Candidaturas/pagination?pageNumber=1&pageSize=10` | Lista candidaturas paginadas. |
| `GET` | `/api/Candidaturas/{id}` | Consulta uma candidatura. |
| `POST` | `/api/Candidaturas` | Cria uma candidatura vinculada a um currículo. |
| `PUT` | `/api/Candidaturas/{id}` | Atualiza uma candidatura. |
| `DELETE` | `/api/Candidaturas/{id}` | Remove uma candidatura. |

### Histórico e dashboard

| Método | Rota | Descrição |
| --- | --- | --- |
| `GET` | `/api/HistoricoContato/pagination?pageNumber=1&pageSize=10` | Lista históricos paginados. |
| `GET` | `/api/HistoricoContato/{id}` | Consulta um histórico. |
| `POST` | `/api/HistoricoContato` | Registra um contato de uma candidatura. |
| `PUT` | `/api/HistoricoContato/{id}` | Atualiza um contato. |
| `DELETE` | `/api/HistoricoContato/{id}` | Remove um contato. |
| `GET` | `/api/Dashboard/Candidaturas` | Retorna os dados do dashboard. |

## Arquitetura

O projeto segue uma organização inspirada em Clean Architecture:

```text
TalentSendSync.API             Entrada HTTP, controllers e configuração da aplicação
TalentSendSync.Application     Casos de uso, serviços, DTOs e contratos
TalentSendSync.Domain          Entidades, enums, regras e interfaces de domínio
TalentSendSync.Infrastructure  EF Core, SQL Server, Identity, repositórios e storage
TalentSendSync.CrossCutting    JWT, CORS, Swagger, rate limiting e injeção de dependência
TalentSendSync.Tests            Testes automatizados das regras de domínio
```

O `UnitOfWork` coordena a persistência das alterações em uma operação única. A injeção de dependência registra as implementações das interfaces e mantém as camadas desacopladas.

### Relacionamentos principais

- Uma empresa pode ter várias candidaturas.
- Um currículo pode ser usado em várias candidaturas.
- Uma candidatura pode ter vários registros de histórico de contato.
- Cada histórico de contato pertence a uma candidatura.

## Testes

Execute todos os testes com:

```bash
dotnet test TalentSendSync.slnx
```

Os testes estão no projeto `TalentSendSync.Tests` e cobrem principalmente as entidades e regras do domínio.
