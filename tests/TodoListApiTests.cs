using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using NUnit.Framework;
using TodoListApp.DTOs;

namespace TodoListApp.Tests;

/// <summary>
/// API Integration Tests for Todo List Application
/// Validates the backend API that supports the web interface
/// Part of the comprehensive MCP testing strategy
/// </summary>
[TestFixture]
public class TodoListApiTests
{
    private HttpClient _client = null!;
    private TodoListWebApplicationFactory _factory = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _factory = new TodoListWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    #region GET Tests

    [Test]
    public async Task Get_AllTodos_Should_Return_Initial_Sample_Data()
    {
        // Act
        var response = await _client.GetAsync("/api/todos");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var todos = await response.Content.ReadFromJsonAsync<List<TodoItemDto>>();
        Assert.That(todos, Is.Not.Null);
        Assert.That(todos!.Count, Is.EqualTo(TestHelpers.ExpectedData.InitialTodoCount));
        
        // Verify expected initial todos
        var titles = todos.Select(t => t.Title).ToList();
        foreach (var expectedTitle in TestHelpers.ExpectedData.InitialTodoTitles)
        {
            Assert.That(titles, Contains.Item(expectedTitle));
        }
    }

    [Test]
    public async Task Get_TodoById_Should_Return_Specific_Todo()
    {
        // Arrange - Get all todos first to get a valid ID
        var allTodosResponse = await _client.GetAsync("/api/todos");
        var todos = await allTodosResponse.Content.ReadFromJsonAsync<List<TodoItemDto>>();
        var firstTodo = todos!.First();

        // Act
        var response = await _client.GetAsync($"/api/todos/{firstTodo.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var todo = await response.Content.ReadFromJsonAsync<TodoItemDto>();
        Assert.That(todo, Is.Not.Null);
        Assert.That(todo!.Id, Is.EqualTo(firstTodo.Id));
        Assert.That(todo.Title, Is.EqualTo(firstTodo.Title));
    }

    [Test]
    public async Task Get_NonExistentTodo_Should_Return_NotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/todos/999");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    #endregion

    #region POST Tests

    [Test]
    public async Task Post_NewTodo_Should_Create_Successfully()
    {
        // Arrange
        var (title, description) = TestHelpers.GenerateTestTodoData("API_Test");
        var createDto = new CreateTodoItemDto
        {
            Title = title,
            Description = description
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/todos", createDto);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        
        var createdTodo = await response.Content.ReadFromJsonAsync<TodoItemDto>();
        Assert.That(createdTodo, Is.Not.Null);
        Assert.That(createdTodo!.Title, Is.EqualTo(title));
        Assert.That(createdTodo.Description, Is.EqualTo(description));
        Assert.That(createdTodo.IsCompleted, Is.False);
        Assert.That(createdTodo.Id, Is.GreaterThan(0));

        // Verify location header
        Assert.That(response.Headers.Location, Is.Not.Null);
        Assert.That(response.Headers.Location!.ToString(), Does.Contain($"/api/todos/{createdTodo.Id}"));
    }

    [Test]
    public async Task Post_TodoWithoutTitle_Should_Return_BadRequest()
    {
        // Arrange
        var createDto = new CreateTodoItemDto
        {
            Title = "", // Empty title should be invalid
            Description = "Test description"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/todos", createDto);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    #endregion

    #region PUT Tests

    [Test]
    public async Task Put_ExistingTodo_Should_Update_Successfully()
    {
        // Arrange - Create a todo first
        var (originalTitle, originalDescription) = TestHelpers.GenerateTestTodoData("PUT_Test_Original");
        var createDto = new CreateTodoItemDto
        {
            Title = originalTitle,
            Description = originalDescription
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/todos", createDto);
        var createdTodo = await createResponse.Content.ReadFromJsonAsync<TodoItemDto>();

        // Update data
        var (updatedTitle, updatedDescription) = TestHelpers.GenerateTestTodoData("PUT_Test_Updated");
        var updateDto = new UpdateTodoItemDto
        {
            Title = updatedTitle,
            Description = updatedDescription,
            IsCompleted = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/todos/{createdTodo!.Id}", updateDto);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var updatedTodo = await response.Content.ReadFromJsonAsync<TodoItemDto>();
        Assert.That(updatedTodo, Is.Not.Null);
        Assert.That(updatedTodo!.Title, Is.EqualTo(updatedTitle));
        Assert.That(updatedTodo.Description, Is.EqualTo(updatedDescription));
        Assert.That(updatedTodo.IsCompleted, Is.True);
        Assert.That(updatedTodo.CompletedAt, Is.Not.Null);
    }

    [Test]
    public async Task Put_NonExistentTodo_Should_Return_NotFound()
    {
        // Arrange
        var updateDto = new UpdateTodoItemDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            IsCompleted = false
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/todos/999", updateDto);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    #endregion

    #region PATCH Tests

    [Test]
    public async Task Patch_ToggleTodo_Should_Change_Completion_Status()
    {
        // Arrange - Get an existing todo
        var allTodosResponse = await _client.GetAsync("/api/todos");
        var todos = await allTodosResponse.Content.ReadFromJsonAsync<List<TodoItemDto>>();
        var targetTodo = todos!.First(t => !t.IsCompleted); // Get a pending todo
        var originalStatus = targetTodo.IsCompleted;

        // Act
        var response = await _client.PatchAsync($"/api/todos/{targetTodo.Id}/toggle", null);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var toggledTodo = await response.Content.ReadFromJsonAsync<TodoItemDto>();
        Assert.That(toggledTodo, Is.Not.Null);
        Assert.That(toggledTodo!.IsCompleted, Is.Not.EqualTo(originalStatus));
        
        if (toggledTodo.IsCompleted)
        {
            Assert.That(toggledTodo.CompletedAt, Is.Not.Null);
        }
        else
        {
            Assert.That(toggledTodo.CompletedAt, Is.Null);
        }
    }

    [Test]
    public async Task Patch_ToggleNonExistentTodo_Should_Return_NotFound()
    {
        // Act
        var response = await _client.PatchAsync("/api/todos/999/toggle", null);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    #endregion

    #region DELETE Tests

    [Test]
    public async Task Delete_ExistingTodo_Should_Remove_Successfully()
    {
        // Arrange - Create a todo to delete
        var (title, description) = TestHelpers.GenerateTestTodoData("DELETE_Test");
        var createDto = new CreateTodoItemDto
        {
            Title = title,
            Description = description
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/todos", createDto);
        var createdTodo = await createResponse.Content.ReadFromJsonAsync<TodoItemDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/todos/{createdTodo!.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        // Verify the todo was actually deleted
        var getResponse = await _client.GetAsync($"/api/todos/{createdTodo.Id}");
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Delete_NonExistentTodo_Should_Return_NotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/todos/999");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    #endregion

    #region Validation Tests

    [Test]
    public async Task Api_Should_Validate_Required_Fields()
    {
        // Test null title
        var invalidDto = new { title = (string?)null, description = "Test" };
        var response = await _client.PostAsJsonAsync("/api/todos", invalidDto);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        // Test empty title
        var emptyDto = new { title = "", description = "Test" };
        response = await _client.PostAsJsonAsync("/api/todos", emptyDto);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Api_Should_Handle_Long_Titles_And_Descriptions()
    {
        // Test very long title (assuming 200 char limit)
        var longTitle = new string('A', 201);
        var dto = new CreateTodoItemDto
        {
            Title = longTitle,
            Description = "Test"
        };
        
        var response = await _client.PostAsJsonAsync("/api/todos", dto);
        // This should either be BadRequest (validation) or OK (truncated)
        Assert.That(response.StatusCode, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.Created));
    }

    #endregion

    #region Content Type Tests

    [Test]
    public async Task Api_Should_Return_Json_Content_Type()
    {
        // Act
        var response = await _client.GetAsync("/api/todos");

        // Assert
        Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
    }

    [Test]
    public async Task Api_Should_Accept_Json_Content_Type()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new { title = "Test", description = "Test" });
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/todos", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    #endregion
}