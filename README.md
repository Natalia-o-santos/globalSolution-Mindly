# Mindly Focus Studio

**Mindly** é uma API ASP.NET Core pensada para organizar sessões de foco e pausas com alertas inteligentes e integração simulada com IoT, seguindo métodos comprovados como Pomodoro para mitigar distração e burnout.

## Integrantes

- Natália de Oliveira Santos - RM56030
- Alex Ribeiro Maia - RM557356
- Felipe Olecsiuc Damasceno - RM559433

## Visão Geral

A solução permite que trabalhadores, estudantes e freelancers gerenciem seu tempo de foco e pausas de forma inteligente, promovendo bem-estar e equilíbrio entre produtividade e saúde mental. A API oferece CRUD completo de **usuários** e **sessões de foco**, busca avançada com paginação e filtros, e integração HATEOAS para navegação entre recursos. Cada sessão de foco pertence a um usuário, permitindo rastreamento individualizado de produtividade.

## Decisões Arquiteturais

### Arquitetura em Camadas
- **Domain**: Entidades com invariantes e regras de negócio encapsuladas
- **Application**: DTOs/ViewModels e serviços de aplicação (casos de uso)
- **Infrastructure**: EF Core, repositórios concretos e acesso a dados
- **Web**: Controllers RESTful com validações e tratamento de erros

### Padrões Utilizados
- **Repository Pattern**: Abstração de acesso a dados via `IFocusSessionRepository` e `IUserRepository`
- **Service Layer**: `FocusSessionService` e `UserService` orquestram casos de uso e validações
- **DTO Pattern**: Separação entre modelos de domínio e modelos de transporte
- **HATEOAS**: Links de navegação em todas as respostas de recursos
- **Relacionamento 1:N**: Um usuário pode ter várias sessões de foco

## Domínio & Arquitetura 

### Entidades do Negócio e Invariantes

**`User`** (`src/Mindly.Api/Domain/Entities/User.cs`):
- **Nome obrigatório**: Entre 2 e 100 caracteres
- **Email obrigatório**: Formato válido, máximo 255 caracteres, único no sistema
- **Relacionamento**: Um usuário pode ter várias sessões de foco (1:N)

**`FocusSession`** (`src/Mindly.Api/Domain/Entities/FocusSession.cs`):
- **Título obrigatório**: Não pode ser vazio ou nulo
- **Duração de foco**: Entre 15 e 150 minutos (método Pomodoro estendido)
- **Duração de pausa**: Entre 5 e 45 minutos
- **Usuário obrigatório**: Deve pertencer a um usuário existente (`UserId`)
- **Status**: Estados válidos (`Planned`, `InProgress`, `Paused`, `Completed`)
- **Transições de status**: Validadas nos métodos `Start()`, `Pause()`, `Complete()`

### Regras de Negócio

**Na Entidade**:
- Validação de duração ao criar/atualizar (`UpdateDetails`)
- Controle de transições de estado (ex: não pode completar uma sessão que não foi iniciada)
- Timestamps automáticos (`CreatedAt`, `UpdatedAt`)

**Nos Serviços**:
- **`UserService`**: Validação de email único, verificação de existência antes de atualizar/deletar
- **`FocusSessionService`**: Validação de existência antes de atualizar/deletar, aplicação de regras de domínio
- Tratamento de exceções de domínio (`DomainValidationException`) em ambos os serviços

##  Aplicação 
### Serviços de Aplicação (Casos de Uso)

**`IUserService`** implementa:
- `CreateAsync`: Criar novo usuário (valida email único)
- `GetByIdAsync`: Buscar usuário por ID
- `UpdateAsync`: Atualizar usuário existente (valida email único)
- `DeleteAsync`: Remover usuário (cascade remove sessões)
- `SearchAsync`: Busca paginada com filtros (nome, email) e ordenação

**`IFocusSessionService`** implementa:
- `CreateAsync`: Criar nova sessão de foco (requer `UserId`)
- `GetByIdAsync`: Buscar sessão por ID
- `UpdateAsync`: Atualizar sessão existente
- `DeleteAsync`: Remover sessão
- `SearchAsync`: Busca paginada com filtros e ordenação

### DTOs/ViewModels para IO entre Camadas

**User**:
- **`UserCreateDto`**: Input para criação (validações `[Required]`, `[EmailAddress]`, `[StringLength]`)
- **`UserUpdateDto`**: Input para atualização
- **`UserQueryParameters`**: Parâmetros de busca (filtros: nome, email; paginação, ordenação)
- **`UserViewModel`**: Output com dados do usuário + links HATEOAS

**FocusSession**:
- **`FocusSessionCreateDto`**: Input para criação (com validações `[Required]`, `[Range]`, `[StringLength]`, inclui `UserId`)
- **`FocusSessionUpdateDto`**: Input para atualização
- **`FocusSessionQueryParameters`**: Parâmetros de busca (filtros, paginação, ordenação)
- **`FocusSessionViewModel`**: Output com dados da sessão + links HATEOAS (inclui `UserId`)

**Comum**:
- **`PagedResponse<T>`**: Resposta paginada com metadados

### Tratamento de Erros

- **`ProblemDetails`**: Respostas padronizadas para erros HTTP
- **`DomainValidationException`**: Exceções de domínio traduzidas para `400 Bad Request`
- **Validações automáticas**: Data Annotations nos DTOs geram `400` com detalhes
- **Middleware de exceções**: `UseExceptionHandler` captura e formata erros

## Infra & Dados 

### EF Core: Mapeamentos de Entidades

**`ApplicationDbContext`** (`src/Mindly.Api/Data/ApplicationDbContext.cs`):
- Configuração de `FocusSession` com `HasMaxLength`, `IsRequired`
- Enum `FocusSessionStatus` mapeado como string
- Timestamps configurados automaticamente

### Repositórios Concretos (CRUD)

**`UserRepository`** implementa `IUserRepository`:
- `AddAsync`: Adicionar novo usuário
- `Update`: Marcar usuário para atualização
- `Remove`: Marcar usuário para remoção
- `GetByIdAsync`: Buscar por ID
- `GetByEmailAsync`: Buscar por email (validação de unicidade)
- `SearchAsync`: Busca com filtros (`name`, `email`), ordenação (`sortBy`, `descending`) e paginação (`page`, `pageSize`)
- `SaveChangesAsync`: Persistir alterações

**`FocusSessionRepository`** implementa `IFocusSessionRepository`:
- `AddAsync`: Adicionar nova entidade
- `Update`: Marcar entidade para atualização
- `Remove`: Marcar entidade para remoção
- `GetByIdAsync`: Buscar por ID
- `SearchAsync`: Busca com filtros (`title`, `status`), ordenação (`sortBy`, `descending`) e paginação (`page`, `pageSize`)
- `SaveChangesAsync`: Persistir alterações

### Migrations Aplicadas

- **Migration**: `Migrations/20251121134456_InitialCreate.cs`
- **Aplicação automática**: `context.Database.Migrate()` no `Program.cs`
- **Seed automático**: `SeedData.EnsureSeeded(context)` popula dados de exemplo

## Camada Web - Web API 

### CRUD/Controllers com Boas Práticas 

**`UsersController`** (`src/Mindly.Api/Controllers/UsersController.cs`):
- ✅ `[ApiController]` e `[Route("api/[controller]")]`
- ✅ Injeção de dependência (`IUserService`, `LinkGenerator`, `ILogger`)
- ✅ Logging de operações importantes
- ✅ Códigos HTTP apropriados (`201 Created`, `204 NoContent`, `404 NotFound`)
- ✅ `CreatedAtAction` para recursos criados

**Endpoints CRUD - Users**:
- `GET /api/users/search`: Busca paginada com filtros
- `GET /api/users/{id}`: Buscar por ID
- `POST /api/users`: Criar novo usuário
- `PUT /api/users/{id}`: Atualizar usuário
- `DELETE /api/users/{id}`: Remover usuário (cascade remove sessões)

**`FocusSessionsController`** (`src/Mindly.Api/Controllers/FocusSessionsController.cs`):
- ✅ `[ApiController]` e `[Route("api/[controller]")]`
- ✅ Injeção de dependência (`IFocusSessionService`, `LinkGenerator`, `ILogger`)
- ✅ Logging de operações importantes
- ✅ Códigos HTTP apropriados (`201 Created`, `204 NoContent`, `404 NotFound`)
- ✅ `CreatedAtAction` para recursos criados

**Endpoints CRUD - FocusSessions**:
- `GET /api/focussessions/search`: Busca paginada com filtros
- `GET /api/focussessions/{id}`: Buscar por ID
- `POST /api/focussessions`: Criar nova sessão (requer `UserId`)
- `PUT /api/focussessions/{id}`: Atualizar sessão
- `DELETE /api/focussessions/{id}`: Remover sessão

### `/search` com Paginação + Ordenação + Filtros (15 pts)

**`GET /api/users/search`**:

**Query Parameters**:
- `name?` (string): Filtrar por nome (busca parcial, case-insensitive)
- `email?` (string): Filtrar por email (busca parcial, case-insensitive)
- `page` (int, padrão: 1): Número da página
- `pageSize` (int, padrão: 10, min: 5, max: 100): Itens por página
- `sortBy` (string, padrão: "createdAt"): Campo para ordenação (`name`, `email`, `createdAt`)
- `descending` (bool, padrão: true): Ordem decrescente

**Resposta**: `PagedResponse<UserViewModel>` com:
- `Page`, `PageSize`, `Total`, `TotalPages`
- `Items`: Array de `UserViewModel` (cada um com links HATEOAS)

**`GET /api/focussessions/search`**:

**Query Parameters**:
- `title?` (string): Filtrar por título (busca parcial, case-insensitive)
- `status?` (FocusSessionStatus): Filtrar por status
- `page` (int, padrão: 1): Número da página
- `pageSize` (int, padrão: 10, min: 5, max: 100): Itens por página
- `sortBy` (string, padrão: "createdAt"): Campo para ordenação (`focusMinutes`, `breakMinutes`, `status`, `createdAt`)
- `descending` (bool, padrão: true): Ordem decrescente

**Resposta**: `PagedResponse<FocusSessionViewModel>` com:
- `Page`, `PageSize`, `Total`, `TotalPages`
- `Items`: Array de `FocusSessionViewModel` (cada um com links HATEOAS)

### HATEOAS

**Links em cada `UserViewModel`**:
- `self`: GET do recurso
- `update-user`: PUT do recurso
- `delete-user`: DELETE do recurso
- `focus-sessions`: GET para buscar sessões do usuário

**Links em cada `FocusSessionViewModel`**:
- `self`: GET do recurso
- `update-session`: PUT do recurso
- `delete-session`: DELETE do recurso

**Implementação**: `LinkGenerator` gera URIs dinâmicas baseadas nas rotas do controller.


## Como Rodar

### Pré-requisitos
- .NET 8.0 SDK
- EF Core Tools: `dotnet tool install --global dotnet-ef` (se ainda não tiver)

### Passo a Passo

1. **Clone o repositório**:
   ```bash
   git clone https://github.com/Natalia-o-santos/globalSolution-Mindly.git
   cd globalSolution-Mindly
   ```

2. **Aplicar Migrations e Seed**:
```bash
cd src/Mindly.Api
dotnet ef database update
   ```
   
   **Nota**: As migrations são aplicadas automaticamente ao iniciar a aplicação (`Program.cs`), mas você pode executar manualmente se preferir.

3. **Executar a API**:
   ```bash
dotnet run
```

   Ou use o perfil do Rider/VSCode: **"Mindly.Api: http"**

4. **Acessar Swagger**:
   - Abra `https://localhost:5001/swagger` (ou `http://localhost:5000/swagger`)
   - A raiz (`/`) redireciona automaticamente para o Swagger

### Variáveis de Ambiente

**Connection String** (opcional, padrão: `Data Source=mindly.db`):
```bash
export ConnectionStrings__DefaultConnection="Data Source=meu_banco.db"
```

**Ambiente** (padrão: `Development`):
```bash
export ASPNETCORE_ENVIRONMENT="Production"
```

**URLs** (configuradas em `launchSettings.json`):
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

##  Rotas/Endpoints

### Base URL
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`

### Endpoints Disponíveis

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/users/search` | Busca paginada de usuários com filtros e ordenação |
| `GET` | `/api/users/{id}` | Buscar usuário por ID |
| `POST` | `/api/users` | Criar novo usuário |
| `PUT` | `/api/users/{id}` | Atualizar usuário |
| `DELETE` | `/api/users/{id}` | Remover usuário (cascade remove sessões) |
| `GET` | `/api/focussessions/search` | Busca paginada de sessões com filtros e ordenação |
| `GET` | `/api/focussessions/{id}` | Buscar sessão por ID |
| `POST` | `/api/focussessions` | Criar nova sessão (requer `UserId`) |
| `PUT` | `/api/focussessions/{id}` | Atualizar sessão |
| `DELETE` | `/api/focussessions/{id}` | Remover sessão |
| `GET` | `/swagger` | Documentação interativa (Swagger UI) |
| `GET` | `/` | Redireciona para `/swagger` |

## 💡 Exemplos de Uso

### cURL

**1. Criar Usuário**:
```bash
curl -X POST https://localhost:5001/api/users \
  -H "Content-Type: application/json" \
  -k \
  -d '{
    "name": "João Silva",
    "email": "joao.silva@mindly.com"
  }'
```

**2. Buscar Usuários (com filtros e paginação)**:
```bash
curl -k "https://localhost:5001/api/users/search?name=João&page=1&pageSize=10&sortBy=name&descending=false"
```

**3. Criar Sessão de Foco** (use o `userId` retornado na criação do usuário):
```bash
curl -X POST https://localhost:5001/api/focussessions \
  -H "Content-Type: application/json" \
  -k \
  -d '{
    "title": "Foco em estudo assistido",
    "focusMinutes": 45,
    "breakMinutes": 10,
    "userId": "00000000-0000-0000-0000-000000000000",
    "description": "Mindly e IoT conectadas",
    "iotIntegrationEnabled": true
  }'
```

**4. Buscar Sessões (com filtros e paginação)**:
```bash
curl -k "https://localhost:5001/api/focussessions/search?status=Planned&page=1&pageSize=5&sortBy=focusMinutes&descending=false"
```

**5. Buscar Sessão por ID**:
```bash
curl -k "https://localhost:5001/api/focussessions/{id}"
```

**6. Atualizar Sessão**:
```bash
curl -X PUT https://localhost:5001/api/focussessions/{id} \
  -H "Content-Type: application/json" \
  -k \
  -d '{
    "title": "Sessão atualizada",
    "focusMinutes": 50,
    "breakMinutes": 15,
    "description": "Nova descrição"
  }'
```

**7. Deletar Sessão**:
```bash
curl -X DELETE https://localhost:5001/api/focussessions/{id} -k
```

### Swagger UI

1. Acesse `https://localhost:5001/swagger`
2. Expanda os endpoints desejados
3. Clique em "Try it out"
4. Preencha os parâmetros e clique em "Execute"
5. Veja a resposta com links HATEOAS

### HTTP File

O projeto inclui `src/Mindly.Api/Mindly.Api.http` com exemplos de requisições prontas para usar no Visual Studio/Rider.

##  Estrutura do Projeto

```
src/Mindly.Api/
├── Controllers/          # Controllers RESTful (UsersController, FocusSessionsController)
├── Data/                # DbContext e Seed
├── Domain/              # Entidades, Enums, Exceções
│   ├── Entities/        # User, FocusSession
│   ├── Enums/           # FocusSessionStatus
│   └── Exceptions/      # DomainValidationException
├── DTOs/                # DTOs e ViewModels
│   └── Application/     # UserCreateDto, UserUpdateDto, UserViewModel, UserQueryParameters
│                        # FocusSessionCreateDto, FocusSessionUpdateDto, FocusSessionViewModel, etc.
├── Migrations/          # Migrations do EF Core
├── Repositories/        # Repositórios concretos (UserRepository, FocusSessionRepository)
├── Services/            # Serviços de aplicação (UserService, FocusSessionService)
├── Properties/          # launchSettings.json
├── Program.cs           # Configuração da aplicação
└── appsettings.json     # Configurações
```

## ✅ Checklist de Requisitos

- ✅ **Domínio & Arquitetura**: Entidades com invariantes, regras de negócio
- ✅ **Aplicação**: Serviços claros, DTOs/ViewModels, ProblemDetails
- ✅ **Infra & Dados**: EF Core, repositórios CRUD, migrations aplicadas
- ✅ **Camada Web**: CRUD com boas práticas, `/search` completo, HATEOAS
- ✅ **Documentação**: README completo com visão geral, arquitetura, execução, rotas, exemplos
- ✅ **Swagger**: Documentação interativa disponível
- ✅ **Validações**: Data Annotations + ProblemDetails
- ✅ **Seed**: Dados de exemplo populados automaticamente

## 🔗 Links Úteis

- **Repositório**: https://github.com/Natalia-o-santos/globalSolution-Mindly
- **Swagger**: https://localhost:5001/swagger (após iniciar a aplicação)
