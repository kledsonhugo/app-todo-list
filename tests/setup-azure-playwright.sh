#!/bin/bash

# Script para configurar Azure Playwright Workspaces
# Azure Playwright Workspaces Setup Script

echo "🚀 Configurando Azure Playwright Workspaces..."
echo

# Verificar se estamos no diretório correto
if [ ! -f "package.json" ]; then
    echo "❌ Erro: Execute este script no diretório tests/"
    echo "   cd tests && ./setup-azure-playwright.sh"
    exit 1
fi

# Instalar dependências
echo "📦 Instalando dependências..."
npm install

# Verificar se as dependências do Azure foram instaladas
if ! npm list @azure/playwright >/dev/null 2>&1; then
    echo "❌ Erro: @azure/playwright não foi instalado corretamente"
    echo "   Verifique se o package.json contém a dependência @azure/playwright"
    exit 1
fi

echo "✅ Dependências instaladas com sucesso!"
echo

# Verificar arquivo de configuração
if [ ! -f "playwright.service.config.ts" ]; then
    echo "❌ Erro: playwright.service.config.ts não encontrado"
    echo "   Este arquivo deve estar presente no diretório tests/"
    exit 1
fi

echo "✅ Arquivo de configuração encontrado!"
echo

# Verificar variáveis de ambiente
if [ ! -f ".env" ]; then
    echo "⚠️  Arquivo .env não encontrado. Criando a partir do exemplo..."
    
    if [ -f ".env.example" ]; then
        cp .env.example .env
        echo "✅ Arquivo .env criado a partir de .env.example"
        echo "📝 IMPORTANTE: Edite o arquivo .env e configure:"
        echo "   - PLAYWRIGHT_SERVICE_URL (endpoint do seu Azure Playwright Workspace)"
        echo "   - PLAYWRIGHT_WORKERS (número de workers, recomendado: 4 para desenvolvimento)"
    else
        echo "❌ Erro: .env.example não encontrado"
        exit 1
    fi
else
    echo "✅ Arquivo .env encontrado!"
fi

echo

# Verificar autenticação Azure
echo "🔐 Verificando autenticação Azure..."
if command -v az >/dev/null 2>&1; then
    if az account show >/dev/null 2>&1; then
        echo "✅ Autenticado no Azure CLI"
        AZURE_USER=$(az account show --query user.name -o tsv 2>/dev/null)
        AZURE_SUBSCRIPTION=$(az account show --query name -o tsv 2>/dev/null)
        echo "   Usuário: $AZURE_USER"
        echo "   Subscription: $AZURE_SUBSCRIPTION"
    else
        echo "⚠️  Não autenticado no Azure CLI"
        echo "   Execute: az login"
    fi
else
    echo "⚠️  Azure CLI não instalado"
    echo "   Instale: https://docs.microsoft.com/cli/azure/install-azure-cli"
fi

echo

# Instruções finais
echo "🎯 Próximos passos:"
echo
echo "1. 📝 Configure o arquivo .env:"
echo "   - Obtenha PLAYWRIGHT_SERVICE_URL do Azure Portal"
echo "   - Configure PLAYWRIGHT_WORKERS (recomendado: 4)"
echo
echo "2. 🔐 Configure autenticação (se não feito):"
echo "   - Azure CLI: az login"
echo "   - GitHub Secrets: AZURE_CLIENT_ID, AZURE_TENANT_ID, AZURE_SUBSCRIPTION_ID"
echo
echo "3. 🧪 Execute testes:"
echo "   - Local: npx playwright test --config=playwright.service.config.ts"
echo "   - CI/CD: Workflows automáticos com GitHub Actions"
echo
echo "📚 Documentação completa: docs/AZURE-PLAYWRIGHT-SETUP.md"
echo

echo "✨ Configuração concluída!"