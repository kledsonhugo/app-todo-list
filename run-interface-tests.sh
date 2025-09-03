#!/bin/bash

# Script para executar testes de interface automatizados
# Automated Interface Testing Script for Todo List API

set -e

echo "🚀 Iniciando Testes de Interface Automatizados / Starting Automated Interface Tests"
echo "=============================================================================="

# Check if .NET is available
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET não encontrado. Instale o .NET 8.0 SDK"
    exit 1
fi

# Navigate to the project root
cd "$(dirname "$0")"

echo "📂 Diretório atual: $(pwd)"

# Function to check if application is running
check_app_running() {
    if curl -s http://localhost:5146 > /dev/null 2>&1; then
        return 0
    else
        return 1
    fi
}

# Function to start application
start_application() {
    echo "🔄 Iniciando aplicação..."
    dotnet run --project TodoListApp.csproj &
    APP_PID=$!
    
    # Wait for application to start
    echo "⏳ Aguardando aplicação inicializar..."
    for i in {1..30}; do
        if check_app_running; then
            echo "✅ Aplicação iniciada com sucesso na porta 5146"
            return 0
        fi
        sleep 1
    done
    
    echo "❌ Falha ao iniciar aplicação"
    kill $APP_PID 2>/dev/null || true
    exit 1
}

# Function to stop application
stop_application() {
    if [ ! -z "$APP_PID" ]; then
        echo "🔄 Parando aplicação..."
        kill $APP_PID 2>/dev/null || true
        wait $APP_PID 2>/dev/null || true
        echo "✅ Aplicação parada"
    fi
}

# Trap to ensure cleanup
trap stop_application EXIT

# Check if application is already running
if ! check_app_running; then
    echo "📋 Aplicação não está rodando. Iniciando..."
    
    # Build the application first
    echo "🔨 Construindo aplicação..."
    dotnet build app-todo-list.sln
    
    # Start application
    start_application
else
    echo "✅ Aplicação já está rodando na porta 5146"
fi

# Build and run tests
echo "🔨 Construindo testes..."
dotnet build TodoListApp.Tests/TodoListApp.Tests.csproj

echo ""
echo "🧪 EXECUTANDO TESTES DE INTERFACE AUTOMATIZADOS"
echo "==============================================="
cd TodoListApp.Tests
dotnet run

echo ""
echo "✅ Testes de Interface Completados!"
echo "🎯 Para executar novamente: ./run-interface-tests.sh"
echo ""
echo "📚 Documentação completa em: TESTES_INTERFACE.md"
echo "🌐 Interface: http://localhost:5146"
echo "📋 API: http://localhost:5146/api/todos"