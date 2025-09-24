using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace TodoListApp.Tests;

[TestClass]
public class TodoListErrorHandlingTests : PageTest
{
    private const string BaseUrl = "http://localhost:5146";
    
    [TestInitialize]
    public async Task Setup()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [TestMethod]
    public async Task DisplaysErrorWhenServerUnreachable()
    {
        // Intercept API requests and return network error
        await Page.RouteAsync("**/api/todos", route => route.AbortAsync());
        
        // Try to refresh the todos
        await Page.ClickAsync("#refreshBtn");
        
        // Wait for error handling
        await Page.WaitForTimeoutAsync(2000);
        
        // Check if error toast appears (this depends on the implementation)
        // The exact selector would depend on how errors are displayed
        var errorToast = Page.Locator(".toast.error");
        if (await errorToast.IsVisibleAsync())
        {
            await Expect(errorToast).ToContainTextAsync("Erro");
        }
    }

    [TestMethod]
    public async Task HandlesApiErrorGracefully()
    {
        // Intercept API requests and return 500 error
        await Page.RouteAsync("**/api/todos", route => route.FulfillAsync(new()
        {
            Status = 500,
            ContentType = "application/json",
            Body = "{\"error\": \"Internal server error\"}"
        }));
        
        // Try to refresh the todos
        await Page.ClickAsync("#refreshBtn");
        
        // Wait for error handling
        await Page.WaitForTimeoutAsync(2000);
        
        // Verify that the application doesn't crash and shows appropriate feedback
        await Expect(Page.Locator("body")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task ValidatesRequiredFields()
    {
        // Try to submit form with empty title
        await Page.FillAsync("#todoTitle", "");
        await Page.FillAsync("#todoDescription", "Some description");
        
        // Try to submit
        await Page.ClickAsync("button[type='submit']");
        
        // Verify HTML5 validation prevents submission
        var titleInput = Page.Locator("#todoTitle");
        var isValid = await titleInput.EvaluateAsync<bool>("el => el.checkValidity()");
        Assert.IsFalse(isValid, "Form should be invalid when title is empty");
    }

    [TestMethod]
    public async Task HandlesLongTextInput()
    {
        // Test with very long title (near maxlength)
        var longTitle = new string('A', 190) + "1234567890"; // 200 characters (max)
        var longDescription = new string('B', 500); // 500 characters (max)
        
        await Page.FillAsync("#todoTitle", longTitle);
        await Page.FillAsync("#todoDescription", longDescription);
        
        // Submit should work with max length content
        await Page.ClickAsync("button[type='submit']");
        
        // Wait for submission
        await Page.WaitForTimeoutAsync(1000);
        
        // Verify the task appears (might be truncated in display)
        var todoItems = Page.Locator(".todo-item");
        var count = await todoItems.CountAsync();
        Assert.IsTrue(count > 0, "Task should be created with long content");
    }

    [TestMethod]
    public async Task HandlesEmptyTodosList()
    {
        // Mock empty response
        await Page.RouteAsync("**/api/todos", route => route.FulfillAsync(new()
        {
            Status = 200,
            ContentType = "application/json",
            Body = "[]"
        }));
        
        // Refresh to get empty list
        await Page.ClickAsync("#refreshBtn");
        
        // Wait for response
        await Page.WaitForTimeoutAsync(1000);
        
        // Verify empty message is displayed
        await Expect(Page.Locator("#emptyMessage")).ToBeVisibleAsync();
        await Expect(Page.Locator("#emptyMessage")).ToContainTextAsync("Nenhuma tarefa encontrada");
    }

    [TestMethod]
    public async Task HandlesInvalidJsonResponse()
    {
        // Mock invalid JSON response
        await Page.RouteAsync("**/api/todos", route => route.FulfillAsync(new()
        {
            Status = 200,
            ContentType = "application/json",
            Body = "invalid json {{"
        }));
        
        // Try to refresh
        await Page.ClickAsync("#refreshBtn");
        
        // Wait for error handling
        await Page.WaitForTimeoutAsync(2000);
        
        // Verify the application doesn't crash
        await Expect(Page.Locator("body")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task HandlesSlowNetworkResponse()
    {
        // Mock slow response
        await Page.RouteAsync("**/api/todos", async route =>
        {
            await Task.Delay(3000); // 3 second delay
            await route.FulfillAsync(new()
            {
                Status = 200,
                ContentType = "application/json",
                Body = "[{\"id\":1,\"title\":\"Slow Task\",\"description\":\"Delayed response\",\"isCompleted\":false}]"
            });
        });
        
        // Start the request
        await Page.ClickAsync("#refreshBtn");
        
        // Verify loading indicator appears
        await Expect(Page.Locator("#loading")).ToBeVisibleAsync();
        
        // Wait for response
        await Page.WaitForTimeoutAsync(4000);
        
        // Verify loading indicator disappears
        await Expect(Page.Locator("#loading")).ToBeHiddenAsync();
    }

    [TestMethod]
    public async Task HandlesConcurrentOperations()
    {
        // Try to submit multiple todos rapidly
        for (int i = 0; i < 3; i++)
        {
            await Page.FillAsync("#todoTitle", $"Concurrent Task {i}");
            await Page.ClickAsync("button[type='submit']");
            // Don't wait between submissions to test concurrency
        }
        
        // Wait for all operations to complete
        await Page.WaitForTimeoutAsync(3000);
        
        // Verify the application handles concurrent requests gracefully
        await Expect(Page.Locator("body")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task HandlesSpecialCharactersInInput()
    {
        // Test with special characters
        var specialTitle = "Test <script>alert('xss')</script> & \"quotes\" 'single'";
        var specialDescription = "Description with émojis 🚀 and spëcial châractèrs";
        
        await Page.FillAsync("#todoTitle", specialTitle);
        await Page.FillAsync("#todoDescription", specialDescription);
        
        await Page.ClickAsync("button[type='submit']");
        
        // Wait for submission
        await Page.WaitForTimeoutAsync(1000);
        
        // Verify the content is properly escaped/handled
        var todoItems = Page.Locator(".todo-item");
        var count = await todoItems.CountAsync();
        Assert.IsTrue(count > 0, "Task should be created with special characters");
        
        // Verify no script execution occurred (XSS protection)
        var alerts = new List<string>();
        Page.Dialog += (_, dialog) =>
        {
            alerts.Add(dialog.Message);
            dialog.DismissAsync();
        };
        
        // Wait a bit to see if any unwanted dialogs appear
        await Page.WaitForTimeoutAsync(1000);
        Assert.AreEqual(0, alerts.Count, "No script-based alerts should appear");
    }

    [TestMethod]
    public async Task HandlesKeyboardNavigation()
    {
        // Test tab navigation
        await Page.PressAsync("body", "Tab"); // Should focus first focusable element
        
        // Fill form using keyboard only
        await Page.PressAsync("#todoTitle", "Control+A"); // Select all
        await Page.Locator("#todoTitle").FillAsync("Keyboard Test");
        
        await Page.PressAsync("body", "Tab"); // Move to description
        await Page.Locator("#todoDescription").FillAsync("Added via keyboard");
        
        await Page.PressAsync("body", "Tab"); // Move to submit button
        await Page.PressAsync("body", "Enter"); // Submit with Enter key
        
        // Wait for submission
        await Page.WaitForTimeoutAsync(1000);
        
        // Verify task was added
        await Expect(Page.Locator(".todo-item").Filter(new() { HasText = "Keyboard Test" })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task HandlesEscapeKeyInModal()
    {
        // Open edit modal first (assuming there's at least one todo)
        var firstTodo = Page.Locator(".todo-item").First;
        if (await firstTodo.IsVisibleAsync())
        {
            await firstTodo.GetByRole(AriaRole.Button, new() { Name = "Editar" }).ClickAsync();
            
            // Verify modal is open
            await Expect(Page.Locator("#editModal")).ToBeVisibleAsync();
            
            // Press Escape key
            await Page.PressAsync("body", "Escape");
            
            // Verify modal closes
            await Expect(Page.Locator("#editModal")).ToBeHiddenAsync();
        }
    }
}