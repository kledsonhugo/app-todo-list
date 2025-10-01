#!/bin/bash

echo "🔐 Setup Azure OIDC Authentication para GitHub Actions"
echo "====================================================="
echo

# Verificar se o Azure CLI está instalado
if ! command -v az &> /dev/null; then
    echo "❌ Azure CLI não está instalado"
    echo "   Instale em: https://docs.microsoft.com/cli/azure/install-azure-cli"
    exit 1
fi

# Verificar se está logado
if ! az account show >/dev/null 2>&1; then
    echo "❌ Não está logado no Azure CLI"
    echo "   Execute: az login"
    exit 1
fi

echo "✅ Azure CLI configurado!"
echo

# Obter informações
SUBSCRIPTION_ID=$(az account show --query id -o tsv)
TENANT_ID=$(az account show --query tenantId -o tsv)
USER_NAME=$(az account show --query user.name -o tsv)

echo "📋 Informações da sua conta:"
echo "   Usuário: $USER_NAME"
echo "   Tenant ID: $TENANT_ID"
echo "   Subscription ID: $SUBSCRIPTION_ID"
echo

# Obter informações do repositório GitHub
read -p "📝 Digite o proprietário do repositório GitHub (ex: kledsonhugo): " GITHUB_OWNER
read -p "📝 Digite o nome do repositório GitHub (ex: app-todo-list): " GITHUB_REPO

# Validar entrada
if [ -z "$GITHUB_OWNER" ] || [ -z "$GITHUB_REPO" ]; then
    echo "❌ Proprietário e nome do repositório são obrigatórios"
    exit 1
fi

echo
echo "🎯 Configurando para repositório: $GITHUB_OWNER/$GITHUB_REPO"
echo

# Criar Service Principal
echo "🔧 Criando Service Principal..."
SP_NAME="GitHub-OIDC-PlaywrightWorkspaces-$(date +%s)"

# Criar Service Principal e capturar output
SP_OUTPUT=$(az ad sp create-for-rbac \
    --name "$SP_NAME" \
    --role contributor \
    --scopes /subscriptions/$SUBSCRIPTION_ID \
    --output json 2>/dev/null)

if [ $? -ne 0 ]; then
    echo "❌ Erro ao criar Service Principal"
    echo "   Verifique se você tem permissões suficientes"
    exit 1
fi

# Debug: mostrar output para diagnóstico
echo "🔍 Debug - Service Principal Output:"
echo "$SP_OUTPUT"
echo

# Extrair CLIENT_ID usando diferentes métodos
CLIENT_ID=$(echo "$SP_OUTPUT" | jq -r '.appId // .clientId // empty' 2>/dev/null)

# Se jq não estiver disponível, usar grep
if [ -z "$CLIENT_ID" ]; then
    CLIENT_ID=$(echo "$SP_OUTPUT" | grep -oE '"(appId|clientId)"\s*:\s*"[^"]*"' | head -1 | cut -d'"' -f4)
fi

# Última tentativa com sed
if [ -z "$CLIENT_ID" ]; then
    CLIENT_ID=$(echo "$SP_OUTPUT" | sed -n 's/.*"appId":"\([^"]*\)".*/\1/p')
fi

echo "✅ Service Principal criado!"
echo "   Nome: $SP_NAME"

# Verificar se CLIENT_ID foi extraído corretamente
if [ -z "$CLIENT_ID" ]; then
    echo "❌ Erro: Não foi possível extrair o CLIENT_ID"
    echo "   JSON recebido: $SP_OUTPUT"
    echo "   Tentando método manual..."
    
    # Tentar extrair manualmente
    CLIENT_ID=$(az ad sp list --display-name "$SP_NAME" --query "[0].appId" -o tsv 2>/dev/null)
    
    if [ -z "$CLIENT_ID" ]; then
        echo "❌ Falha ao obter CLIENT_ID. Execute manualmente:"
        echo "   az ad sp list --display-name '$SP_NAME' --query '[0].appId' -o tsv"
        exit 1
    fi
fi

echo "   Client ID: $CLIENT_ID"
echo

# Configurar credenciais de identidade federada para OIDC
echo "🔗 Configurando credenciais de identidade federada..."

# Para branch main
az ad app federated-credential create \
    --id $CLIENT_ID \
    --parameters '{
        "name": "main-branch-credential",
        "issuer": "https://token.actions.githubusercontent.com",
        "subject": "repo:'$GITHUB_OWNER'/'$GITHUB_REPO':ref:refs/heads/main",
        "description": "GitHub Actions - main branch",
        "audiences": ["api://AzureADTokenExchange"]
    }' >/dev/null 2>&1

if [ $? -eq 0 ]; then
    echo "✅ Credencial federada para branch main configurada"
else
    echo "⚠️  Erro ao configurar credencial federada para main - pode já existir"
fi

# Para pull requests
az ad app federated-credential create \
    --id $CLIENT_ID \
    --parameters '{
        "name": "pull-request-credential", 
        "issuer": "https://token.actions.githubusercontent.com",
        "subject": "repo:'$GITHUB_OWNER'/'$GITHUB_REPO':pull_request",
        "description": "GitHub Actions - pull requests",
        "audiences": ["api://AzureADTokenExchange"]
    }' >/dev/null 2>&1

if [ $? -eq 0 ]; then
    echo "✅ Credencial federada para pull requests configurada"
else
    echo "⚠️  Erro ao configurar credencial federada para PR - pode já existir"
fi

echo

echo "🔑 ADICIONE ESTES SECRETS NO GITHUB:"
echo "====================================="
echo "Nome do Secret: AZURE_CLIENT_ID"
echo "Valor: $CLIENT_ID"
echo
echo "Nome do Secret: AZURE_TENANT_ID" 
echo "Valor: $TENANT_ID"
echo
echo "Nome do Secret: AZURE_SUBSCRIPTION_ID"
echo "Valor: $SUBSCRIPTION_ID"
echo

echo "📖 COMO ADICIONAR OS SECRETS:"
echo "=============================="
echo "1. Acesse: https://github.com/$GITHUB_OWNER/$GITHUB_REPO/settings/secrets/actions"
echo "2. Clique em 'New repository secret'"
echo "3. Adicione cada secret acima (nome e valor)"
echo

echo "🎭 PRÓXIMOS PASSOS:"
echo "=================="
echo "1. Criar um Azure Playwright Workspace no portal Azure"
echo "2. Obter a URL do endpoint do workspace"
echo "3. Adicionar PLAYWRIGHT_SERVICE_URL como secret no GitHub"
echo "   Formato: https://[workspace-name].[region].playwright.azure.com/"
echo

echo "🔍 VERIFICAR CONFIGURAÇÃO:"
echo "=========================="
echo "Depois de adicionar os secrets, você pode verificar se a autenticação está funcionando"
echo "executando manualmente o workflow no GitHub Actions."
echo

echo "✅ Configuração OIDC completa!"
echo "🚀 Seus workflows GitHub Actions agora devem funcionar com autenticação segura!"
echo
echo "📚 Guia completo: docs/AZURE-CREDENTIALS-SETUP.md"