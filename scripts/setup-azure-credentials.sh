#!/bin/bash

# Script para configurar Service Principal para GitHub Actions
# Azure Playwright Workspaces Authentication Setup

echo "🔐 Configurando Service Principal para GitHub Actions + Azure Playwright Workspaces"
echo

# Verificar se está logado no Azure
if ! az account show >/dev/null 2>&1; then
    echo "❌ Você não está logado no Azure CLI"
    echo "   Execute: az login"
    exit 1
fi

# Obter informações da conta atual
SUBSCRIPTION_ID=$(az account show --query id -o tsv)
SUBSCRIPTION_NAME=$(az account show --query name -o tsv)
TENANT_ID=$(az account show --query tenantId -o tsv)

echo "✅ Informações da Conta Azure:"
echo "   Subscription: $SUBSCRIPTION_NAME"
echo "   Subscription ID: $SUBSCRIPTION_ID"
echo "   Tenant ID: $TENANT_ID"
echo

# Nome do Service Principal
SP_NAME="GitHub-Actions-PlaywrightWorkspaces-$(date +%Y%m%d)"

echo "🔧 Criando Service Principal: $SP_NAME"

# Criar Service Principal com permissões Contributor
SP_OUTPUT=$(az ad sp create-for-rbac \
    --name "$SP_NAME" \
    --role contributor \
    --scopes /subscriptions/$SUBSCRIPTION_ID \
    --sdk-auth)

if [ $? -eq 0 ]; then
    echo "✅ Service Principal criado com sucesso!"
    echo
    echo "🔑 CREDENCIAIS PARA GITHUB SECRETS:"
    echo "=================================="
    
    # Extrair valores do JSON
    CLIENT_ID=$(echo $SP_OUTPUT | jq -r '.clientId')
    CLIENT_SECRET=$(echo $SP_OUTPUT | jq -r '.clientSecret')
    
    echo "AZURE_CLIENT_ID: $CLIENT_ID"
    echo "AZURE_TENANT_ID: $TENANT_ID"
    echo "AZURE_SUBSCRIPTION_ID: $SUBSCRIPTION_ID"
    echo "AZURE_CLIENT_SECRET: $CLIENT_SECRET"
    echo
    echo "📋 JSON COMPLETO PARA AZURE_CREDENTIALS:"
    echo "$SP_OUTPUT"
    echo
else
    echo "❌ Erro ao criar Service Principal"
    exit 1
fi

# Verificar se há Playwright Workspaces
echo "🎭 Verificando Playwright Workspaces..."
WORKSPACES=$(az resource list --resource-type "Microsoft.App/PlaywrightWorkspaces" --query "[].{name:name,resourceGroup:resourceGroup,location:location}" -o table)

if [ ! -z "$WORKSPACES" ]; then
    echo "✅ Playwright Workspaces encontrados:"
    echo "$WORKSPACES"
    echo
    echo "💡 Para cada workspace, você precisará:"
    echo "   1. Obter a URL do endpoint (Azure Portal > Workspace > Get Started)"
    echo "   2. Adicionar PLAYWRIGHT_SERVICE_URL aos GitHub Secrets"
else
    echo "⚠️  Nenhum Playwright Workspace encontrado"
    echo "   Crie um em: https://portal.azure.com > Azure App Testing > Playwright Workspaces"
fi

echo
echo "🚀 PRÓXIMOS PASSOS:"
echo "=================="
echo "1. Copie as credenciais acima"
echo "2. Vá para seu repositório GitHub > Settings > Secrets and variables > Actions"
echo "3. Adicione os seguintes Repository Secrets:"
echo "   - AZURE_CLIENT_ID"
echo "   - AZURE_TENANT_ID" 
echo "   - AZURE_SUBSCRIPTION_ID"
echo "   - PLAYWRIGHT_SERVICE_URL (do seu workspace)"
echo
echo "4. (Opcional) Para compatibilidade com workflows antigos:"
echo "   - AZURE_CREDENTIALS (JSON completo)"
echo
echo "✨ Após configurar, seus workflows GitHub Actions poderão autenticar no Azure!"