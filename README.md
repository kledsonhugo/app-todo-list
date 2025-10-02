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
├── tests/                # Test Suite Completo
│   ├── e2e/                # Testes End-to-End
│   │   ├── api.spec.js       # Testes da API (8 cenários)
│   │   └── todo-app.spec.js  # Testes da interface (8 cenários)
│   ├── playwright.chromium.config.js    # Config local Chromium apenas (4 workers)
│   ├── playwright.multi.config.js       # Config local multi-browser (4 workers)
│   ├── playwright.azure.chromium.config.ts  # Config Azure Chromium (10 workers)
│   ├── playwright.azure.multi.config.ts     # Config Azure multi-browser (8 workers)
│   ├── package.json        # Scripts npm e dependências
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

# 3. Executar testes locais (Chromium apenas - mais rápido)
npm run test:local:chromium
# ou: npx playwright test --config=playwright.chromium.config.js

# 4. Executar testes locais multi-browser (completo)
npm run test:local:multi  
# ou: npx playwright test --config=playwright.multi.config.js

# 5. Executar testes Azure Playwright (se configurado)
npm run test:azure:chromium   # Azure Chromium apenas
npm run test:azure:multi      # Azure multi-browser

# 6. Visualizar relatório dos testes
npm run report
# ou: npx playwright show-report
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
- **Playwright** - Framework de testes E2E moderno
- **Configurações Flexíveis** - Local (.js) e Azure (.ts)
- **Multi-Browser Support** - Chromium, Firefox, WebKit
- **Parallel Execution** - 4 workers locais, 8-10 no Azure
- **Headless Mode** - Execução otimizada sem interface gráfica
- **Visual Testing** - Screenshots e vídeos de falhas
- **API Testing** - Testes diretos dos endpoints REST
- **Azure Playwright** - Integração com serviço de testes na nuvem

### 🚀 CI/CD (GitHub Actions)
- **3 Pipelines Especializados** - E2E, Multi-browser, Production
- **Matrix Strategy** - Execução paralela por browser
- **Security Scans** - TruffleHog para detecção de secrets
- **Artifact Management** - Relatórios e evidências
- **Performance Optimization** - 50-70% speedup implementado

## 🔄 CI/CD Pipelines

Este projeto possui uma estratégia robusta de CI/CD com 3 pipelines especializados, todos utilizando as configurações locais (.js) para máxima compatibilidade:

### 🧪 Pipeline Principal (E2E Tests)
- **Arquivo**: `.github/workflows/playwright-tests.yml`
- **Trigger**: Push em qualquer branch
- **Configuração**: `playwright.chromium.config.js` (local)
- **Browser**: Chromium apenas (otimizado para velocidade)
- **Workers**: 4 paralelos
- **Modo**: Headless
- **Tempo**: ~1.5 minutos
- **Objetivo**: Feedback rápido para desenvolvimento

### 🌐 Pipeline Multi-Browser
- **Arquivo**: `.github/workflows/multi-browser-tests.yml`  
- **Trigger**: Agendado diário + Execução manual
- **Configuração**: `playwright.multi.config.js` (local)
- **Browsers**: Chromium, Firefox, WebKit
- **Workers**: 4 por browser (execução em matriz paralela)
- **Modo**: Headless
- **Tempo**: ~4-5 minutos
- **Objetivo**: Compatibilidade cross-browser

### 🚀 Pipeline de Produção
- **Arquivo**: `.github/workflows/production-release.yml`
- **Trigger**: Push na main + Tags + Releases
- **Configuração**: `playwright.chromium.config.js` (local)
- **Inclui**: Code quality, API tests, E2E tests, Security scans
- **Workers**: 4 paralelos
- **Modo**: Headless
- **Tempo**: ~4 minutos
- **Objetivo**: Release com qualidade garantida

### 📊 Performance
| Pipeline | Configuração | Workers | Browsers | Tempo Médio |
|----------|-------------|---------|----------|-------------|
| E2E Principal | Local Chromium | 4 | 1 (Chromium) | ~1.5min ⚡ |
| Multi-Browser | Local Multi | 4x3 | 3 (Chrome/Firefox/Safari) | ~4-5min 🚀 |
| Produção | Local Chromium | 4 | 1 (Chromium) | ~4min ✅ |

### 🎯 Características dos Pipelines
- **Sem dependências Azure** - Todos usam configurações locais (.js)
- **Modo headless** - Performance máxima em ambiente CI
- **Paralelização otimizada** - 4 workers para balanceamento ideal
- **Artifacts automáticos** - Relatórios HTML e screenshots de falhas
- **Matrix strategy** - Execução paralela por browser no multi-browser

## ☁️ Azure Playwright Integration

O projeto inclui configurações opcionais para **Azure Playwright Service**, permitindo execução de testes em infraestrutura de nuvem escalável:

### 🔧 Configurações Azure Disponíveis
- **`playwright.azure.chromium.config.ts`** - Chromium na nuvem (10 workers, timeouts otimizados)
- **`playwright.azure.multi.config.ts`** - Multi-browser na nuvem (8 workers, máxima compatibilidade)

### 🚀 Vantagens do Azure Playwright
- **Paralelização massiva** - Até 20 workers simultâneos
- **Infraestrutura gerenciada** - Sem necessidade de manter browsers locais
- **Logs centralizados** - Monitoramento e debugging avançados
- **Escalabilidade automática** - Recursos ajustados conforme demanda

### 📋 Como Usar (Opcional)
```bash
# Executar no Azure Playwright (requer configuração de workspace)
npm run test:azure:chromium    # Chromium na nuvem
npm run test:azure:multi       # Multi-browser na nuvem
```

> **Nota**: As configurações Azure são opcionais. Os workflows de CI/CD usam apenas configurações locais para máxima compatibilidade.

## 🧪 Testes Automatizados

### � Arquitetura de Testes
O projeto implementa uma estratégia de testes híbrida com configurações específicas para diferentes ambientes:

#### **🏠 Configurações Locais (JavaScript)**
- **`playwright.chromium.config.js`** - Chromium apenas, 4 workers, headless
- **`playwright.multi.config.js`** - Multi-browser (Chrome/Firefox/Safari), 4 workers, headless

#### **☁️ Configurações Azure (TypeScript)**  
- **`playwright.azure.chromium.config.ts`** - Azure Chromium, 10 workers, timeouts otimizados
- **`playwright.azure.multi.config.ts`** - Azure multi-browser, 8 workers, máxima compatibilidade

### 📋 Scripts de Teste Disponíveis
```bash
# Testes locais (desenvolvimento)
npm run test:local:chromium    # Chromium apenas (mais rápido)
npm run test:local:multi       # Multi-browser (validação completa)

# Testes Azure Playwright (CI/CD)
npm run test:azure:chromium    # Azure Chromium (CI otimizado)
npm run test:azure:multi       # Azure multi-browser (validação final)

# Utilitários
npm run test:ui               # Modo interface gráfica
npm run test:debug           # Modo debug para desenvolvimento
npm run report              # Visualizar relatórios HTML
```

### 🎯 Cobertura de Testes
- **16 testes de API** - Cobertura completa de todos os endpoints REST
- **16 testes de UI** - Cobertura de todas as interações da interface web
- **Total: 32 testes** executados em paralelo com 4 workers

### ✅ Cenários Testados

#### **📡 API Tests (8 cenários)**
- ✅ Listar todas as tarefas (GET /api/todos)
- ✅ Obter tarefa específica (GET /api/todos/{id})
- ✅ Criar nova tarefa (POST /api/todos)
- ✅ Atualizar tarefa (PUT /api/todos/{id})
- ✅ Alternar status de conclusão (PATCH /api/todos/{id}/toggle)
- ✅ Deletar tarefa (DELETE /api/todos/{id})
- ✅ Tratar erro 404 para tarefa inexistente
- ✅ Validar campos obrigatórios

#### **🖥️ Interface Tests (8 cenários)**
- ✅ Carregamento da página principal
- ✅ Exibição de tarefas padrão
- ✅ Criação de novas tarefas
- ✅ Marcação como concluída/pendente
- ✅ Filtros por status (Todas/Pendentes/Concluídas)
- ✅ Abertura do modal de edição
- ✅ Exclusão de tarefas com confirmação
- ✅ Atualização da lista (refresh)

#### **⚙️ Configurações de Performance**
- **Workers locais**: 4 (balanceamento ideal)
- **Workers Azure**: 8-10 (máxima paralelização)
- **Timeouts**: Otimizados por ambiente (45s local, 90-120s Azure)
- **Retries**: Configurados por contexto (1-3 tentativas)
- **Modo headless**: Ativo para performance máxima

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