#!/bin/bash

echo "🔍 Busca Específica por Service Principal"
echo "========================================"
echo

# Verificar se o Azure CLI está instalado
if ! command -v az &> /dev/null; then
    echo "❌ Azure CLI não está instalado"
    exit 1
fi

# Verificar se está logado
if ! az account show >/dev/null 2>&1; then
    echo "❌ Não está logado no Azure CLI"
    exit 1
fi

echo "✅ Azure CLI configurado!"
echo

# Obter informações
SUBSCRIPTION_ID=$(az account show --query id -o tsv)
TENANT_ID=$(az account show --query tenantId -o tsv)

echo "📋 Informações da conta:"
echo "   Tenant ID: $TENANT_ID"
echo "   Subscription ID: $SUBSCRIPTION_ID"
echo

# Buscar Service Principals com filtro mais específico
echo "🔍 Buscando todos os Service Principals com 'GitHub' no nome..."

# Primeiro, tentar buscar pelo nome específico que sabemos que foi criado
SP_NAME_PATTERN="GitHub-OIDC-PlaywrightWorkspaces-1759311269"
echo "🎯 Procurando especificamente por: $SP_NAME_PATTERN"

SP_DETAILS=$(az ad sp list --display-name "$SP_NAME_PATTERN" --query "[0].{name:displayName, appId:appId}" -o json 2>/dev/null)

if [ "$SP_DETAILS" != "null" ] && [ -n "$SP_DETAILS" ]; then
    CLIENT_ID=$(echo "$SP_DETAILS" | jq -r '.appId // empty')
    SP_NAME=$(echo "$SP_DETAILS" | jq -r '.name // empty')
    
    echo "✅ Service Principal encontrado!"
    echo "   Nome: $SP_NAME"
    echo "   CLIENT_ID: $CLIENT_ID"
    echo
else
    echo "⚠️  Service Principal específico não encontrado. Tentando busca ampla..."
    
    # Buscar todos os SPs que contêm "GitHub" usando --all
    echo "🔍 Buscando todos os Service Principals (pode demorar)..."
    ALL_SPS=$(az ad sp list --all --query "[?contains(displayName, 'GitHub')].{name:displayName, appId:appId, created:createdDateTime}" -o json 2>/dev/null)
    
    if [ "$ALL_SPS" != "[]" ] && [ -n "$ALL_SPS" ]; then
        echo "📋 Service Principals com 'GitHub' encontrados:"
        echo "$ALL_SPS" | jq -r '.[] | "   Nome: \(.name) | CLIENT_ID: \(.appId) | Criado: \(.created)"'
        echo
        
        # Pegar o mais recente
        LATEST_SP=$(echo "$ALL_SPS" | jq -r '.[0]')
        CLIENT_ID=$(echo "$LATEST_SP" | jq -r '.appId // empty')
        SP_NAME=$(echo "$LATEST_SP" | jq -r '.name // empty')
        
        echo "🎯 Usando o mais recente:"
        echo "   Nome: $SP_NAME"
        echo "   CLIENT_ID: $CLIENT_ID"
        echo
    else
        echo "❌ Nenhum Service Principal encontrado com 'GitHub' no nome"
        echo
        echo "🔧 SOLUÇÃO MANUAL:"
        echo "=================="
        echo "1. Liste todos os Service Principals:"
        echo "   az ad sp list --query \"[].{name:displayName, appId:appId}\" -o table"
        echo
        echo "2. Procure pelo Service Principal criado recentemente"
        echo "3. Copie o appId (CLIENT_ID)"
        echo
        exit 1
    fi
fi

# Se chegamos aqui, temos um CLIENT_ID
if [ -n "$CLIENT_ID" ]; then
    echo "🔗 Verificando e configurando credenciais federadas..."
    
    # Verificar se tem credenciais federadas
    FEDERATED_CREDS=$(az ad app federated-credential list --id "$CLIENT_ID" --query "length(@)" -o tsv 2>/dev/null)
    
    if [ "$FEDERATED_CREDS" -gt 0 ]; then
        echo "✅ Credenciais federadas já configuradas ($FEDERATED_CREDS encontradas)"
    else
        echo "⚠️  Configurando credenciais federadas..."
        
        # Configurar credenciais federadas
        GITHUB_OWNER="kledsonhugo"
        GITHUB_REPO="app-todo-list"
        
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
            echo "⚠️  Erro ao configurar credencial para main - pode já existir"
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
            echo "⚠️  Erro ao configurar credencial para PR - pode já existir"
        fi
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
    echo "📖 ACESSE: https://github.com/kledsonhugo/app-todo-list/settings/secrets/actions"
    echo "1. Clique em 'New repository secret'"
    echo "2. Adicione cada secret acima (nome e valor exatos)"
    echo
    echo "🎭 AINDA FALTA:"
    echo "==============="
    echo "4. Criar Azure Playwright Workspace e obter PLAYWRIGHT_SERVICE_URL"
    echo "5. Adicionar PLAYWRIGHT_SERVICE_URL como secret no GitHub"
    echo
    echo "✅ Depois disso, execute o workflow para testar!"
fi