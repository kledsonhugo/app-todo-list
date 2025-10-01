# 🔐 Configuração de Credenciais Azure para GitHub Actions

Este guia te ajudará a configurar as credenciais necessárias para usar Azure Playwright Workspaces com GitHub Actions.

## 📋 Pré-requisitos

1. ✅ Conta Azure ativa
2. ✅ Azure CLI instalado e configurado
3. ✅ Permissões para criar Service Principals
4. ✅ Azure Playwright Workspace criado

## 🔧 Passo a Passo

### 1. Login no Azure CLI

```bash
# Fazer login no Azure
az login

# Verificar conta ativa
az account show

# (Opcional) Definir subscription específica
az account set --subscription "sua-subscription-id"
```

### 2. Criar Service Principal

#### Opção A: Comando Manual
```bash
# Obter informações da conta
SUBSCRIPTION_ID=$(az account show --query id -o tsv)
TENANT_ID=$(az account show --query tenantId -o tsv)

# Criar Service Principal
az ad sp create-for-rbac \
  --name "GitHub-Actions-PlaywrightWorkspaces" \
  --role contributor \
  --scopes /subscriptions/$SUBSCRIPTION_ID \
  --sdk-auth
```

#### Opção B: Script Automatizado
```bash
# Executar script fornecido
cd scripts
chmod +x setup-azure-credentials.sh
./setup-azure-credentials.sh
```

### 3. Resultado Esperado

O comando retornará um JSON similar a este:

```json
{
  "clientId": "12345678-1234-1234-1234-123456789012",
  "clientSecret": "abcdef123456...",
  "subscriptionId": "87654321-4321-4321-4321-210987654321",
  "tenantId": "11111111-2222-3333-4444-555555555555",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
}
```

### 4. Obter URL do Playwright Workspace

1. Acesse o [Azure Portal](https://portal.azure.com)
2. Navegue para **Azure App Testing** > **Playwright Workspaces**
3. Selecione seu workspace
4. Vá para **Get Started**
5. Na seção **Add region endpoint in your setup**, copie a URL
6. A URL será similar a: `wss://eastus.api.playwright.microsoft.com/playwrightworkspaces/{workspace-id}/browsers`

## 🔑 Configurar GitHub Secrets

### 1. Acessar Configurações do GitHub

1. Vá para seu repositório no GitHub
2. Clique em **Settings** (aba superior)
3. No menu lateral, clique em **Secrets and variables** > **Actions**

### 2. Adicionar Repository Secrets

Clique em **New repository secret** e adicione cada um dos seguintes:

| Secret Name | Value | Descrição |
|-------------|-------|-----------|
| `AZURE_CLIENT_ID` | `{clientId do JSON}` | ID da aplicação do Service Principal |
| `AZURE_TENANT_ID` | `{tenantId do JSON}` | ID do tenant Azure AD |
| `AZURE_SUBSCRIPTION_ID` | `{subscriptionId do JSON}` | ID da subscription Azure |
| `PLAYWRIGHT_SERVICE_URL` | `{URL do workspace}` | Endpoint do Playwright Workspace |

#### Opcional (para compatibilidade):
| Secret Name | Value | Descrição |
|-------------|-------|-----------|
| `AZURE_CREDENTIALS` | `{JSON completo}` | Credenciais completas (método antigo) |

### 3. Exemplo de Configuração

```
AZURE_CLIENT_ID: 12345678-1234-1234-1234-123456789012
AZURE_TENANT_ID: 11111111-2222-3333-4444-555555555555
AZURE_SUBSCRIPTION_ID: 87654321-4321-4321-4321-210987654321
PLAYWRIGHT_SERVICE_URL: wss://eastus.api.playwright.microsoft.com/playwrightworkspaces/ff73001c-d91c-41ba-959c-a26aebaf5a6c/browsers
```

## ✅ Verificar Configuração

### 1. Testar Localmente

```bash
# Verificar autenticação
az account show

# Listar Playwright Workspaces
az resource list --resource-type "Microsoft.App/PlaywrightWorkspaces"
```

### 2. Testar no GitHub Actions

1. Faça push de alguma alteração para triggerar o workflow
2. Verifique se o step `Login to Azure` passa sem erros
3. Monitore os logs para confirmar conexão com Playwright Workspaces

## 🔧 Solução de Problemas

### Erro: "client-id and tenant-id are not supplied"

**Causa**: Secrets não estão configurados corretamente no GitHub

**Solução**:
1. Verifique se todos os secrets foram adicionados
2. Confirme que os nomes estão exatos (case-sensitive)
3. Verifique se não há espaços em branco extras

### Erro: "Authentication failed"

**Causa**: Service Principal não tem permissões suficientes

**Solução**:
```bash
# Verificar role assignments
az role assignment list --assignee {client-id}

# Adicionar permissões se necessário
az role assignment create \
  --assignee {client-id} \
  --role contributor \
  --scope /subscriptions/{subscription-id}
```

### Erro: "Workspace not found"

**Causa**: URL do workspace incorreta ou permissões insuficientes

**Solução**:
1. Verifique se a URL está correta no Azure Portal
2. Confirme que o Service Principal tem acesso ao resource group do workspace
3. Adicione role assignment específico para o workspace:

```bash
az role assignment create \
  --assignee {client-id} \
  --role "Owner" \
  --scope /subscriptions/{subscription-id}/resourceGroups/{resource-group}/providers/Microsoft.App/PlaywrightWorkspaces/{workspace-name}
```

## 🚀 Workflows Prontos

Após configurar os secrets, os seguintes workflows funcionarão automaticamente:

- `.github/workflows/azure-playwright-tests.yml` - Testes rápidos
- `.github/workflows/azure-multi-browser-tests.yml` - Testes completos

## 🔄 Rotação de Credenciais

### Renovar Client Secret

```bash
# Gerar novo secret para o Service Principal
az ad sp credential reset --id {client-id}

# Atualizar AZURE_CREDENTIALS no GitHub com novo JSON
```

### Verificar Expiração

```bash
# Listar credenciais e verificar expiração
az ad sp credential list --id {client-id}
```

## 📞 Suporte

- **Azure Support**: [Portal do Azure > Support](https://portal.azure.com/#blade/Microsoft_Azure_Support/HelpAndSupportBlade)
- **GitHub Actions**: [GitHub Docs](https://docs.github.com/actions)
- **Azure Login Action**: [GitHub - Azure/login](https://github.com/Azure/login)

## 🔗 Links Úteis

- [Criar Service Principal - Documentação](https://docs.microsoft.com/azure/active-directory/develop/howto-create-service-principal-portal)
- [GitHub Actions Azure Authentication](https://docs.microsoft.com/azure/developer/github/connect-from-azure)
- [Azure Playwright Workspaces](https://learn.microsoft.com/azure/app-testing/playwright-workspaces/)