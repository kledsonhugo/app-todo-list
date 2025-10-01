#!/bin/bash

echo "🎯 Solução Simples - Comando Direto"
echo "==================================="
echo

echo "📋 Execute este comando para listar TODOS os Service Principals:"
echo "================================================================"
echo "az ad sp list --query \"[].{name:displayName, appId:appId}\" -o table"
echo
echo "📝 Procure na lista pelo Service Principal com nome parecido com:"
echo "   'GitHub-OIDC-PlaywrightWorkspaces-1759311269'"
echo "   ou qualquer outro que contenha 'GitHub' e 'Playwright'"
echo
echo "📋 Seus dados para os secrets do GitHub:"
echo "========================================="

# Obter informações básicas
SUBSCRIPTION_ID=$(az account show --query id -o tsv)
TENANT_ID=$(az account show --query tenantId -o tsv)

echo "AZURE_TENANT_ID: $TENANT_ID"
echo "AZURE_SUBSCRIPTION_ID: $SUBSCRIPTION_ID"
echo "AZURE_CLIENT_ID: [COPIE o appId da lista acima]"
echo
echo "🔗 Local para adicionar no GitHub:"
echo "https://github.com/kledsonhugo/app-todo-list/settings/secrets/actions"
echo

# Executar o comando automaticamente
echo "🔍 Executando comando automaticamente..."
echo "========================================"
az ad sp list --query "[].{name:displayName, appId:appId}" -o table | grep -i github || echo "⚠️ Nenhum resultado com 'github' encontrado"
echo
echo "Se não encontrou nada acima, execute o comando completo manualmente:"
echo "az ad sp list --query \"[].{name:displayName, appId:appId}\" -o table"