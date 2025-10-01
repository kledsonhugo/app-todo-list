# 🎭 Azure Playwright Workspaces - Guia Completo

Este guia explica como configurar e usar Azure Playwright Workspaces para testes E2E escaláveis em nuvem.

## 🌟 O que é Azure Playwright Workspaces?

Azure Playwright Workspaces é um serviço gerenciado na nuvem que permite executar testes Playwright com:
- ⚡ **20 workers paralelos** na nuvem
- 🌐 **5 configurações de browser** (Chromium, Firefox, WebKit, Mobile Chrome, Mobile Safari)
- 📈 **Escalabilidade automática** sem gerenciar infraestrutura
- 🔍 **Diagnósticos avançados** e relatórios detalhados

## 🚀 Configuração Inicial

### 1. Criando Azure Playwright Workspace

1. Acesse o [Azure Portal](https://portal.azure.com)
2. Procure por "Playwright Testing" ou "Microsoft Playwright Testing"
3. Clique em "Create" para criar um novo workspace
4. Escolha:
   - **Subscription**: Sua assinatura Azure
   - **Resource Group**: Crie um novo ou use existente
   - **Region**: East US (recomendado para melhor performance)
   - **Workspace Name**: Um nome único para seu workspace

### 2. Obtendo Service URL

Após criar o workspace:
1. Vá para o workspace criado no Azure Portal
2. Clique em "Get Started"
3. Copie a **Service URL** no formato:
   ```
   wss://region.api.playwright.microsoft.com/playwrightworkspaces/workspace-id/browsers
   ```

### 3. Configurando Autenticação

Execute o script de configuração OIDC:
```bash
cd scripts
./setup-azure-oidc.sh
```

Este script criará:
- Service Principal com permissões adequadas
- Federated Identity Credentials para GitHub Actions
- Secrets necessários no GitHub

## 🔧 Configuração Local

### 1. Arquivo .env

Crie/edite o arquivo `tests/.env`:
```env
# Azure Playwright Workspaces Configuration
PLAYWRIGHT_SERVICE_URL=wss://eastus.api.playwright.microsoft.com/playwrightworkspaces/SEU-WORKSPACE-ID/browsers

# Workers para desenvolvimento local (opcional)
PLAYWRIGHT_WORKERS=4

# Informações do repositório (automático em CI/CD)
GITHUB_REPOSITORY=app-todo-list
GITHUB_RUN_ID=local-development
```

### 2. Verificando Configuração

Execute o script de verificação:
```bash
cd scripts
./test-azure-config.sh
```

Este script verifica:
- ✅ Variáveis de ambiente
- ✅ Conectividade com Azure
- ✅ Configuração do Playwright
- ✅ Detecção automática de modo (local vs Azure)

## 🧪 Executando Testes

### Testes Locais (Modo Padrão)
```bash
cd tests
npm run test              # Modo local com 3 browsers
npm run test:ui           # Interface visual
npm run test:headed      # Modo headed (visível)
```

### Testes com Azure Workspaces
```bash
cd tests
npm run test:azure        # Azure com 5 browsers e 20 workers
npm run test:azure:ui     # Interface visual com Azure
npm run test:azure:headed # Modo headed com Azure
```

### Script de Teste Automatizado
```bash
cd tests
./test-azure-local.sh azure     # Testar com Azure
./test-azure-local.sh local     # Testar local
./test-azure-local.sh compare   # Comparar ambos
```

## 🏗️ Configuração CI/CD

### GitHub Secrets Necessários

Configure estes secrets no GitHub (criados automaticamente pelo script):
- `AZURE_CLIENT_ID`: ID do Service Principal
- `AZURE_TENANT_ID`: ID do Tenant Azure
- `AZURE_SUBSCRIPTION_ID`: ID da Subscription
- `PLAYWRIGHT_SERVICE_URL`: URL do workspace Azure

### Workflows Disponíveis

1. **azure-playwright-tests.yml**
   - Execução principal com 20 workers
   - Relatórios HTML detalhados
   - Screenshots de falhas
   
2. **azure-multi-browser-tests.yml**
   - Testes matrix por browser
   - Execução agendada diária
   - Comparação de performance

## 📊 Configuração Inteligente

O sistema detecta automaticamente o ambiente:

### Modo Local (Padrão)
- ❌ `CI != true` ou `PLAYWRIGHT_SERVICE_URL` não definida
- 🏠 3 browsers: Chromium, Firefox, WebKit
- ⚡ 4 workers paralelos
- 💻 Execução local

### Modo Azure Workspaces
- ✅ `CI == true` E `PLAYWRIGHT_SERVICE_URL` definida
- 🌐 5 browsers: Chromium, Firefox, WebKit, Mobile Chrome, Mobile Safari
- ⚡ 20 workers paralelos
- ☁️ Execução na nuvem

## 🎯 Configuração Avançada

### playwright.service.config.ts

Configuração principal que:
- 🔍 **Detecta automaticamente** se deve usar Azure
- 🔄 **Fallback inteligente** para modo local se Azure indisponível
- 📝 **Logs detalhados** de configuração
- 🎛️ **Configuração condicional** de browsers e workers

### Exemplo de Log de Configuração

```
🎭 Playwright Configuration:
   Azure Workspaces: ✅ Enabled (cloud mode)
   Service URL: wss://eastus.api.playwright.microsoft.com/...
   Workers: 20
   Browsers: 5 (including mobile)
```

## 🚨 Troubleshooting

### Erro 401 Unauthorized

**Problema**: Tests failing with WebSocket authentication errors

**Soluções**:
1. Verificar se PLAYWRIGHT_SERVICE_URL está correta
2. Confirmar autenticação Azure (execute `az account show`)
3. Verificar permissões do Service Principal
4. Re-executar `scripts/setup-azure-oidc.sh`

### Configuração não carrega

**Problema**: Configuration detection not working

**Soluções**:
1. Verificar arquivo `.env` em `tests/.env`
2. Confirmar variáveis de ambiente: `printenv | grep PLAYWRIGHT`
3. Testar configuração: `scripts/test-azure-config.sh`

### Performance lenta

**Problema**: Tests running slower than expected

**Soluções**:
1. Confirmar que está usando Azure (check logs)
2. Verificar região do workspace (use East US)
3. Aumentar timeout se necessário

## 📈 Benefícios Azure vs Local

| Aspecto | Local | Azure Workspaces |
|---------|-------|------------------|
| **Workers** | 4 | 20 |
| **Browsers** | 3 (desktop) | 5 (desktop + mobile) |
| **Infraestrutura** | Sua máquina | Gerenciada Microsoft |
| **Escalabilidade** | Limitada | Automática |
| **Custo** | Grátis | Pay-per-use |
| **Configuração** | Simples | Requer Azure |

## 🎛️ Comandos Úteis

```bash
# Verificar status da aplicação
curl http://localhost:5146/api/todos

# Testar configuração Azure
CI=true PLAYWRIGHT_SERVICE_URL="sua-url" scripts/test-azure-config.sh

# Ver relatório de testes
cd tests && npm run report

# Executar apenas um browser
npx playwright test --project=chromium

# Debug de testes específicos
npx playwright test --debug tests/e2e/todo-app.spec.js
```

## 📚 Referências

- [Azure Playwright Testing Documentation](https://docs.microsoft.com/azure/playwright-testing/)
- [Playwright Test Configuration](https://playwright.dev/docs/test-configuration)
- [GitHub Actions with Azure](https://docs.github.com/actions/deployment/security-hardening-your-deployments/configuring-openid-connect-in-azure)

## ✅ Checklist de Configuração

- [ ] Azure Playwright Workspace criado
- [ ] Service URL obtida e configurada
- [ ] Service Principal criado (OIDC)
- [ ] GitHub Secrets configurados
- [ ] Arquivo `.env` local configurado
- [ ] Teste de configuração executado com sucesso
- [ ] Primeiro teste Azure executado
- [ ] Workflows GitHub Actions funcionando

---

💡 **Dica**: Use `./test-azure-local.sh compare` para comparar performance entre local e Azure!