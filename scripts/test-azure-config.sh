#!/bin/bash

# Script para testar configuração do Azure Playwright Workspaces
# Verifica se a conexão está funcionando corretamente

set -e

echo "🔍 Testando configuração Azure Playwright Workspaces..."
echo "=================================================="

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

# Verificar variáveis de ambiente
echo
log_info "Verificando variáveis de ambiente..."

if [ -n "$PLAYWRIGHT_SERVICE_URL" ]; then
    log_success "PLAYWRIGHT_SERVICE_URL: $PLAYWRIGHT_SERVICE_URL"
else
    log_warning "PLAYWRIGHT_SERVICE_URL não está definida"
fi

if [ -n "$PLAYWRIGHT_WORKERS" ]; then
    log_success "PLAYWRIGHT_WORKERS: $PLAYWRIGHT_WORKERS"
else
    log_info "PLAYWRIGHT_WORKERS não está definida (usará padrão)"
fi

# Verificar se está em CI
if [ "$CI" = "true" ]; then
    log_success "Executando em ambiente CI"
    if [ -n "$GITHUB_REPOSITORY" ]; then
        log_success "GitHub Repository: $GITHUB_REPOSITORY"
    fi
    if [ -n "$GITHUB_RUN_ID" ]; then
        log_success "GitHub Run ID: $GITHUB_RUN_ID"
    fi
else
    log_info "Executando em ambiente local"
fi

# Testar configuração do Playwright
echo
log_info "Testando configuração do Playwright..."

cd /home/kledsonbasso/source/app-todo-list/tests

# Verificar se configuração carrega sem erros
if npx playwright --version > /dev/null 2>&1; then
    PLAYWRIGHT_VERSION=$(npx playwright --version)
    log_success "Playwright CLI está funcionando: $PLAYWRIGHT_VERSION"
else
    log_error "Problema com Playwright CLI"
fi

# Tentar carregar a configuração
echo
log_info "Carregando configuração do serviço..."

# Usar Node.js para testar a configuração
cat > temp-config-test.mjs << 'EOF'
import { defineConfig, devices } from '@playwright/test';

// Detectar se deve usar Azure Workspaces
const serviceUrl = process.env.PLAYWRIGHT_SERVICE_URL;
const isCI = process.env.CI === 'true';
const useAzureWorkspaces = !!(serviceUrl && serviceUrl.trim() && isCI);

console.log('🔧 Configuração detectada:');
console.log(`   Service URL: ${serviceUrl || 'não definida'}`);
console.log(`   CI Environment: ${isCI}`);
console.log(`   Use Azure Workspaces: ${useAzureWorkspaces}`);

if (useAzureWorkspaces) {
    console.log('✅ Configuração Azure Workspaces será usada');
    console.log(`   Workers: ${process.env.PLAYWRIGHT_WORKERS || '20'}`);
    console.log('   Browsers: chromium, firefox, webkit, Mobile Chrome, Mobile Safari');
} else {
    console.log('🏠 Configuração local será usada');
    console.log('   Workers: 4');
    console.log('   Browsers: chromium, firefox, webkit');
}

// Testar se a configuração pode ser interpretada
try {
    // Simular a lógica da configuração sem importar o arquivo TS diretamente
    const config = defineConfig({
        testDir: './e2e',
        workers: useAzureWorkspaces ? parseInt(process.env.PLAYWRIGHT_WORKERS || '20') : 4,
        projects: useAzureWorkspaces ? [
            { name: 'chromium', use: { ...devices['Desktop Chrome'] } },
            { name: 'firefox', use: { ...devices['Desktop Firefox'] } },
            { name: 'webkit', use: { ...devices['Desktop Safari'] } },
            { name: 'Mobile Chrome', use: { ...devices['Pixel 5'] } },
            { name: 'Mobile Safari', use: { ...devices['iPhone 12'] } },
        ] : [
            { name: 'chromium', use: { ...devices['Desktop Chrome'] } },
            { name: 'firefox', use: { ...devices['Desktop Firefox'] } },
            { name: 'webkit', use: { ...devices['Desktop Safari'] } },
        ]
    });
    
    console.log('✅ Configuração simulada com sucesso');
    console.log(`   Projetos configurados: ${config.projects?.length || 0}`);
    console.log(`   Workers configurados: ${config.workers}`);
} catch (error) {
    console.error('❌ Erro ao simular configuração:', error.message);
    process.exit(1);
}
EOF

if node temp-config-test.mjs; then
    log_success "Configuração testada com sucesso"
else
    log_error "Erro ao testar configuração"
fi

# Limpeza
rm -f temp-config-test.mjs

# Testar conectividade se Azure está configurado
if [ -n "$PLAYWRIGHT_SERVICE_URL" ] && [ "$CI" = "true" ]; then
    echo
    log_info "Testando conectividade com Azure Playwright Workspaces..."
    
    # Tentar fazer um ping básico para a URL do serviço
    if command -v curl > /dev/null 2>&1; then
        # Extrair domínio da URL
        domain=$(echo "$PLAYWRIGHT_SERVICE_URL" | sed -E 's|wss?://([^/]+).*|\1|')
        
        echo "   Testando conectividade para: $domain"
        if curl -s --connect-timeout 10 --max-time 30 "https://$domain" > /dev/null 2>&1; then
            log_success "Conectividade básica OK"
        else
            log_warning "Conectividade básica falhou (pode ser normal se endpoint não aceita HTTP)"
        fi
    else
        log_info "curl não disponível, pulando teste de conectividade"
    fi
    
    # Verificar formato da URL
    if [[ "$PLAYWRIGHT_SERVICE_URL" =~ ^wss:// ]]; then
        log_success "URL do serviço tem formato WebSocket correto"
    else
        log_warning "URL do serviço pode ter formato incorreto (esperado: wss://...)"
    fi
fi

echo
echo "=================================================="
log_info "Teste de configuração concluído!"

# Sugestões baseadas no que foi encontrado
echo
log_info "💡 Próximos passos sugeridos:"

if [ -z "$PLAYWRIGHT_SERVICE_URL" ] || [ "$CI" != "true" ]; then
    echo "   1. Para testar localmente: npm run test"
    echo "   2. Para testar configuração Azure: definir PLAYWRIGHT_SERVICE_URL e CI=true"
else
    echo "   1. Executar testes: npm run test:azure"
    echo "   2. Verificar logs de autenticação no GitHub Actions"
    echo "   3. Confirmar que Service Principal tem permissões corretas"
fi

echo "   4. Verificar documentação em docs/AZURE_SETUP.md"
echo