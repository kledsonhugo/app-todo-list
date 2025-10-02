# Configurações de Teste Playwright

Este projeto possui múltiplas configurações de teste Playwright para diferentes cenários e ambientes, organizadas com nomenclatura consistente.

## 📁 Estrutura de Arquivos

```
tests/
├── playwright.chromium.config.js         # Local - Chromium apenas
├── playwright.multi.config.js            # Local - Multi-browser  
├── playwright.azure.chromium.config.ts   # Azure - Chromium apenas
├── playwright.azure.multi.config.ts      # Azure - Multi-browser
├── package.json                          # Scripts npm
└── README.md                             # Esta documentação
```

## 🏠 Testes Locais

### Chromium Apenas (Rápido)
```bash
npm run test:local:chromium
# ou
npx playwright test --config=playwright.chromium.config.js
```
- **Arquivo**: `playwright.chromium.config.js`
- **Uso**: Desenvolvimento diário, testes rápidos
- **Browser**: Chromium apenas
- **Workers**: 4 localmente
- **Timeout**: 45 segundos

### Multi-Browser (Completo)
```bash
npm run test:local:multi
# ou
npx playwright test --config=playwright.multi.config.js
```
- **Arquivo**: `playwright.multi.config.js`
- **Uso**: Validação completa antes de commits
- **Browsers**: Chromium, Firefox, WebKit
- **Workers**: Configurável
- **Timeout**: Padrão Playwright

## ☁️ Testes Azure Playwright

### Azure Chromium (CI/CD Otimizado)
```bash
npm run test:azure:chromium
# ou
npx playwright test --config=playwright.azure.chromium.config.ts
```
- **Arquivo**: `playwright.azure.chromium.config.ts`
- **Uso**: CI/CD, testes automatizados rápidos
- **Browser**: Chromium apenas
- **Workers**: 10 (configurável via PLAYWRIGHT_WORKERS)
- **Timeout**: 60 segundos
- **Otimizações**: Máxima performance no Azure

### Azure Chromium Ultra-Rápido
```bash
npm run test:azure:chromium:fast
# ou
npx playwright test --config=playwright.azure.chromium.config.ts --workers=20
```
- **Uso**: Validação rápida em ambiente Azure
- **Workers**: 20 (máxima paralelização)

### Azure Multi-Browser (Validação Completa)
```bash
npm run test:azure:multi
# ou
npx playwright test --config=playwright.azure.multi.config.ts
```
- **Arquivo**: `playwright.azure.multi.config.ts`
- **Uso**: Validação completa cross-browser no Azure
- **Browsers**: Chromium, Firefox, WebKit
- **Workers**: 8 (configurável via PLAYWRIGHT_WORKERS)
- **Timeout**: 90 segundos
- **Retries**: 2 tentativas em caso de falha

### Azure Multi-Browser Otimizado
```bash
npm run test:azure:multi:complete
# ou
npx playwright test --config=playwright.azure.multi.config.ts --workers=12
```
- **Uso**: Validação completa otimizada
- **Workers**: 12 (balanceado para multi-browser)

## 📊 Comparação das Configurações

| Configuração | Arquivo | Ambiente | Browsers | Workers | Timeout | Uso Recomendado |
|-------------|---------|----------|----------|---------|---------|-----------------|
| `local:chromium` | `playwright.chromium.config.js` | Local | Chromium | 4 | 45s | Desenvolvimento diário |
| `local:multi` | `playwright.multi.config.js` | Local | Chrome, Firefox, Safari | Variável | 30s | Validação pré-commit |
| `azure:chromium` | `playwright.azure.chromium.config.ts` | Azure | Chromium | 10 | 60s | CI/CD automatizado |
| `azure:chromium:fast` | `playwright.azure.chromium.config.ts` | Azure | Chromium | 20 | 60s | Validação rápida |
| `azure:multi` | `playwright.azure.multi.config.ts` | Azure | Chrome, Firefox, Safari | 8 | 90s | Validação cross-browser |
| `azure:multi:complete` | `playwright.azure.multi.config.ts` | Azure | Chrome, Firefox, Safari | 12 | 90s | Teste completo otimizado |

## 🛠️ Configurações Específicas do Azure

### Otimizações Aplicadas:
- **Timeouts estendidos** para compensar latência de rede
- **Args do Chrome otimizados** para ambiente Azure
- **Configurações de rede** para loopback exposure
- **Credenciais Azure** via DefaultAzureCredential
- **Retry automático** em caso de falhas temporárias

### Variáveis de Ambiente:
- `PLAYWRIGHT_WORKERS`: Número de workers paralelos
- `CI`: Detecta ambiente CI para ajustes automáticos

## 🎯 Estratégia de Uso Recomendada

### Desenvolvimento Local:
1. **Durante desenvolvimento**: `npm run test:local:chromium`
2. **Antes de commit**: `npm run test:local:multi`

### Pipeline CI/CD:
1. **Pull Request**: `npm run test:azure:chromium:fast`
2. **Release**: `npm run test:azure:multi:complete`
3. **Nightly**: `npm run test:azure:multi`

### Debugging:
1. **Local com UI**: `npm run test:ui`
2. **Debug específico**: `npm run test:debug`
3. **Relatórios**: `npm run report`
=======
# Todo List Web Interface Tests

Este projeto contém testes automatizados usando Playwright para validar todas as funcionalidades da interface web da aplicação Todo List.

## Pré-requisitos

1. .NET 8.0 SDK
2. Aplicação Todo List rodando em `http://localhost:5146`
3. Playwright browsers instalados

## Estrutura dos Testes

### `TodoListWebTests.cs`
Testes das funcionalidades principais da interface:
- ✅ Carregamento da página
- ✅ Adicionar nova tarefa
- ✅ Validação de campos obrigatórios
- ✅ Filtros de tarefas (Todas, Pendentes, Concluídas)
- ✅ Toggle de conclusão de tarefas
- ✅ Modal de edição
- ✅ Exclusão de tarefas com confirmação
- ✅ Botão de atualização

### `TodoListResponsiveTests.cs`
Testes de design responsivo:
- 📱 Layout desktop (1200px)
- 📱 Layout tablet (768px)
- 📱 Layout mobile (375px)
- 📱 Layout mobile pequeno (320px)
- 📱 Interação responsiva dos formulários
- 📱 Botões de filtro em diferentes tamanhos

### `TodoListErrorHandlingTests.cs`
Testes de tratamento de erros e casos extremos:
- ❌ Erro de servidor indisponível
- ❌ Erro de API (500)
- ❌ Validação de campos
- ❌ Textos longos
- ❌ Lista vazia
- ❌ JSON inválido
- ❌ Resposta lenta da rede
- ❌ Operações concorrentes
- ❌ Caracteres especiais
- ⌨️ Navegação por teclado
- ⌨️ Tecla Escape no modal

## Como Executar

### 1. Instalar Playwright (primeira vez)
```bash
dotnet tool install --global Microsoft.Playwright.CLI
playwright install
```

### 2. Iniciar a aplicação
```bash
cd /home/runner/work/app-todo-list/app-todo-list
dotnet run --project TodoListApp.csproj
```

### 3. Executar os testes
Em outro terminal:
```bash
cd /home/runner/work/app-todo-list/app-todo-list/tests
dotnet test
```

### Executar testes específicos
```bash
# Apenas testes de funcionalidades principais
dotnet test --filter "TestClass=TodoListWebTests"

# Apenas testes responsivos
dotnet test --filter "TestClass=TodoListResponsiveTests"

# Apenas testes de tratamento de erros
dotnet test --filter "TestClass=TodoListErrorHandlingTests"
```

### Executar com relatório detalhado
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Browsers Suportados

Os testes são executados automaticamente em:
- ✅ Chromium (Chrome/Edge)
- ✅ Firefox
- ✅ WebKit (Safari)

## Funcionalidades Testadas

### Interface Principal
- [x] Carregamento e renderização da página
- [x] Formulário de adição de tarefas
- [x] Lista de tarefas dinâmica
- [x] Botões de filtro (Todas/Pendentes/Concluídas)
- [x] Botões de ação (Completar/Editar/Excluir)
- [x] Modal de edição
- [x] Notificações (toast messages)
- [x] Indicadores de carregamento

### Validações
- [x] Campo título obrigatório
- [x] Limites de caracteres (título: 200, descrição: 500)
- [x] Confirmação de exclusão
- [x] Escape de caracteres especiais

### UX/UI
- [x] Design responsivo
- [x] Navegação por teclado
- [x] Acessibilidade básica
- [x] Feedback visual (loading, success, error)
- [x] Animações e transições

### Integração com API
- [x] Criação de tarefas
- [x] Listagem de tarefas
- [x] Atualização de tarefas
- [x] Exclusão de tarefas
- [x] Toggle de status
- [x] Tratamento de erros da API
- [x] Sincronização automática

## Relatórios de Teste

Os testes geram relatórios automáticos que incluem:
- Resultados por browser
- Screenshots em caso de falha
- Logs detalhados de execução
- Métricas de performance

## Manutenção

### Atualizar Playwright
```bash
playwright install --with-deps
```

### Debug dos testes
```bash
dotnet test --logger "console;verbosity=detailed" --filter "TestMethod=CanAddNewTodo"
```

### Executar com interface gráfica (apenas para desenvolvimento)
Modifique `GlobalSetup.cs` e altere `Headless = false` para ver os testes executando.

## Cobertura de Testes

✅ **100% das funcionalidades** da interface web estão cobertas:

1. **Adicionar Tarefas** - Formulário, validação, integração API
2. **Visualizar Tarefas** - Lista, filtros, estados visuais
3. **Editar Tarefas** - Modal, campos, salvamento
4. **Excluir Tarefas** - Confirmação, integração API
5. **Completar Tarefas** - Toggle, feedback visual
6. **Filtrar Tarefas** - Todos os filtros disponíveis
7. **Atualizar Lista** - Sincronização manual
8. **Responsividade** - Desktop, tablet, mobile
9. **Tratamento de Erros** - Rede, API, validação
10. **Acessibilidade** - Teclado, screen readers

## Contribuição

Para adicionar novos testes:
1. Identifique a funcionalidade a testar
2. Escolha a classe de teste apropriada
3. Siga o padrão AAA (Arrange, Act, Assert)
4. Use seletores CSS robustos
5. Adicione waits apropriados
6. Teste em múltiplos browsers
