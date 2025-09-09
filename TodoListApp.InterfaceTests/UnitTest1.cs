namespace TodoListApp.InterfaceTests;

[TestClass]
public class TodoInterfaceTests
{
    private static WebApplicationFactory<Program>? _factory;
    private static HttpClient? _client;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [TestMethod]
    public async Task WebPageLoadsSuccessfully()
    {
        // Test that the main page loads successfully
        var response = await _client!.GetAsync("/");
        
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        
        // Check that the HTML contains expected elements for the interface
        Assert.IsTrue(content.Contains("Todo List - Gerenciador de Tarefas"), "Page should have correct title");
        Assert.IsTrue(content.Contains("Minha Lista de Tarefas"), "Page should have main heading");
        Assert.IsTrue(content.Contains("addTodoForm"), "Page should have add todo form");
        Assert.IsTrue(content.Contains("todosList"), "Page should have todos list container");
        Assert.IsTrue(content.Contains("editModal"), "Page should have edit modal");
    }

    [TestMethod]
    public async Task StaticResourcesLoadCorrectly()
    {
        // Test that CSS and JavaScript files are served correctly for the interface
        var cssResponse = await _client!.GetAsync("/styles.css");
        Assert.AreEqual(HttpStatusCode.OK, cssResponse.StatusCode);
        
        var jsResponse = await _client.GetAsync("/script.js");
        Assert.AreEqual(HttpStatusCode.OK, jsResponse.StatusCode);
        
        // Verify JavaScript contains interface functionality
        var jsContent = await jsResponse.Content.ReadAsStringAsync();
        Assert.IsTrue(jsContent.Contains("loadTodos"), "JavaScript should contain loadTodos function");
        Assert.IsTrue(jsContent.Contains("createTodo"), "JavaScript should contain createTodo function");
        Assert.IsTrue(jsContent.Contains("handleAddTodo"), "JavaScript should contain handleAddTodo function");
        Assert.IsTrue(jsContent.Contains("API_BASE_URL"), "JavaScript should contain API configuration");
    }

    [TestMethod]
    public async Task ApiEndpointsRespondCorrectlyForInterface()
    {
        // Test that the API endpoints used by the interface work correctly
        
        // Test GET /api/todos - used by interface to load todos
        var todosResponse = await _client!.GetAsync("/api/todos");
        Assert.AreEqual(HttpStatusCode.OK, todosResponse.StatusCode);
        
        var todosContent = await todosResponse.Content.ReadAsStringAsync();
        var todos = JsonSerializer.Deserialize<TodoItemDto[]>(todosContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.IsNotNull(todos);
        Assert.AreEqual(3, todos.Length, "Should have 3 initial todos");
    }

    [TestMethod]
    public async Task InterfaceCanCreateTodo()
    {
        // Test the POST endpoint used by the interface to create todos
        var newTodo = new
        {
            Title = "Interface Test Todo",
            Description = "Created via interface test"
        };
        
        var json = JsonSerializer.Serialize(newTodo);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _client!.PostAsync("/api/todos", stringContent);
        
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var createdTodo = JsonSerializer.Deserialize<TodoItemDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.IsNotNull(createdTodo);
        Assert.AreEqual("Interface Test Todo", createdTodo.Title);
        Assert.AreEqual("Created via interface test", createdTodo.Description);
        Assert.IsFalse(createdTodo.IsCompleted);
    }

    [TestMethod]
    public async Task InterfaceCanUpdateTodo()
    {
        // Test the PUT endpoint used by the interface to update todos
        
        // First create a todo
        var newTodo = new { Title = "Update Test", Description = "To be updated" };
        var json = JsonSerializer.Serialize(newTodo);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        var createResponse = await _client!.PostAsync("/api/todos", stringContent);
        
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdTodo = JsonSerializer.Deserialize<TodoItemDto>(createContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        // Now update it
        var updatedTodo = new
        {
            Title = "Updated via Interface",
            Description = "Description updated",
            IsCompleted = true
        };
        
        var updateJson = JsonSerializer.Serialize(updatedTodo);
        var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");
        
        var updateResponse = await _client.PutAsync($"/api/todos/{createdTodo!.Id}", updateContent);
        
        Assert.AreEqual(HttpStatusCode.OK, updateResponse.StatusCode);
        
        var updateResponseContent = await updateResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TodoItemDto>(updateResponseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.IsNotNull(result);
        Assert.AreEqual("Updated via Interface", result.Title);
        Assert.IsTrue(result.IsCompleted);
    }

    [TestMethod]
    public async Task InterfaceCanToggleTodoCompletion()
    {
        // Test the PATCH endpoint used by the interface to toggle completion
        
        // Get existing todos
        var todosResponse = await _client!.GetAsync("/api/todos");
        var todosContent = await todosResponse.Content.ReadAsStringAsync();
        var todos = JsonSerializer.Deserialize<TodoItemDto[]>(todosContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        var firstTodo = todos![0];
        var originalStatus = firstTodo.IsCompleted;
        
        // Toggle completion
        var toggleResponse = await _client.PatchAsync($"/api/todos/{firstTodo.Id}/toggle", null);
        
        Assert.AreEqual(HttpStatusCode.OK, toggleResponse.StatusCode);
        
        var toggleContent = await toggleResponse.Content.ReadAsStringAsync();
        var toggledTodo = JsonSerializer.Deserialize<TodoItemDto>(toggleContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.IsNotNull(toggledTodo);
        Assert.AreEqual(!originalStatus, toggledTodo.IsCompleted, "Status should be toggled");
    }

    [TestMethod]
    public async Task InterfaceCanDeleteTodo()
    {
        // Test the DELETE endpoint used by the interface
        
        // First create a todo to delete
        var newTodo = new { Title = "To Delete", Description = "Will be deleted" };
        var json = JsonSerializer.Serialize(newTodo);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        var createResponse = await _client!.PostAsync("/api/todos", stringContent);
        
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdTodo = JsonSerializer.Deserialize<TodoItemDto>(createContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        // Now delete it
        var deleteResponse = await _client.DeleteAsync($"/api/todos/{createdTodo!.Id}");
        
        Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        
        // Verify it's gone
        var getResponse = await _client.GetAsync($"/api/todos/{createdTodo.Id}");
        Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [TestMethod]
    public async Task InterfaceHandlesErrorsCorrectly()
    {
        // Test error scenarios that the interface needs to handle
        
        // Test 404 for non-existent todo
        var notFoundResponse = await _client!.GetAsync("/api/todos/999999");
        Assert.AreEqual(HttpStatusCode.NotFound, notFoundResponse.StatusCode);
        
        // Test validation error for empty title
        var invalidTodo = new { Description = "No title" };
        var json = JsonSerializer.Serialize(invalidTodo);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        
        var badRequestResponse = await _client.PostAsync("/api/todos", stringContent);
        Assert.AreEqual(HttpStatusCode.BadRequest, badRequestResponse.StatusCode);
    }

    [TestMethod]
    public async Task EndToEndInterfaceWorkflow()
    {
        // Test a complete workflow that the interface would perform
        
        // 1. Load initial page (already tested above)
        var pageResponse = await _client!.GetAsync("/");
        Assert.AreEqual(HttpStatusCode.OK, pageResponse.StatusCode);
        
        // 2. Get initial todos (interface loads these on page load)
        var initialTodosResponse = await _client.GetAsync("/api/todos");
        var initialTodosContent = await initialTodosResponse.Content.ReadAsStringAsync();
        var initialTodos = JsonSerializer.Deserialize<TodoItemDto[]>(initialTodosContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        var initialCount = initialTodos!.Length;
        
        // 3. Create a new todo (user fills form and submits)
        var newTodo = new { Title = "E2E Test Todo", Description = "End-to-end test" };
        var json = JsonSerializer.Serialize(newTodo);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/todos", stringContent);
        
        Assert.AreEqual(HttpStatusCode.Created, createResponse.StatusCode);
        var createdTodo = JsonSerializer.Deserialize<TodoItemDto>(
            await createResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        // 4. Verify the todo appears in the list (interface refreshes)
        var updatedTodosResponse = await _client.GetAsync("/api/todos");
        var updatedTodosContent = await updatedTodosResponse.Content.ReadAsStringAsync();
        var updatedTodos = JsonSerializer.Deserialize<TodoItemDto[]>(updatedTodosContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.AreEqual(initialCount + 1, updatedTodos!.Length, "Todo count should increase");
        Assert.IsTrue(updatedTodos.Any(t => t.Title == "E2E Test Todo"), "New todo should be in list");
        
        // 5. Edit the todo (user clicks edit, modifies, and saves)
        var editedTodo = new
        {
            Title = "E2E Test Todo - Edited",
            Description = "Edited description",
            IsCompleted = false
        };
        
        var editJson = JsonSerializer.Serialize(editedTodo);
        var editContent = new StringContent(editJson, Encoding.UTF8, "application/json");
        var editResponse = await _client.PutAsync($"/api/todos/{createdTodo!.Id}", editContent);
        
        Assert.AreEqual(HttpStatusCode.OK, editResponse.StatusCode);
        
        // 6. Toggle completion (user clicks toggle button)
        var toggleResponse = await _client.PatchAsync($"/api/todos/{createdTodo.Id}/toggle", null);
        Assert.AreEqual(HttpStatusCode.OK, toggleResponse.StatusCode);
        
        // 7. Delete the todo (user clicks delete and confirms)
        var deleteResponse = await _client.DeleteAsync($"/api/todos/{createdTodo.Id}");
        Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        
        // 8. Verify final state matches initial state
        var finalTodosResponse = await _client.GetAsync("/api/todos");
        var finalTodosContent = await finalTodosResponse.Content.ReadAsStringAsync();
        var finalTodos = JsonSerializer.Deserialize<TodoItemDto[]>(finalTodosContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.AreEqual(initialCount, finalTodos!.Length, "Should return to initial count");
    }
}

// DTO class for JSON serialization (matches the interface expectations)
public class TodoItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}