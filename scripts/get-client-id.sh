#!/bin/bash

echo "🔐 Azure OIDC Quick Fix - Obter CLIENT_ID existente"
echo "=================================================="
echo

# Verificar se o Azure CLI está instalado
if ! command -v az &> /dev/null; then
    echo "❌ Azure CLI não está instalado"
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

echo "📋 Informações da conta:"
echo "   Tenant ID: $TENANT_ID"
echo "   Subscription ID: $SUBSCRIPTION_ID"
echo

# Listar Service Principals criados recentemente
echo "🔍 Procurando Service Principals criados hoje..."
SP_LIST=$(az ad sp list --query "[?contains(displayName, 'GitHub') && contains(displayName, 'Playwright')].{name:displayName, appId:appId}" -o table)

if [ -n "$SP_LIST" ]; then
    echo "📋 Service Principals encontrados:"
    echo "$SP_LIST"
    echo
    
    # Pegar o mais recente
    LATEST_SP=$(az ad sp list --query "[?contains(displayName, 'GitHub') && contains(displayName, 'Playwright')] | [0]" -o json)
    CLIENT_ID=$(echo "$LATEST_SP" | jq -r '.appId // empty')
    SP_NAME=$(echo "$LATEST_SP" | jq -r '.displayName // empty')
    
    if [ -n "$CLIENT_ID" ]; then
        echo "✅ CLIENT_ID encontrado:"
        echo "   Service Principal: $SP_NAME"
        echo "   CLIENT_ID: $CLIENT_ID"
        echo
        
        # Verificar se tem credenciais federadas
        echo "🔍 Verificando credenciais federadas..."
        FEDERATED_CREDS=$(az ad app federated-credential list --id "$CLIENT_ID" --query "length(@)" -o tsv 2>/dev/null)
        
        if [ "$FEDERATED_CREDS" -gt 0 ]; then
            echo "✅ Credenciais federadas já configuradas ($FEDERATED_CREDS encontradas)"
        else
            echo "⚠️  Nenhuma credencial federada encontrada. Configurando..."
            
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
        
    else
        echo "❌ Não foi possível extrair CLIENT_ID automaticamente"
    fi
else
    echo "⚠️  Nenhum Service Principal relacionado ao GitHub/Playwright encontrado"
    echo "   Execute o script principal: ./setup-azure-oidc.sh"
fi

echo
echo "🧪 TESTAR AUTENTICAÇÃO:"
echo "======================="
echo "Após adicionar os secrets, execute o workflow manualmente no GitHub Actions"
echo "para verificar se a autenticação OIDC está funcionando."
echo