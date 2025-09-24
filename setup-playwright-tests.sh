#!/bin/bash

# Script para configurar e executar testes Playwright
# Usage: ./setup-playwright-tests.sh [run|setup|demo]

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

log_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

log_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

log_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Função para setup do Playwright
setup_playwright() {
    log_info "Configurando Playwright para testes de interface..."
    
    # Install Playwright CLI globally
    log_info "Instalando Playwright CLI..."
    dotnet tool install --global Microsoft.Playwright.CLI --ignore-failed-sources || true
    
    # Add dotnet tools to PATH
    export PATH="$PATH:~/.dotnet/tools"
    
    # Install browsers
    log_info "Instalando browsers do Playwright..."
    ~/.dotnet/tools/playwright install --with-deps || {
        log_warning "Falha na instalação completa. Tentando apenas Chromium..."
        ~/.dotnet/tools/playwright install chromium || {
            log_error "Falha na instalação dos browsers"
            return 1
        }
    }
    
    log_success "Playwright configurado com sucesso"
}

# Função para executar testes
run_tests() {
    log_info "Executando testes de interface web..."
    
    # Check if app is running
    if ! curl -s http://localhost:5146/api/todos > /dev/null; then
        log_error "Aplicação não está rodando em http://localhost:5146"
        log_info "Execute: dotnet run --project TodoListApp.csproj"
        exit 1
    fi
    
    log_success "Aplicação encontrada em http://localhost:5146"
    
    cd tests/
    
    # Run tests by category
    log_info "Executando testes de funcionalidade principal..."
    dotnet test --filter "TestClass=TodoListWebTests" \
        --logger "console;verbosity=normal" || true
    
    log_info "Executando testes de design responsivo..."
    dotnet test --filter "TestClass=TodoListResponsiveTests" \
        --logger "console;verbosity=normal" || true
    
    log_info "Executando testes de tratamento de erros..."
    dotnet test --filter "TestClass=TodoListErrorHandlingTests" \
        --logger "console;verbosity=normal" || true
    
    log_success "Execução dos testes concluída"
}

# Função para demo
run_demo() {
    log_info "Executando demonstração dos testes..."
    
    # Build tests
    log_info "Building projeto de testes..."
    dotnet build tests/TodoListApp.Tests.csproj
    
    if [ $? -eq 0 ]; then
        log_success "Projeto de testes construído com sucesso"
    else
        log_error "Falha no build do projeto de testes"
        exit 1
    fi
    
    # Run demo script
    if [ -f "test-demo.sh" ]; then
        log_info "Executando script de demonstração..."
        ./test-demo.sh
    else
        log_warning "Script de demonstração não encontrado"
    fi
    
    log_success "Demonstração concluída"
}

# Main script logic
case "${1:-demo}" in
    "setup")
        setup_playwright
        ;;
    "run")
        run_tests
        ;;
    "demo")
        run_demo
        ;;
    "full")
        log_info "Executando setup completo e testes..."
        setup_playwright
        sleep 2
        run_tests
        ;;
    *)
        echo "Usage: $0 [setup|run|demo|full]"
        echo ""
        echo "  setup - Instala e configura Playwright"
        echo "  run   - Executa todos os testes (requer app rodando)"
        echo "  demo  - Demonstração dos testes (sem executar browsers)"
        echo "  full  - Setup + execução completa"
        exit 1
        ;;
esac