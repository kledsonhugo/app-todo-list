# 🔐 Correção do Erro de Autenticação OIDC no Azure

## ❌ Problema Identificado

O erro `AADSTS70025` indica que o Service Principal criado **não possui credenciais de identidade federada configuradas**, que são necessárias para autenticação OIDC do GitHub Actions.

```
Error: AADSTS70025: The client '***'(GitHub-Actions-PlaywrightWorkspaces-20251001) has no configured federated identity credentials.
```

## 🔍 Causa do Problema

Existem dois métodos de autenticação Azure para GitHub Actions:

### 1. **Autenticação por Chave/Segredo** (método antigo)
- Usa `clientSecret` no workflow
- Armazena credenciais sensíveis como secrets
- Menos seguro

### 2. **Autenticação OIDC** (método recomendado) ⭐
- Usa identidade federada sem segredos
- Mais seguro e recomendado pela Microsoft
- **Requer configuração de credenciais federadas**

## ✅ Solução

O workflow atual está configurado para OIDC, mas o Service Principal foi criado sem credenciais federadas.

### Opção 1: Executar Script Automatizado (Recomendado)

```bash
cd /home/kledsonbasso/source/app-todo-list
./scripts/setup-azure-oidc.sh
```

Este script:
1. ✅ Cria um novo Service Principal 
2. ✅ Configura credenciais de identidade federada para OIDC
3. ✅ Configura para branch `main` e pull requests
4. ✅ Fornece os secrets corretos para o GitHub

### Opção 2: Configuração Manual

#### Passo 1: Configurar Credenciais Federadas

Para o Service Principal existente, execute:

```bash
# Substitua pelos valores do seu repositório
GITHUB_OWNER="kledsonhugo"
GITHUB_REPO="app-todo-list"
CLIENT_ID="seu-client-id-atual"

# Configurar para branch main
az ad app federated-credential create \
    --id $CLIENT_ID \
    --parameters '{
        "name": "main-branch-credential",
        "issuer": "https://token.actions.githubusercontent.com", 
        "subject": "repo:'$GITHUB_OWNER'/'$GITHUB_REPO':ref:refs/heads/main",
        "description": "GitHub Actions - main branch",
        "audiences": ["api://AzureADTokenExchange"]
    }'

# Configurar para pull requests
az ad app federated-credential create \
    --id $CLIENT_ID \
    --parameters '{
        "name": "pull-request-credential",
        "issuer": "https://token.actions.githubusercontent.com",
        "subject": "repo:'$GITHUB_OWNER'/'$GITHUB_REPO':pull_request", 
        "description": "GitHub Actions - pull requests",
        "audiences": ["api://AzureADTokenExchange"]
    }'
```

#### Passo 2: Verificar Secrets no GitHub

Acesse: `https://github.com/kledsonhugo/app-todo-list/settings/secrets/actions`

Certifique-se de que existem:
- ✅ `AZURE_CLIENT_ID` 
- ✅ `AZURE_TENANT_ID`
- ✅ `AZURE_SUBSCRIPTION_ID`
- ✅ `PLAYWRIGHT_SERVICE_URL`

## 🔧 Verificação da Configuração

### 1. Listar Credenciais Federadas

```bash
az ad app federated-credential list --id $CLIENT_ID --query "[].{name:name, subject:subject, issuer:issuer}"
```

### 2. Testar Autenticação

Execute o workflow manualmente no GitHub Actions para verificar se o erro foi resolvido.

## 📋 Configuração do Workflow Atual

O workflow está corretamente configurado para OIDC:

```yaml
permissions:
  id-token: write  # ✅ Necessário para OIDC
  contents: read

steps:
- name: 🔐 Login to Azure
  uses: azure/login@v2
  with:
    client-id: ${{ secrets.AZURE_CLIENT_ID }}        # ✅ Sem client-secret
    tenant-id: ${{ secrets.AZURE_TENANT_ID }}
    subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
```

## 🎯 Próximos Passos

1. **Execute o script OIDC**: `./scripts/setup-azure-oidc.sh`
2. **Adicione os secrets** fornecidos pelo script no GitHub
3. **Crie o Azure Playwright Workspace** e obtenha a URL
4. **Adicione `PLAYWRIGHT_SERVICE_URL`** como secret
5. **Execute o workflow** para testar

## 🚨 Troubleshooting

### Se o erro persistir:

1. **Verificar permissões**: Certifique-se de ter permissões de administrador no Azure AD
2. **Aguardar propagação**: As credenciais federadas podem levar alguns minutos para propagar
3. **Verificar sintaxe**: Confirme que o `subject` nas credenciais corresponde exatamente ao repositório

### Logs de Debug:

Se necessário, adicione ao workflow para debug:

```yaml
- name: 🔍 Debug Azure Info
  run: |
    echo "Client ID: ${{ secrets.AZURE_CLIENT_ID }}"
    echo "Tenant ID: ${{ secrets.AZURE_TENANT_ID }}"
    echo "Subscription ID: ${{ secrets.AZURE_SUBSCRIPTION_ID }}"
```

## 📚 Referências

- [Azure Login Action - OIDC](https://github.com/Azure/login#readme)
- [Federated Identity Credentials](https://docs.microsoft.com/azure/active-directory/develop/workload-identity-federation)
- [GitHub OIDC with Azure](https://docs.github.com/actions/deployment/security-hardening-your-deployments/configuring-openid-connect-in-azure)

---

💡 **Dica**: A autenticação OIDC é mais segura pois não requer armazenar segredos no GitHub, usando instead tokens temporários baseados na identidade do workflow.