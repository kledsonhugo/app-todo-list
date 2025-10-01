# Configuração do Azure Playwright Workspaces

Este documento fornece instruções completas para configurar e usar Azure Playwright Workspaces com este projeto.

## Índice
1. [Visão Geral](#visão-geral)
2. [Pré-requisitos](#pré-requisitos)
3. [Configuração do Azure Playwright Workspace](#configuração-do-azure-playwright-workspace)
4. [Configuração da Autenticação](#configuração-da-autenticação)
5. [Configuração dos GitHub Secrets](#configuração-dos-github-secrets)
6. [Configuração Local](#configuração-local)
7. [Execução dos Testes](#execução-dos-testes)
8. [Monitoramento e Relatórios](#monitoramento-e-relatórios)
9. [Custos e Otimização](#custos-e-otimização)
10. [Solução de Problemas](#solução-de-problemas)

## Visão Geral

Azure Playwright Workspaces é um serviço gerenciado da Microsoft que permite executar testes Playwright em escala na nuvem. Os principais benefícios incluem:

- **Escalabilidade**: Até 20 workers paralelos
- **Multi-browser**: Suporte completo para Chromium, Firefox, WebKit e navegadores móveis
- **Performance**: Execução em infraestrutura otimizada na nuvem
- **Relatórios**: Dashboards integrados no Azure Portal
- **Confiabilidade**: Infraestrutura gerenciada e auto-scaling

## Pré-requisitos

1. **Conta Azure**: Assinatura ativa do Azure
2. **Azure Playwright Workspace**: Workspace criado no Azure App Testing
3. **GitHub Repository**: Repositório com acesso aos GitHub Actions
4. **Permissões**: Acesso para criar Service Principals no Azure AD

## Configuração do Azure Playwright Workspace

### 1. Criar o Workspace

1. Acesse o [Portal do Azure](https://portal.azure.com)
2. Navegue para **Azure App Testing**
3. Clique em **Create** > **Playwright workspace**
4. Preencha as informações:
   - **Resource Group**: Crie um novo ou use existente
   - **Workspace Name**: Nome único para seu workspace
   - **Region**: Escolha a região mais próxima (ex: East US, West Europe)
5. Clique em **Review + Create** e depois **Create**

### 2. Obter o Endpoint do Serviço

1. Acesse seu Playwright workspace no Azure Portal
2. Vá para a página **Get Started**
3. Na seção **Add region endpoint in your setup**, copie a URL do endpoint
4. A URL será similar a: `wss://eastus.api.playwright.microsoft.com/playwrightworkspaces/{workspace-id}/browsers`

## Configuração da Autenticação

### Opção 1: Microsoft Entra ID (Recomendado)

#### Criar Service Principal

Usando Azure CLI:
```bash
# Login no Azure
az login

# Criar service principal
az ad sp create-for-rbac --name "GitHub-Playwright-{repo-name}" --role contributor --scopes /subscriptions/{subscription-id}/resourceGroups/{resource-group}/providers/Microsoft.App/PlaywrightWorkspaces/{workspace-name}

# Ou usar PowerShell
# Connect-AzAccount
# $sp = New-AzADServicePrincipal -DisplayName "GitHub-Playwright-{repo-name}"
```

#### Configurar Federated Identity

1. No Portal do Azure, vá para **Azure Active Directory** > **App registrations**
2. Encontre seu service principal
3. Vá para **Certificates & secrets** > **Federated credentials**
4. Clique em **Add credential**
5. Configure:
   - **Federated credential scenario**: GitHub Actions deploying Azure resources
   - **Organization**: Seu usuário/organização GitHub
   - **Repository**: Nome do repositório
   - **Entity type**: 
     - Para push/PR: Branch
     - Para release: Tag
     - Para workflow manual: Environment (opcional)
   - **GitHub branch name**: main (ou sua branch principal)
6. Clique em **Add**

#### Copiar Informações de Autenticação

Anote as seguintes informações:
- **Client ID** (Application ID)
- **Tenant ID** (Directory ID)
- **Subscription ID**

### Opção 2: Access Token (Não Recomendado)

⚠️ **Aviso**: Access tokens funcionam como senhas de longa duração e são menos seguros.

1. No Azure Portal, vá para seu Playwright workspace
2. Navegue para **Settings** > **Access tokens**
3. Habilite **Access token authentication**
4. Clique em **Generate token**
5. Copie e armazene o token com segurança

## Configuração dos GitHub Secrets

No seu repositório GitHub:

1. Vá para **Settings** > **Secrets and variables** > **Actions**
2. Clique em **New repository secret**
3. Adicione os seguintes secrets:

### Para autenticação Microsoft Entra ID:
```
AZURE_CLIENT_ID: {client-id-do-service-principal}
AZURE_TENANT_ID: {tenant-id-da-sua-organizacao}
AZURE_SUBSCRIPTION_ID: {subscription-id-do-azure}
PLAYWRIGHT_SERVICE_URL: {endpoint-url-do-workspace}
```

### Para autenticação por Access Token:
```
PLAYWRIGHT_SERVICE_ACCESS_TOKEN: {access-token-gerado}
PLAYWRIGHT_SERVICE_URL: {endpoint-url-do-workspace}
```

## Configuração Local

### 1. Instalar Dependências

```bash
cd tests
npm install
```

### 2. Configurar Variáveis de Ambiente

1. Copie o arquivo de exemplo:
```bash
cp .env.example .env
```

2. Edite o arquivo `.env`:
```env
# Azure Playwright Workspaces service endpoint
PLAYWRIGHT_SERVICE_URL=wss://eastus.api.playwright.microsoft.com/playwrightworkspaces/workspace-id/browsers

# Number of parallel workers for local development
PLAYWRIGHT_WORKERS=4
```

### 3. Autenticação Local

Para desenvolvimento local, faça login no Azure CLI:
```bash
az login
```

O Playwright usará automaticamente suas credenciais do Azure CLI quando `PLAYWRIGHT_SERVICE_URL` estiver configurado.

## Execução dos Testes

### GitHub Actions (Automático)

Os testes executam automaticamente quando:

1. **Push/PR para main**: Workflow `azure-playwright-tests.yml`
   - Execução rápida com 20 workers
   - Todos os browsers (Chromium, Firefox, WebKit, Mobile)

2. **Execução agendada (diária)**: Workflow `azure-multi-browser-tests.yml`
   - Teste completo multi-browser
   - Opção de teste único para verificação

3. **Execução manual**: Via GitHub Actions UI
   - Dispatch manual dos workflows
   - Opções de configuração flexíveis

### Desenvolvimento Local

```bash
cd tests

# Executar com Azure Playwright Workspaces
npx playwright test --config=playwright.service.config.ts

# Executar teste específico
npx playwright test todo-crud.spec.js --config=playwright.service.config.ts

# Executar com menos workers para desenvolvimento
PLAYWRIGHT_WORKERS=2 npx playwright test --config=playwright.service.config.ts

# Executar apenas um projeto/browser
npx playwright test --config=playwright.service.config.ts --project=chromium
```

**⚠️ Nota Importante**: A configuração do Azure Playwright automaticamente:
- **Inicia a aplicação .NET** antes dos testes (usando webServer)
- **Para a aplicação** após os testes
- **Configura baseURL** para http://localhost:5146
- **Reutiliza servidor** em desenvolvimento local para performance

### Comandos de Teste

```bash
# Teste único para validação (economiza créditos)
npx playwright test todo-crud.spec.js --config=playwright.service.config.ts --workers=5

# Teste completo (usa mais recursos)
npx playwright test --config=playwright.service.config.ts --workers=20

# Teste com relatório detalhado
npx playwright test --config=playwright.service.config.ts --reporter=html

# Teste em modo debug (local apenas)
npx playwright test --config=playwright.service.config.ts --debug
```

## Monitoramento e Relatórios

### Azure Portal

1. Acesse seu Playwright workspace no Azure Portal
2. Vá para **Test runs** para ver execuções recentes
3. Clique em uma execução para ver:
   - Resultados detalhados por browser
   - Screenshots e traces de falhas
   - Logs de execução
   - Métricas de performance

### GitHub Actions

- **Artifacts**: Reports HTML e screenshots são automaticamente salvos
- **Logs**: Logs detalhados disponíveis em cada workflow
- **Summary**: Resumo visual dos resultados no GitHub

### Relatórios Locais

```bash
# Gerar e abrir relatório HTML
npx playwright show-report

# Visualizar traces de falhas
npx playwright show-trace test-results/trace.zip
```

## Custos e Otimização

### Modelo de Cobrança

- **Cobrança**: Por minuto de teste executado
- **Free Tier**: Minutos gratuitos mensais disponíveis
- **Pricing**: Consulte [Azure Pricing Calculator](https://azure.microsoft.com/pricing/calculator/)

### Estratégias de Otimização

1. **Teste Único Primeiro**:
   ```bash
   # Valide com um teste antes de executar a suíte completa
   npx playwright test todo-crud.spec.js --config=playwright.service.config.ts
   ```

2. **Workers Otimizados**:
   - Desenvolvimento: 2-4 workers
   - CI rápido: 10-15 workers  
   - CI completo: 20 workers

3. **Execução Condicional**:
   - PR: Teste rápido (single test)
   - Merge to main: Teste completo
   - Scheduled: Multi-browser completo

4. **Timeouts Ajustados**:
   ```typescript
   // playwright.service.config.ts
   timeout: 30000, // 30s para ações
   expect: { timeout: 10000 }, // 10s para assertions
   ```

### Monitoramento de Custos

1. Azure Portal > Cost Management
2. Filtre por resource group do Playwright workspace
3. Configure alertas de orçamento
4. Monitore uso mensal

## Solução de Problemas

### Problemas Comuns

#### 1. Erro de Autenticação
```
Error: Failed to connect to Azure Playwright service
```

**Soluções**:
- Verifique se `PLAYWRIGHT_SERVICE_URL` está correto
- Confirme que secrets estão configurados corretamente
- Teste autenticação local: `az account show`

#### 2. Erro de URL Inválida
```
Error: Cannot navigate to invalid URL
```

**Soluções**:
- Verifique se `baseURL` está configurado no playwright.service.config.ts
- Confirme que webServer está iniciando a aplicação corretamente
- Teste se a aplicação responde em http://localhost:5146

#### 3. Timeout na Conexão
```
Error: Timeout waiting for browser to connect
```

**Soluções**:
- Verifique conectividade de rede
- Confirme que o workspace está ativo
- Teste com timeout maior:
  ```typescript
  timeout: 90000, // 90 segundos
  ```

#### 4. Aplicação não inicia
```
Error: webServer command failed
```

**Soluções**:
- Verifique se está no diretório `tests/`
- Confirme que `../TodoListApp.csproj` existe
- Execute `dotnet restore` no diretório raiz
- Verifique se a porta 5146 está disponível

#### 3. Workers Insuficientes
```
Error: Maximum workers exceeded
```

**Soluções**:
- Reduza número de workers: `--workers=10`
- Verifique limites do workspace no Azure Portal
- Solicite aumento de quota se necessário

#### 4. Erro de Configuração
```
Error: Cannot find playwright.service.config.ts
```

**Soluções**:
- Verifique se o arquivo existe em `tests/`
- Confirme sintaxe TypeScript correta
- Execute `npm install` para dependências

### Logs e Debug

#### Habilitar Logs Detalhados
```bash
# Debug local
DEBUG=pw:* npx playwright test --config=playwright.service.config.ts

# Logs do serviço Azure
PLAYWRIGHT_SERVICE_DEBUG=1 npx playwright test --config=playwright.service.config.ts
```

#### GitHub Actions Debug
```yaml
# No workflow YAML
env:
  ACTIONS_STEP_DEBUG: true
  ACTIONS_RUNNER_DEBUG: true
```

### Contato e Suporte

- **Azure Support**: Portal do Azure > Support
- **Playwright Documentation**: [playwright.dev](https://playwright.dev)
- **Azure Playwright Workspaces**: [Documentação oficial](https://learn.microsoft.com/azure/app-testing/playwright-workspaces/)

### Links Úteis

- [Azure Playwright Workspaces Quickstart](https://learn.microsoft.com/azure/app-testing/playwright-workspaces/quickstart-run-end-to-end-tests)
- [GitHub Actions Azure Authentication](https://learn.microsoft.com/azure/developer/github/connect-from-azure)
- [Playwright Best Practices](https://playwright.dev/docs/best-practices)
- [Azure Cost Management](https://docs.microsoft.com/azure/cost-management-billing/)