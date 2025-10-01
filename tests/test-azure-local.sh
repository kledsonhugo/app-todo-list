#!/bin/bash

# Script para testar Azure Playwright Workspaces localmente
# Carrega configurações do .env e executa testes

set -e

# Cores para output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para log colorido
log_info() { echo -e "${BLUE}ℹ️  $1${NC}"; }
log_success() { echo -e "${GREEN}✅ $1${NC}"; }
log_warning() { echo -e "${YELLOW}⚠️  $1${NC}"; }
log_error() { echo -e "${RED}❌ $1${NC}"; }

echo "🎭 Testing Azure Playwright Workspaces Locally"
echo "==============================================="

# Verificar se está no diretório correto
if [ ! -f "package.json" ]; then
    log_error "Execute este script do diretório tests/"
    exit 1
fi

# Carregar variáveis do .env se existir
if [ -f ".env" ]; then
    log_info "Carregando configurações do .env..."
    set -a
    source .env
    set +a
    log_success "Variáveis carregadas do .env"
else
    log_warning "Arquivo .env não encontrado"
    log_info "Criando .env de exemplo..."
    cat > .env << 'EOF'
# Azure Playwright Workspaces Configuration
# Get this from your Azure portal -> Your Playwright workspace -> Get Started page
PLAYWRIGHT_SERVICE_URL=wss://eastus.api.playwright.microsoft.com/playwrightworkspaces/6caad8c5-b415-4edd-a4f4-eac26a8e0f86/browsers

# Number of parallel workers
PLAYWRIGHT_WORKERS=4

# GitHub repository information (automatically set in CI/CD)
GITHUB_REPOSITORY=app-todo-list
GITHUB_RUN_ID=local-development
EOF
    log_success "Arquivo .env criado com valores de exemplo"
    log_warning "Por favor, atualize o PLAYWRIGHT_SERVICE_URL com sua workspace"
fi

# Verificar se as variáveis necessárias estão definidas
if [ -z "$PLAYWRIGHT_SERVICE_URL" ]; then
    log_error "PLAYWRIGHT_SERVICE_URL não está definida no .env"
    exit 1
fi

# Mostrar configuração
echo
log_info "Configuração Azure Playwright Workspaces:"
echo "   Service URL: $PLAYWRIGHT_SERVICE_URL"
echo "   Workers: ${PLAYWRIGHT_WORKERS:-4}"
echo "   Repository: ${GITHUB_REPOSITORY:-local}"
echo "   Run ID: ${GITHUB_RUN_ID:-local-development}"

# Verificar se a aplicação está rodando
echo
log_info "Verificando se a aplicação está rodando..."
if curl -f -s --max-time 5 http://localhost:5146/api/todos > /dev/null; then
    log_success "Aplicação está rodando em http://localhost:5146"
else
    log_error "Aplicação não está rodando!"
    log_info "Execute primeiro: cd .. && dotnet run --project TodoListApp.csproj"
    exit 1
fi

# Verificar modo de execução
MODE=${1:-"azure"}
case $MODE in
    "azure")
        log_info "Executando testes com Azure Playwright Workspaces..."
        export CI=true
        npm run test:azure
        ;;
    "local")
        log_info "Executando testes em modo local..."
        unset CI
        npm run test:local
        ;;
    "compare")
        log_info "Executando comparação: local vs Azure..."
        echo
        log_info "1. Executando em modo local..."
        unset CI
        npm run test:local
        
        echo
        log_info "2. Executando com Azure Workspaces..."
        export CI=true
        npm run test:azure
        ;;
    *)
        log_error "Modo inválido: $MODE"
        echo "Uso: $0 [azure|local|compare]"
        echo "  azure   - Executar com Azure Playwright Workspaces"
        echo "  local   - Executar em modo local"
        echo "  compare - Executar ambos e comparar"
        exit 1
        ;;
esac

echo
log_success "Testes concluídos!"
log_info "Para ver o relatório: npm run report"