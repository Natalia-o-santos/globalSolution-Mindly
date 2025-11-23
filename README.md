# Mindly Focus Studio

**Mindly** é uma API ASP.NET Core pensada para organizar sessões de foco e pausas com alertas inteligentes e integração simulada com IoT, seguindo métodos comprovados como Pomodoro para mitigar distração e burnout.

##Integrantes

Natália de Oliveira Santos - RM56030
Alex Ribeiro Maia - RM557356
Felipe Olecsiuc Damasceno - RM559433

## Arquitetura & Domínio

- **Entidade principal**: `FocusSession` representa blocos de foco e descanso. O domínio garante título obrigatório, duração de foco entre 15 e 150 minutos, pausas entre 5 e 45 minutos, e controla os estados `Planned`, `InProgress`, `Paused` e `Completed`.
- **Regras**: a mesma entidade valida invariantes ao criar ou atualizar e oferece comandos (`Start`, `Pause`, `Complete`) que respeitam transições válidas. A flag `IoTIntegrationEnabled` simula o controle de dispositivos inteligentes que sinalizam início/fim de blocos.
- **Serviços**: `FocusSessionService` orquestra casos de uso (CRUD, busca, status), aplica validações do domínio e traduz exceções em `ProblemDetails` quando necessário.

## Infraestrutura & Dados

- **EF Core + SQLite**: `ApplicationDbContext` mapeia `FocusSession` com definições de tamanho e obrigatoriedade. O projeto aplica migrations (veja `Migrations/2025...`), gera o banco `mindly.db` e executa `SeedData` para popular exemplos.
- **Repositório concreto**: `FocusSessionRepository` expõe busca paginada com filtros (`title`, `status`), ordenação flexível e `PagedResult`. Os métodos `Add`, `Update`, `Remove` e `SaveChanges` cobrem o CRUD.

## Camada Web

- Web API com `Controllers`, validações via data annotations e `ProblemDetails` customizado. Todas as rotas aceitam conteúdo JSON e retornam recursos com HATEOAS.
- **Swagger** ativado no ambiente de desenvolvimento (`/swagger`).
- **Endpoints principais** (prefixo `GET/POST/PUT/DELETE /api/focussessions`): CRUD completo com boas práticas e logs.
- **Busca `/api/focussessions/search`** que recebe query params `(title?, status?, page?, pageSize?, sortBy?, descending?)`, aplica paginação, ordenação e filtros. A resposta inclui `PagedResponse<FocusSessionViewModel>` com links HATEOAS (`self`, `update-session`, `delete-session`).

## Execução & scripts

```bash
cd src/Mindly.Api
dotnet ef database update
dotnet run
```

- O `dotnet ef database update` aplica migrations. Use `ConnectionStrings__DefaultConnection` para apontar outro banco (ex.: `export ConnectionStrings__DefaultConnection="Data Source=teste.db"`).
- O app executa `SeedData` automaticamente ao iniciar para garantir exemplos.
- O Swagger fica disponível em `https://localhost:{port}/swagger` e documenta o `search` e rotas CRUD.

## Uso / Exemplo de cURL

- Criar sessão:
  ```bash
  curl -X POST https://localhost:5001/api/focussessions \
    -H "Content-Type: application/json" \
    -d '{"title":"Foco em study assistido","focusMinutes":45,"breakMinutes":10,"description":"Mindly e IoT conectadas","iotIntegrationEnabled":true}'
  ```

- Buscar sessões filtrando status e ordenando pelo foco:
  ```bash
  curl "https://localhost:5001/api/focussessions/search?status=Planned&page=1&pageSize=5&sortBy=focusMinutes&descending=false"
  ```

A resposta do `search` inclui os links HATEOAS para continuar navegando entre as ações possíveis.

## Documentação extra

- O README explica os endpoints, o pipeline de migrations e a lógica de domínio.
- Arquitetura em camadas separadas (`Domain`, `DTOs`, `Repositories`, `Services`, `Controllers`).

## Próximos passos sugeridos

- Criar coleções Postman ou HTTP file para fluxos de foco/pause.
- Estender integração IoT simulada com eventos de webhook ou sinal sonoro.
- Adicionar testes automatizados para regras de transição de status.
