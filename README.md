# 🚀 Todo List API - Full-Stack Application

Uma aplicação .NET 8.0 Web API completa para gerenciamento de lista de tarefas com interface web moderna, testes automatizados E2E e CI/CD otimizado.

## ✨ Características Principais

- 🎯 **API REST Completa** - ASP.NET Core Web API com Swagger/OpenAPI
- 🌐 **Interface Web Moderna** - HTML5, CSS3, JavaScript ES6+ responsivo
- 🧪 **Testes E2E Automatizados** - Playwright multi-browser com paralelização
- 🚀 **CI/CD Otimizado** - GitHub Actions com 3 pipelines especializados
- ⚡ **Performance Otimizada** - Workers paralelos e configurações por ambiente
- 🔒 **Segurança Integrada** - Scans automáticos e validação de código
- 📊 **Monitoramento Completo** - Relatórios detalhados e artefatos

## 🎯 Funcionalidades

### 🌐 Interface Web
- ✅ **Interface Responsiva** - Design moderno com gradients e animações
- ✅ **CRUD Completo** - Criar, ler, atualizar e deletar tarefas
- ✅ **Filtros Inteligentes** - Todas, Pendentes, Concluídas com contadores
- ✅ **Edição Modal** - Interface elegante para modificar tarefas
- ✅ **Notificações Toast** - Feedback visual em tempo real
- ✅ **Mobile First** - Otimizado para dispositivos móveis

### 🔧 API REST
- ✅ **Endpoints RESTful** - Padrões de API bem definidos
- ✅ **Documentação Swagger** - Interface interativa para testes
- ✅ **Validação de Dados** - DTOs com validação robusta
- ✅ **CORS Configurado** - Acesso cross-origin habilitado
- ✅ **Status Codes Adequados** - Respostas HTTP semânticas

### 🧪 Testes & Qualidade
- ✅ **Testes E2E** - Playwright com cobertura completa da aplicação
- ✅ **Multi-Browser** - Suporte a Chromium, Firefox e WebKit
- ✅ **Paralelização** - 4 workers para execução otimizada
- ✅ **CI/CD Robusto** - 3 pipelines especializados
- ✅ **Security Scans** - Análise automática de vulnerabilidades

## Acesso Rápido

- **🌐 Interface Web:** http://localhost:5146
- **📚 API Docs (Swagger):** http://localhost:5146/api/docs

## 🏗️ Estrutura do Projeto

```
├── Controllers/           # API Controllers
│   └── TodosController.cs    # Endpoints REST da API
├── DTOs/                 # Data Transfer Objects
│   └── TodoItemDtos.cs      # Contratos de entrada/saída
├── Models/               # Domain Models
│   └── TodoItem.cs         # Modelo principal da tarefa
├── Services/             # Business Logic
│   └── TodoService.cs      # Serviço de gerenciamento de tarefas
├── wwwroot/              # Static Web Assets
│   ├── index.html          # Interface web principal
│   ├── styles.css          # Estilos CSS responsivos
│   └── script.js           # JavaScript da aplicação
├── tests/                # Test Suite
│   ├── e2e/                # Testes End-to-End
│   │   ├── api.spec.js       # Testes da API
│   │   └── todo-app.spec.js  # Testes da interface
│   ├── playwright-chromium.config.js  # Config otimizada CI
│   ├── playwright-simple.config.js    # Config multi-browser
│   └── docs/               # Documentação de testes
├── .github/workflows/    # CI/CD Pipelines
│   ├── playwright-tests.yml        # Pipeline principal E2E
│   ├── multi-browser-tests.yml     # Pipeline multi-browser
│   └── production-release.yml      # Pipeline de produção
├── docs/                 # Documentação do projeto
├── Program.cs            # Configuração da aplicação
└── TodoListApp.http      # Exemplos de requisições HTTP
```

## 🚀 Como Executar

### 📋 Pré-requisitos
- **.NET 8.0 SDK** - [Download aqui](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** (opcional - apenas para executar testes)

### ⚡ Execução Rápida
```bash
# 1. Clone o repositório
git clone https://github.com/kledsonhugo/app-todo-list.git
cd app-todo-list

# 2. Restaurar dependências
dotnet restore app-todo-list.sln

# 3. Executar a aplicação
dotnet run --project TodoListApp.csproj
```

### 🌐 Acessos
- **Interface Web:** http://localhost:5146
- **API Swagger:** http://localhost:5146/swagger
- **API Docs:** Disponível via Swagger UI

### 🧪 Executar Testes (Opcional)
```bash
# 1. Instalar dependências de teste
cd tests && npm install

# 2. Instalar browsers Playwright
npx playwright install

# 3. Executar testes (com app rodando)
npx playwright test --config=playwright-chromium.config.js

# 4. Executar testes multi-browser
npx playwright test --config=playwright-simple.config.js
```

### ☁️ Azure Playwright Workspaces
Este projeto também suporta execução de testes com **Azure Playwright Workspaces** para testes em escala na nuvem:

```bash
# Executar testes no Azure Playwright Workspaces
npx playwright test --config=playwright.service.config.ts --workers=20
```

**Benefícios do Azure Playwright Workspaces:**
- ⚡ **20 workers paralelos** na nuvem
- 🌐 **Multi-browser completo** (Chromium, Firefox, WebKit, Mobile)
- 📊 **Relatórios integrados** no Azure Portal
- 🚀 **Infraestrutura escalável** e gerenciada
- 💰 **Execução otimizada** com controle de custos

**Workflows Automatizados:**
- `azure-playwright-tests.yml` - Testes rápidos em Push/PR
- `azure-multi-browser-tests.yml` - Testes completos agendados

📚 **Configuração completa:** [docs/AZURE-PLAYWRIGHT-SETUP.md](docs/AZURE-PLAYWRIGHT-SETUP.md)

## Interface Web

A interface web oferece:

### 🎨 Design Moderno
- Interface limpa e intuitiva
- Gradient de cores atrativo
- Ícones Font Awesome
- Animações suaves

### 📱 Responsivo
- Funciona perfeitamente em desktop, tablet e mobile
- Layout adaptativo
- Botões otimizados para touch

### ⚡ Funcionalidades Interativas
- **Adicionar Tarefas** - Formulário com validação
- **Filtrar Tarefas** - Todas, Pendentes, Concluídas
- **Editar Tarefas** - Modal de edição completo
- **Marcar como Concluída** - Toggle rápido
- **Excluir Tarefas** - Com confirmação
- **Notificações** - Toast messages para feedback
- **Atualização Automática** - Sincronização com a API

### 🔄 Integração com API
- Comunicação assíncrona com a API
- Tratamento de erros
- Loading states
- Validação de dados

## Endpoints da API

### GET /api/todos
Retorna todas as tarefas.

### GET /api/todos/{id}
Retorna uma tarefa específica por ID.

### POST /api/todos
Cria uma nova tarefa.

**Body:**
```json
{
  "title": "Título da tarefa",
  "description": "Descrição opcional"
}
```

### PUT /api/todos/{id}
Atualiza uma tarefa existente.

**Body:**
```json
{
  "title": "Novo título",
  "description": "Nova descrição",
  "isCompleted": true
}
```

### PATCH /api/todos/{id}/toggle
Alterna o status de conclusão de uma tarefa.

### DELETE /api/todos/{id}
Remove uma tarefa.

## Modelo de Dados

### TodoItem
```json
{
  "id": 1,
  "title": "Título da tarefa",
  "description": "Descrição da tarefa",
  "isCompleted": false,
  "createdAt": "2025-09-03T10:30:00Z",
  "completedAt": null,
  "updatedAt": "2025-09-03T10:30:00Z"
}
```

## 🔧 Tecnologias Utilizadas

### 🖥️ Backend (.NET 8.0)
- **ASP.NET Core Web API** - Framework principal
- **Swagger/OpenAPI** - Documentação automática da API
- **CORS** - Configurado para desenvolvimento e produção
- **In-Memory Storage** - Armazenamento simples para demonstração
- **DTOs & Validation** - Contratos bem definidos

### 🎨 Frontend (Vanilla Web)
- **HTML5** - Semântica moderna
- **CSS3** - Flexbox, Grid, Custom Properties, Animations
- **JavaScript ES6+** - Async/Await, Fetch API, Modules
- **Font Awesome** - Ícones profissionais
- **Responsive Design** - Mobile-first approach

### 🧪 Testing & Quality (Playwright)
- **Playwright** - Framework de testes E2E
- **Multi-Browser Support** - Chromium, Firefox, WebKit
- **Parallel Execution** - 4 workers para performance
- **Visual Testing** - Screenshots e videos de falhas
- **API Testing** - Testes diretos dos endpoints

### 🚀 CI/CD (GitHub Actions)
- **3 Pipelines Especializados** - E2E, Multi-browser, Production
- **Matrix Strategy** - Execução paralela por browser
- **Security Scans** - TruffleHog para detecção de secrets
- **Artifact Management** - Relatórios e evidências
- **Performance Optimization** - 50-70% speedup implementado

## 🔄 CI/CD Pipelines

Este projeto possui uma estratégia robusta de CI/CD com 3 pipelines especializados:

### 🧪 Pipeline Principal (E2E Tests)
- **Trigger:** Push em qualquer branch
- **Browser:** Chromium (otimizado para velocidade)
- **Workers:** 4 paralelos
- **Tempo:** ~1.5 minutos
- **Objetivo:** Feedback rápido para desenvolvimento

### 🌐 Pipeline Multi-Browser
- **Trigger:** Agendado diário + Manual
- **Browsers:** Chromium, Firefox, WebKit
- **Workers:** 4 por browser (paralelo)
- **Tempo:** ~4-5 minutos
- **Objetivo:** Compatibilidade cross-browser

### 🚀 Pipeline de Produção
- **Trigger:** Push na main + Tags + Releases
- **Inclui:** Code quality, API tests, E2E tests, Security scans
- **Workers:** 4 paralelos
- **Tempo:** ~4 minutos
- **Objetivo:** Release com qualidade garantida

### 📊 Performance Benchmarks
| Pipeline | Antes | Depois | Speedup |
|----------|-------|--------|---------|
| E2E Principal | ~3min | ~1.5min | 🚀 50% |
| Multi-Browser | ~15min | ~4-5min | 🚀 70% |
| Produção | ~8min | ~4min | 🚀 50% |

## 🧪 Testes Automatizados

### 📋 Cobertura de Testes
- **16 testes E2E** cobrindo toda a aplicação
- **API Tests** - Todos os endpoints REST
- **UI Tests** - Interface web completa
- **Multi-Browser** - Compatibilidade garantida

### 🎯 Cenários Testados
- ✅ Carregamento da página principal
- ✅ Exibição de tarefas padrão
- ✅ Criação de novas tarefas
- ✅ Marcação como concluída
- ✅ Filtros por status
- ✅ Edição de tarefas (modal)
- ✅ Exclusão de tarefas
- ✅ Atualização da lista
- ✅ Validação de API (CRUD completo)
- ✅ Tratamento de erros

### 🔧 Configurações de Teste
- **Local Development:** 50% cores (Chromium) / 25% cores (Multi-browser)
- **CI Environment:** 4 workers fixos para máxima performance
- **Timeouts:** Otimizados por browser (15s-30s)
- **Retry Strategy:** 2 tentativas em CI para confiabilidade

## 📚 Documentação Completa

### �️ Documentos Disponíveis
- **`docs/CI-CD-PIPELINES.md`** - Detalhes completos dos pipelines
- **`docs/PERFORMANCE-OPTIMIZATION.md`** - Otimizações de performance implementadas
- **`docs/MULTI-BROWSER-FIXES.md`** - Correções para compatibilidade multi-browser
- **`docs/GITHUB-ACTIONS-OPTIMIZATION.md`** - Melhorias no GitHub Actions
- **`docs/PLAYWRIGHT-CONFIGS.md`** - Estratégias de configuração dos testes
- **`tests/CI-WORKERS-FIX.md`** - Correção da configuração de workers
- **`INTERFACE_WEB.md`** - Guia detalhado da interface web
- **`TESTE.md`** - Instruções de teste da API

### 🎯 Para Desenvolvedores
```bash
# Executar apenas testes da API
npx playwright test tests/e2e/api.spec.js

# Executar apenas testes da UI
npx playwright test tests/e2e/todo-app.spec.js

# Executar com browser específico
npx playwright test --project=firefox

# Gerar relatório HTML
npx playwright test --reporter=html
```

### � Para DevOps
```bash
# Verificar configuração de workers
PLAYWRIGHT_WORKERS=4 CI=true npx playwright test --dry-run

# Testar pipeline localmente
act -j playwright-tests

# Validar configurações
dotnet format app-todo-list.sln --verify-no-changes
```

## 🔮 Próximos Passos

### 🎯 Roadmap de Funcionalidades
- [ ] **Persistência** - Entity Framework Core + PostgreSQL
- [ ] **Autenticação** - JWT + Identity
- [ ] **Paginação** - Listagem otimizada
- [ ] **Busca Avançada** - Full-text search
- [ ] **Categorias/Tags** - Organização melhorada
- [ ] **Notificações** - Push notifications
- [ ] **PWA** - Modo offline
- [ ] **Docker** - Containerização

### 🚀 Melhorias Técnicas
- [ ] **Cache Redis** - Performance de API
- [ ] **Rate Limiting** - Proteção contra abuse
- [ ] **Health Checks** - Monitoramento
- [ ] **Logging Estruturado** - Observabilidade
- [ ] **Metrics** - Prometheus + Grafana
- [ ] **Load Testing** - K6 + Artillery

### 🧪 Qualidade & Testes
- [ ] **Unit Tests** - xUnit + Moq
- [ ] **Integration Tests** - WebApplicationFactory
- [ ] **Load Tests** - Testes de carga automatizados
- [ ] **Security Tests** - OWASP ZAP integration
- [ ] **Accessibility Tests** - axe-core integration

## 📄 Dados de Exemplo

A aplicação inicia com 3 tarefas de exemplo:
1. **"Estudar .NET"** (pendente)
2. **"Fazer exercícios"** (concluída)  
3. **"Ler documentação"** (pendente)

## 📜 Licença

Este projeto está licenciado sob a MIT License - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 🤝 Contribuições

Contribuições são bem-vindas! Por favor:

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## 👨‍💻 Autor

**Kledson Hugo** - [GitHub](https://github.com/kledsonhugo)

---

⭐ **Se este projeto foi útil, deixe uma estrela!** ⭐