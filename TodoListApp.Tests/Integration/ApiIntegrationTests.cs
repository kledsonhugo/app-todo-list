using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using TodoListApp.Tests.Models;

namespace TodoListApp.Tests.Integration;

/// <summary>
/// Testes de integração da API Todo List
/// Valida funcionalidades CRUD e comportamentos da API
/// </summary>
public class ApiIntegrationTests
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://localhost:5146";

    public ApiIntegrationTests()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    /// <summary>
    /// Testa se a API retorna as tarefas padrão
    /// </summary>
    public async Task<bool> TestGetDefaultTodos()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/todos");
            if (!response.IsSuccessStatusCode) return false;

            var todos = await response.Content.ReadFromJsonAsync<List<TodoItemDto>>();
            
            return todos != null && 
                   todos.Count == 3 && 
                   todos.Any(t => t.Title == "Estudar .NET") &&
                   todos.Any(t => t.Title == "Fazer exercícios") &&
                   todos.Any(t => t.Title == "Ler documentação");
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Testa criação de nova tarefa via API
    /// </summary>
    public async Task<bool> TestCreateTodo()
    {
        try
        {
            var newTodo = new CreateTodoItemDto
            {
                Title = "Teste de API",
                Description = "Tarefa criada via teste de integração"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/todos", newTodo);
            if (!response.IsSuccessStatusCode) return false;

            var createdTodo = await response.Content.ReadFromJsonAsync<TodoItemDto>();
            
            return createdTodo != null && 
                   createdTodo.Title == "Teste de API" &&
                   createdTodo.Description == "Tarefa criada via teste de integração" &&
                   !createdTodo.IsCompleted;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Testa atualização de tarefa via API
    /// </summary>
    public async Task<bool> TestUpdateTodo()
    {
        try
        {
            var updateTodo = new UpdateTodoItemDto
            {
                Title = "Estudar .NET - Atualizado",
                Description = "Descrição atualizada via teste",
                IsCompleted = true
            };

            var response = await _httpClient.PutAsJsonAsync("/api/todos/1", updateTodo);
            if (!response.IsSuccessStatusCode) return false;

            var updatedTodo = await response.Content.ReadFromJsonAsync<TodoItemDto>();
            
            return updatedTodo != null && 
                   updatedTodo.Title == "Estudar .NET - Atualizado" &&
                   updatedTodo.IsCompleted;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Testa toggle de conclusão de tarefa
    /// </summary>
    public async Task<bool> TestToggleTodo()
    {
        try
        {
            // Primeiro, pegar o status atual
            var getResponse = await _httpClient.GetAsync("/api/todos/1");
            if (!getResponse.IsSuccessStatusCode) return false;
            
            var originalTodo = await getResponse.Content.ReadFromJsonAsync<TodoItemDto>();
            var originalStatus = originalTodo!.IsCompleted;

            // Fazer toggle
            var toggleResponse = await _httpClient.PatchAsync("/api/todos/1/toggle", null);
            if (!toggleResponse.IsSuccessStatusCode) return false;

            var toggledTodo = await toggleResponse.Content.ReadFromJsonAsync<TodoItemDto>();
            
            return toggledTodo != null && toggledTodo.IsCompleted != originalStatus;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Testa workflow completo: Create -> Read -> Update -> Delete
    /// </summary>
    public async Task<bool> TestCompleteWorkflow()
    {
        try
        {
            // Create
            var newTodo = new CreateTodoItemDto
            {
                Title = "Workflow Test",
                Description = "Teste de workflow completo"
            };
            
            var createResponse = await _httpClient.PostAsJsonAsync("/api/todos", newTodo);
            if (!createResponse.IsSuccessStatusCode) return false;
            
            var createdTodo = await createResponse.Content.ReadFromJsonAsync<TodoItemDto>();
            if (createdTodo == null) return false;

            // Read
            var readResponse = await _httpClient.GetAsync($"/api/todos/{createdTodo.Id}");
            if (!readResponse.IsSuccessStatusCode) return false;

            // Update
            var updateTodo = new UpdateTodoItemDto
            {
                Title = "Workflow Test - Atualizado",
                Description = "Teste atualizado",
                IsCompleted = true
            };
            
            var updateResponse = await _httpClient.PutAsJsonAsync($"/api/todos/{createdTodo.Id}", updateTodo);
            if (!updateResponse.IsSuccessStatusCode) return false;

            // Delete
            var deleteResponse = await _httpClient.DeleteAsync($"/api/todos/{createdTodo.Id}");
            if (!deleteResponse.IsSuccessStatusCode) return false;

            // Verify deletion
            var verifyResponse = await _httpClient.GetAsync($"/api/todos/{createdTodo.Id}");
            return verifyResponse.StatusCode == System.Net.HttpStatusCode.NotFound;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}