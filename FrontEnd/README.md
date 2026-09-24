# TalentSendSync Frontend

Frontend do TalentSendSync, desenvolvido com React e Vite para gerenciamento de currículos, candidaturas e histórico de contatos.

## Stack

- React 19
- Vite
- React Router DOM
- Tailwind CSS
- Axios
- Recharts
- React Icons
- ESLint

O gerenciador de pacotes utilizado no projeto é o **Yarn**.

## Pré-requisitos

- Node.js instalado
- Yarn instalado
- API do TalentSendSync disponível em `http://localhost:5079`

## Instalação

Entre na pasta do projeto e instale as dependências:

```bash
cd TalentSendSyncFront
yarn install
```

## Comandos disponíveis

Iniciar o servidor de desenvolvimento com hot reload:

```bash
yarn dev
```

Executar o lint:

```bash
yarn lint
```

Gerar a build de produção:

```bash
yarn build
```

Pré-visualizar a build de produção localmente:

```bash
yarn preview
```

## Configuração da API

O endereço da API está configurado em `src/Services/Api.tsx`:

```ts
baseURL: "http://localhost:5079/api"
```

Para usar outro ambiente, altere o `baseURL` nesse arquivo antes de executar o frontend. O Axios inclui automaticamente o token JWT salvo no `localStorage` no cabeçalho `Authorization` das requisições autenticadas.

## Funcionalidades

- Cadastro e login de usuários
- Proteção de rotas autenticadas
- Dashboard com indicadores de candidaturas
- Listagem paginada de currículos
- Upload, atualização, download e exclusão de currículos
- Criação, edição e exclusão de candidaturas
- Visualização detalhada de candidaturas
- Registro, edição e exclusão do histórico de contatos

## Rotas principais

| Rota | Acesso | Descrição |
| --- | --- | --- |
| `/login` | Público | Login do usuário |
| `/register` | Público | Cadastro de usuário |
| `/dashboard` | Autenticado | Indicadores e visão geral |
| `/curriculos` | Autenticado | Gerenciamento de currículos |
| `/candidaturas` | Autenticado | Listagem de candidaturas |
| `/candidaturas/:candidaturaId` | Autenticado | Detalhes de uma candidatura |
| `/historico-contato` | Autenticado | Histórico de contatos |

Usuários não autenticados são redirecionados para `/login`. A rota `/` redireciona para `/dashboard`.

## Estrutura do projeto

```text
src/
├── Components/       # Layout, sidebar, modais, paginação e proteção de rotas
├── Contexts/         # Estado e regras de autenticação
├── Pages/            # Telas de autenticação e módulos da aplicação
├── Services/         # Cliente Axios e funções de acesso à API
├── Types/            # Tipos TypeScript compartilhados
├── Utils/            # Funções utilitárias e mapeamentos de status
├── App.jsx           # Configuração das rotas
└── main.jsx          # Ponto de entrada da aplicação
```

## Integração com a API

As operações da aplicação são centralizadas em `src/Services/api.ts`, que expõe serviços para:

- `curriculosApi`
- `candidaturasApi`
- `historicoContatoApi`
- `dashboardApi`

O serviço de currículos utiliza `multipart/form-data` para envio de arquivos. As respostas de listagem são normalizadas para suportar paginação no frontend.
