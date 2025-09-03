using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TodoListApp.Tests;

/// <summary>
/// Custom Web Application Factory for testing the Todo List app
/// Provides integration testing capabilities with proper setup
/// </summary>
public class TodoListWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Override services for testing if needed
            // For example, we could replace the TodoService with a test implementation
        });

        builder.UseEnvironment("Testing");
        
        // Suppress logging during tests to reduce noise
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });
    }
}

/// <summary>
/// Helper class for common test operations and utilities
/// Implements MCP-style intelligent test assistance
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Generates test data for creating new todos
    /// </summary>
    public static (string title, string description) GenerateTestTodoData(string prefix = "Test")
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"); // Added milliseconds for uniqueness
        var uniqueId = Guid.NewGuid().ToString("N")[..8]; // Add unique identifier
        return (
            title: $"{prefix} Todo {timestamp}_{uniqueId}",
            description: $"Automated test description created at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {uniqueId}"
        );
    }

    /// <summary>
    /// Common CSS selectors used throughout tests
    /// </summary>
    public static class Selectors
    {
        public const string TodoTitle = "#todoTitle";
        public const string TodoDescription = "#todoDescription";
        public const string SubmitButton = "button[type=submit]";
        public const string TodoItem = ".todo-item";
        public const string AllFilter = "button[data-filter='all']";
        public const string PendingFilter = "button[data-filter='pending']";
        public const string CompletedFilter = "button[data-filter='completed']";
        public const string RefreshButton = "button:has-text('Atualizar')";
        public const string EditModal = "#editModal";
        public const string EditTitle = "#editTitle";
        public const string EditDescription = "#editDescription";
        public const string SaveChangesButton = "button:has-text('Salvar Alterações')";
    }

    /// <summary>
    /// Common test timeouts
    /// </summary>
    public static class Timeouts
    {
        public const int StandardWait = 1000;
        public const int LongWait = 5000;
        public const int NetworkWait = 3000;
    }

    /// <summary>
    /// Expected test data for validation
    /// </summary>
    public static class ExpectedData
    {
        public static readonly string[] InitialTodoTitles = 
        {
            "Estudar .NET",
            "Fazer exercícios", 
            "Ler documentação"
        };

        public const int InitialTodoCount = 3;
        public const string PageTitle = "Todo List - Gerenciador de Tarefas";
        public const string MainHeading = "Minha Lista de Tarefas";
    }
}