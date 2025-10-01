#!/bin/bash

echo "🔐 Setup Azure Credentials para GitHub Actions"
echo "=============================================="
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

# Criar Service Principal
echo "🔧 Criando Service Principal..."
SP_OUTPUT=$(az ad sp create-for-rbac \
    --name "GitHub-PlaywrightWorkspaces-$(date +%s)" \
    --role contributor \
    --scopes /subscriptions/$SUBSCRIPTION_ID \
    --json-auth 2>/dev/null)

if [ $? -eq 0 ]; then
    echo "✅ Service Principal criado!"
    echo
    
    # Extrair CLIENT_ID
    CLIENT_ID=$(echo "$SP_OUTPUT" | grep -o '"clientId":"[^"]*"' | cut -d'"' -f4)
    
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
    echo "📝 JSON completo para AZURE_CREDENTIALS (se necessário):"
    echo "$SP_OUTPUT"
    echo
else
    echo "❌ Erro ao criar Service Principal"
    echo "   Verifique se você tem permissões suficientes"
    exit 1
fi

echo "🎭 Agora você precisa:"
echo "1. Criar um Azure Playwright Workspace no portal Azure"
echo "2. Obter a URL do endpoint do workspace"
echo "3. Adicionar PLAYWRIGHT_SERVICE_URL como secret no GitHub"
echo
echo "🚀 Após isso, seus workflows GitHub Actions funcionarão!"
echo
echo "📚 Guia completo: docs/AZURE-CREDENTIALS-SETUP.md"