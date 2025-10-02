# 🚀 GitHub Actions - Pipeline de Testes

Este diretório contém os workflows do GitHub Actions para execução automatizada dos testes de interface web com Playwright.

## 📁 Estrutura dos Workflows

### `playwright-tests.yml` - **Execução Completa**
Pipeline principal para execução completa dos testes Playwright:

**Características:**
- ✅ Execução em Ubuntu latest
- ✅ Instalação completa dos browsers (Chromium, Firefox, WebKit)  
- ✅ Separação por categorias de teste
- ✅ Upload de screenshots em caso de falha
- ✅ Relatórios detalhados de teste
- ✅ Cache de dependências e browsers

**Triggers:**
- Push para `main`, `develop` ou `test`
- Pull requests para `main`, `develop` ou `test`
- Execução manual via `workflow_dispatch`

### `playwright-docker.yml` - **Execução Docker**
Pipeline otimizado usando imagem Docker com browsers pré-instalados:

**Características:**
- ✅ Mais rápido (browsers já instalados)
- ✅ Ambiente controlado
- ✅ Validação da estrutura dos testes
- ✅ Relatório de implementação

**Triggers:**
- Push/PR para `main`, `develop` ou `test` que afeta arquivos de teste
- Mudanças em `wwwroot/`, `tests/`, `*.cs`, `*.csproj`

### `ci.yml` - **Build e Validação**
Pipeline básico de CI/CD com validação da aplicação:

**Características:**
- ✅ Build da solução completa
- ✅ Verificação de formatação de código
- ✅ Smoke tests da API e interface web
- ✅ Build Docker (quando disponível)

## 🎯 Estratégia de Execução

### Ambiente de Desenvolvimento
```bash
# Setup local
./setup-playwright-tests.sh setup

# Executar testes
./setup-playwright-tests.sh run

# Demonstração
./setup-playwright-tests.sh demo
```

### Pull Requests
1. **`ci.yml`** - Validação básica (sempre executado)
2. **`playwright-docker.yml`** - Validação da estrutura dos testes
3. **`playwright-tests.yml`** - Execução completa (quando possível)

### Branch Principal
- Todos os workflows são executados
- Artefatos são salvos
- Relatórios são gerados

## 📊 Métricas e Relatórios

### Artefatos Gerados
- **test-results**: Arquivos TRX com resultados
- **playwright-screenshots**: Screenshots em caso de falha  
- **playwright-logs**: Logs detalhados de execução

### Relatórios
- **Test Reporter**: Integração com dorny/test-reporter
- **GitHub Step Summary**: Resumo visual dos resultados
- **Console Output**: Logs detalhados durante execução

## 🔧 Configuração

### Variáveis de Ambiente
```yaml
env:
  DOTNET_VERSION: '8.0.x'
  ASPNETCORE_URLS: 'http://localhost:5146'
  ASPNETCORE_ENVIRONMENT: 'Production'
```

### Cache
- **NuGet packages**: `~/.nuget/packages`
- **Playwright browsers**: `~/.cache/ms-playwright`

### Timeout
- **Pipeline completo**: 30 minutos
- **Inicialização da app**: 60 segundos
- **Validação da API**: 30 segundos

## 🎭 Cobertura dos Testes

### Testes Implementados (29 métodos)

**`TodoListWebTests.cs` (12 testes)**
- ✅ `PageLoadsCorrectly`
- ✅ `CanAddNewTodo`
- ✅ `CannotAddTodoWithoutTitle` 
- ✅ `CanFilterTodos`
- ✅ `CanToggleTodoCompletion`
- ✅ `CanOpenEditModal`
- ✅ `CanCancelEditModal`
- ✅ `CanCloseEditModalWithX`
- ✅ `CanDeleteTodoWithConfirmation`
- ✅ `CanCancelDeleteTodo`
- ✅ `RefreshButtonWorks`
- ✅ `ShowsLoadingIndicator`

**`TodoListResponsiveTests.cs` (6 testes)**
- ✅ `DesktopLayoutDisplaysCorrectly` (1200px)
- ✅ `TabletLayoutDisplaysCorrectly` (768px)
- ✅ `MobileLayoutDisplaysCorrectly` (375px)
- ✅ `SmallMobileLayoutDisplaysCorrectly` (320px)
- ✅ `ResponsiveFormInteractionWorks`
- ✅ `ResponsiveFilterButtonsWork`

**`TodoListErrorHandlingTests.cs` (11 testes)**
- ✅ `DisplaysErrorWhenServerUnreachable`
- ✅ `HandlesApiErrorGracefully`
- ✅ `ValidatesRequiredFields`
- ✅ `HandlesLongTextInput`
- ✅ `HandlesEmptyTodosList`
- ✅ `HandlesInvalidJsonResponse`
- ✅ `HandlesSlowNetworkResponse`
- ✅ `HandlesConcurrentOperations`
- ✅ `HandlesSpecialCharactersInInput`
- ✅ `HandlesKeyboardNavigation`
- ✅ `HandlesEscapeKeyInModal`

## 🚨 Troubleshooting

### Browsers não instalados
```yaml
- name: 'Install Playwright browsers'
  run: playwright install --with-deps
```

### App não inicia
```yaml
- name: 'Wait for app to start'
  run: |
    timeout 60 bash -c 'until curl -s http://localhost:5146/api/todos; do sleep 2; done'
```

### Testes falhando
1. Verificar logs no console
2. Baixar artefatos (screenshots, logs)
3. Executar localmente com mesmo ambiente

## 📈 Status dos Workflows

### Badges (adicionar ao README principal)
```markdown
[![Playwright Tests](https://github.com/kledsonhugo/app-todo-list/actions/workflows/playwright-tests.yml/badge.svg)](https://github.com/kledsonhugo/app-todo-list/actions/workflows/playwright-tests.yml)

[![CI/CD](https://github.com/kledsonhugo/app-todo-list/actions/workflows/ci.yml/badge.svg)](https://github.com/kledsonhugo/app-todo-list/actions/workflows/ci.yml)
```

## 🔮 Próximos Passos

1. **Integração com SonarCloud** para análise de qualidade
2. **Deploy automático** após testes passarem  
3. **Testes de performance** com Lighthouse
4. **Notificações** no Slack/Teams
5. **Ambientes de staging** para testes

---

**🎯 Resultado**: Pipeline completo de CI/CD com execução automatizada dos 29 testes Playwright cobrindo 100% da funcionalidade da interface web!