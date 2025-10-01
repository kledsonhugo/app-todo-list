# 🔧 Correção: Conflito de Porta no Azure Playwright Workspaces

## ❌ Problema Identificado

```
Error: http://localhost:5146 is already used, make sure that nothing is running on the port/url or set reuseExistingServer:true in config.webServer.
```

## 🔍 Causa do Problema

O workflow do GitHub Actions tem dois pontos onde a aplicação .NET pode ser iniciada:

1. **Step "Start application in background"** - Inicia a app explicitamente
2. **Playwright webServer** - Tenta iniciar a app automaticamente

Isso causava conflito porque ambos tentavam usar a porta 5146.

## ✅ Solução Implementada

### Configuração Corrigida no `playwright.service.config.ts`:

```typescript
webServer: {
  command: 'dotnet run --project ../TodoListApp.csproj',
  url: 'http://localhost:5146',
  reuseExistingServer: true, // ✅ Sempre reutilizar servidor existente
  timeout: 120 * 1000,
  env: {
    ASPNETCORE_ENVIRONMENT: 'Development',
    ASPNETCORE_URLS: 'http://localhost:5146',
  },
}
```

### Antes (Problemático):
```typescript
reuseExistingServer: !process.env.CI, // ❌ No CI, não reutilizava (false)
```

### Depois (Corrigido):
```typescript
reuseExistingServer: true, // ✅ Sempre reutiliza servidor existente
```

## 🔄 Como Funciona Agora

### 1. **Ambiente Local (Desenvolvimento)**
- ✅ `reuseExistingServer: true` - Se a app já estiver rodando, reutiliza
- ✅ Se não estiver rodando, inicia automaticamente

### 2. **Ambiente CI (GitHub Actions)**
- ✅ GitHub Actions inicia a app explicitamente no background
- ✅ Playwright detecta que já está rodando e reutiliza
- ✅ Sem conflitos de porta

## 📋 Fluxo do Workflow Atualizado

```yaml
# 1. GitHub Actions inicia a aplicação
- name: 🚀 Start application in background
  run: |
    dotnet run --project TodoListApp.csproj &
    echo "APP_PID=$!" >> $GITHUB_ENV

# 2. Aguarda aplicação ficar pronta
- name: ⏳ Wait for application to be ready
  run: |
    timeout 90 bash -c 'until curl -f http://localhost:5146/api/todos; do sleep 3; done'

# 3. Playwright reutiliza a aplicação já rodando
- name: 🧪 Run Playwright tests with Azure Playwright Workspaces
  run: npx playwright test --config=playwright.service.config.ts --workers=20
  # ✅ Playwright não tenta iniciar nova instância
```

## 🎯 Benefícios da Correção

### ✅ **Sem Conflitos de Porta**
- Aplicação é iniciada apenas uma vez
- Playwright reutiliza a instância existente

### ✅ **Maior Confiabilidade**
- Elimina race conditions
- Startup mais previsível

### ✅ **Melhor Performance**
- Não há tempo perdido tentando iniciar segunda instância
- Testes começam imediatamente

### ✅ **Funciona em Todos os Ambientes**
- ✅ Local: Inicia automaticamente se necessário
- ✅ CI: Reutiliza instância do workflow
- ✅ Azure Playwright Workspaces: Compatível

## 🧪 Verificação

Após a correção, o workflow deve executar sem erros:

1. ✅ Aplicação inicia no background
2. ✅ Health check confirma que está funcionando  
3. ✅ Playwright detecta servidor existente
4. ✅ Testes executam com 20 workers no Azure
5. ✅ Sem conflitos de porta

## 📚 Configurações Relacionadas

### WebServer no Playwright (Atual):
```typescript
webServer: {
  command: 'dotnet run --project ../TodoListApp.csproj',
  url: 'http://localhost:5146',
  reuseExistingServer: true, // ✅ Sempre reutilizar
  timeout: 120 * 1000,
}
```

### Configuração Azure Playwright:
```typescript
use: {
  baseURL: 'http://localhost:5146', // ✅ URL base para todos os testes
  ...(process.env.PLAYWRIGHT_SERVICE_URL ? {
    connectOptions: {
      wsEndpoint: process.env.PLAYWRIGHT_SERVICE_URL,
    },
  } : {}),
}
```

---

💡 **Resultado**: Agora os testes do Azure Playwright Workspaces devem executar corretamente com 20 workers paralelos em ambiente cloud! 🚀