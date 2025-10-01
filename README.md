# 🚀 Todo List API - Full-Stack Application

Uma aplicação .NET 8.0 Web API completa para gerenciamento de lista de tarefas com interface web moderna.

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
│   │   ├── api.spec.js       # Testes da API (8 cenários)
│   │   └── todo-app.spec.js  # Testes da interface (8 cenários)
│   ├── playwright-chromium.config.js  # Config otimizada CI
│   ├── playwright-simple.config.js    # Config local multi-browser
│   ├── package.json        # Dependências e scripts de teste
│   └── package-lock.json   # Lock das dependências
├── .github/workflows/    # CI/CD Pipelines
│   ├── playwright-tests.yml        # Pipeline principal E2E
│   ├── multi-browser-tests.yml     # Pipeline multi-browser
│   └── production-release.yml      # Pipeline de produção
└── Program.cs            # Configuração da aplicação
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

### 🧪 Executar Testes (Opcional)
```bash
# 1. Instalar dependências de teste
cd tests && npm install

# 2. Instalar browsers Playwright
npx playwright install

# 3. Executar testes locais otimizados (Chromium + Firefox)
npx playwright test --config=playwright-simple.config.js

# 4. Executar testes CI (apenas Chromium - mais rápido)
npx playwright test --config=playwright-chromium.config.js

# 5. Visualizar relatório dos testes
npx playwright show-report
```

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
- **Testes E2E** cobrindo toda a aplicação (8 API + 8 UI)
- **API Tests** - Todos os endpoints REST com validação completa

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