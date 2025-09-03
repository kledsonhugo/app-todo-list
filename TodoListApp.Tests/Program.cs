using System;
using System.Threading.Tasks;
using TodoListApp.Tests.Integration;

namespace TodoListApp.Tests;

/// <summary>
/// Runner principal para testes de interface automatizados
/// Executa testes de API e interface web para validação QA
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🚀 INICIANDO TESTES DE INTERFACE AUTOMATIZADOS");
        Console.WriteLine("==============================================");
        Console.WriteLine();

        var totalTests = 0;
        var passedTests = 0;

        // Aguardar um pouco para garantir que a aplicação está rodando
        Console.WriteLine("⏳ Aguardando aplicação estar disponível...");
        await Task.Delay(2000);

        // Testes de API
        Console.WriteLine("📋 EXECUTANDO TESTES DE INTEGRAÇÃO DA API");
        Console.WriteLine("==========================================");
        
        var apiTests = new ApiIntegrationTests();

        // Teste 1: GET Default Todos
        totalTests++;
        Console.Write("1. Buscar tarefas padrão........................ ");
        var test1 = await apiTests.TestGetDefaultTodos();
        Console.WriteLine(test1 ? "✅ PASSOU" : "❌ FALHOU");
        if (test1) passedTests++;

        // Teste 2: POST Create Todo
        totalTests++;
        Console.Write("2. Criar nova tarefa........................... ");
        var test2 = await apiTests.TestCreateTodo();
        Console.WriteLine(test2 ? "✅ PASSOU" : "❌ FALHOU");
        if (test2) passedTests++;

        // Teste 3: PUT Update Todo
        totalTests++;
        Console.Write("3. Atualizar tarefa existente.................. ");
        var test3 = await apiTests.TestUpdateTodo();
        Console.WriteLine(test3 ? "✅ PASSOU" : "❌ FALHOU");
        if (test3) passedTests++;

        // Teste 4: PATCH Toggle Todo
        totalTests++;
        Console.Write("4. Alternar status de conclusão............... ");
        var test4 = await apiTests.TestToggleTodo();
        Console.WriteLine(test4 ? "✅ PASSOU" : "❌ FALHOU");
        if (test4) passedTests++;

        // Teste 5: Complete Workflow
        totalTests++;
        Console.Write("5. Workflow completo (CRUD)................... ");
        var test5 = await apiTests.TestCompleteWorkflow();
        Console.WriteLine(test5 ? "✅ PASSOU" : "❌ FALHOU");
        if (test5) passedTests++;

        apiTests.Dispose();

        Console.WriteLine();
        
        // Testes de Interface Web (usando verificações básicas)
        Console.WriteLine("🌐 EXECUTANDO TESTES DE INTERFACE WEB");
        Console.WriteLine("=====================================");
        
        var webTests = new WebInterfaceTests();

        // Teste 6: Homepage Loading
        totalTests++;
        Console.Write("6. Carregamento da página inicial............. ");
        var test6 = await webTests.TestHomepageLoads();
        Console.WriteLine(test6 ? "✅ PASSOU" : "❌ FALHOU");
        if (test6) passedTests++;

        // Teste 7: API Endpoint Availability
        totalTests++;
        Console.Write("7. Disponibilidade da API..................... ");
        var test7 = await webTests.TestApiEndpointAvailability();
        Console.WriteLine(test7 ? "✅ PASSOU" : "❌ FALHOU");
        if (test7) passedTests++;

        webTests.Dispose();

        // Relatório Final
        Console.WriteLine();
        Console.WriteLine("📊 RELATÓRIO FINAL DE QA");
        Console.WriteLine("========================");
        Console.WriteLine($"Total de Testes: {totalTests}");
        Console.WriteLine($"Testes Passou: {passedTests}");
        Console.WriteLine($"Testes Falhou: {totalTests - passedTests}");
        Console.WriteLine($"Taxa de Sucesso: {(double)passedTests / totalTests * 100:F1}%");
        
        if (passedTests == totalTests)
        {
            Console.WriteLine();
            Console.WriteLine("🎉 TODOS OS TESTES PASSARAM!");
            Console.WriteLine("✅ Aplicação validada com sucesso");
            Console.WriteLine("🚀 Sistema pronto para produção");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("⚠️  ALGUNS TESTES FALHARAM");
            Console.WriteLine("🔍 Verifique os logs acima para detalhes");
            Console.WriteLine("🛠️  Correções necessárias antes do deploy");
        }

        Console.WriteLine();
        Console.WriteLine("💡 Para mais informações:");
        Console.WriteLine("   - API: http://localhost:5146/api/todos");
        Console.WriteLine("   - Interface: http://localhost:5146");
        Console.WriteLine("   - Swagger: http://localhost:5146/api/docs");
    }
}