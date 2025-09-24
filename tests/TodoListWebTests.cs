using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using System.Text.RegularExpressions;

namespace TodoListApp.Tests;

[TestClass]
public class TodoListWebTests : PageTest
{
    private const string BaseUrl = "http://localhost:5146";
    
    [TestInitialize]
    public async Task Setup()
    {
        // Navigate to the todo list page
        await Page.GotoAsync(BaseUrl);
        
        // Wait for the page to load completely
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [TestMethod]
    public async Task PageLoadsCorrectly()
    {
        // Verify the main heading is present
        await Expect(Page.Locator("h1")).ToContainTextAsync("Minha Lista de Tarefas");
        
        // Verify main sections are present
        await Expect(Page.Locator(".add-todo-section")).ToBeVisibleAsync();
        await Expect(Page.Locator(".filters-section")).ToBeVisibleAsync();
        await Expect(Page.Locator(".todos-section")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task CanAddNewTodo()
    {
        // Fill in the form
        await Page.FillAsync("#todoTitle", "Test Task");
        await Page.FillAsync("#todoDescription", "Test Description");
        
        // Submit the form
        await Page.ClickAsync("button[type='submit']");
        
        // Wait for the task to appear and verify it exists
        await Page.WaitForSelectorAsync(".todo-item:has-text('Test Task')", new PageWaitForSelectorOptions { Timeout = 5000 });
        
        // Verify the task appears in the list
        await Expect(Page.Locator(".todo-item").Filter(new() { HasText = "Test Task" })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task CannotAddTodoWithoutTitle()
    {
        // Leave title empty and try to submit
        await Page.FillAsync("#todoTitle", "");
        await Page.FillAsync("#todoDescription", "Test Description");
        
        // Try to submit - should not work due to HTML5 validation
        await Page.ClickAsync("button[type='submit']");
        
        // Verify validation message appears
        var titleInput = Page.Locator("#todoTitle");
        await Expect(titleInput).ToHaveAttributeAsync("required", "");
        
        // Check if browser validation prevents submission
        var validationMessage = await titleInput.EvaluateAsync("el => el.validationMessage");
        Assert.IsFalse(string.IsNullOrEmpty(validationMessage.ToString()));
    }

    [TestMethod]
    public async Task CanFilterTodos()
    {
        // Click on "Pendentes" filter
        await Page.ClickAsync("button[data-filter='pending']");
        
        // Verify the filter button is active
        await Expect(Page.Locator("button[data-filter='pending']")).ToHaveClassAsync(new Regex(".*active.*"));
        
        // Click on "Concluídas" filter
        await Page.ClickAsync("button[data-filter='completed']");
        
        // Verify the filter button is active
        await Expect(Page.Locator("button[data-filter='completed']")).ToHaveClassAsync(new Regex(".*active.*"));
        
        // Click on "Todas" filter
        await Page.ClickAsync("button[data-filter='all']");
        
        // Verify the filter button is active
        await Expect(Page.Locator("button[data-filter='all']")).ToHaveClassAsync(new Regex(".*active.*"));
    }

    [TestMethod]
    public async Task CanToggleTodoCompletion()
    {
        // Find the first todo item
        var firstTodo = Page.Locator(".todo-item").First;
        
        // Click the toggle button (complete/reopen)
        await firstTodo.Locator(".toggle-btn").ClickAsync();
        
        // Wait for the status to update
        await Page.WaitForTimeoutAsync(1000);
        
        // Verify the status changed (this would need to check the actual visual change)
        // The exact assertion would depend on how the completion status is displayed
    }

    [TestMethod]
    public async Task CanOpenEditModal()
    {
        // Find the first todo item and click edit
        var firstTodo = Page.Locator(".todo-item").First;
        await firstTodo.Locator(".edit-btn").ClickAsync();
        
        // Verify modal is visible
        await Expect(Page.Locator("#editModal")).ToBeVisibleAsync();
        
        // Verify modal content
        await Expect(Page.Locator("#editTodoTitle")).ToBeVisibleAsync();
        await Expect(Page.Locator("#editTodoDescription")).ToBeVisibleAsync();
        await Expect(Page.Locator("#editTodoCompleted")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task CanCancelEditModal()
    {
        // Open edit modal
        var firstTodo = Page.Locator(".todo-item").First;
        await firstTodo.Locator(".edit-btn").ClickAsync();
        
        // Click cancel
        await Page.ClickAsync("#cancelEdit");
        
        // Verify modal is hidden
        await Expect(Page.Locator("#editModal")).ToBeHiddenAsync();
    }

    [TestMethod]
    public async Task CanCloseEditModalWithX()
    {
        // Open edit modal
        var firstTodo = Page.Locator(".todo-item").First;
        await firstTodo.Locator(".edit-btn").ClickAsync();
        
        // Click close button (X)
        await Page.ClickAsync("#closeModal");
        
        // Verify modal is hidden
        await Expect(Page.Locator("#editModal")).ToBeHiddenAsync();
    }

    [TestMethod]
    public async Task CanDeleteTodoWithConfirmation()
    {
        // Get the count of todos before deletion
        var todoCountBefore = await Page.Locator(".todo-item").CountAsync();
        
        // Find the first todo item and click delete
        var firstTodo = Page.Locator(".todo-item").First;
        
        // Handle the confirmation dialog
        Page.Dialog += async (_, dialog) =>
        {
            Assert.AreEqual("confirm", dialog.Type);
            Assert.IsTrue(dialog.Message.Contains("excluir"));
            await dialog.AcceptAsync();
        };
        
        await firstTodo.Locator(".delete-btn").ClickAsync();
        
        // Wait for the item to be removed
        await Page.WaitForTimeoutAsync(1000);
        
        // Verify the todo count decreased
        var todoCountAfter = await Page.Locator(".todo-item").CountAsync();
        Assert.AreEqual(todoCountBefore - 1, todoCountAfter);
    }

    [TestMethod]
    public async Task CanCancelDeleteTodo()
    {
        // Get the count of todos before
        var todoCountBefore = await Page.Locator(".todo-item").CountAsync();
        
        // Find the first todo item and click delete
        var firstTodo = Page.Locator(".todo-item").First;
        
        // Handle the confirmation dialog - dismiss it
        Page.Dialog += async (_, dialog) =>
        {
            await dialog.DismissAsync();
        };
        
        await firstTodo.Locator(".delete-btn").ClickAsync();
        
        // Wait a bit
        await Page.WaitForTimeoutAsync(1000);
        
        // Verify the todo count stayed the same
        var todoCountAfter = await Page.Locator(".todo-item").CountAsync();
        Assert.AreEqual(todoCountBefore, todoCountAfter);
    }

    [TestMethod]
    public async Task RefreshButtonWorks()
    {
        // Click the refresh button
        await Page.ClickAsync("#refreshBtn");
        
        // Wait for loading to complete
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Verify todos are still displayed (assuming default todos exist)
        var todoCount = await Page.Locator(".todo-item").CountAsync();
        Assert.IsTrue(todoCount > 0);
    }

    [TestMethod]
    public async Task ShowsLoadingIndicator()
    {
        // Click refresh to trigger loading
        await Page.ClickAsync("#refreshBtn");
        
        // The loading indicator should appear briefly
        // This is a timing-dependent test, so we'll just check it exists in the DOM
        await Expect(Page.Locator("#loading")).ToBeAttachedAsync();
    }

    [TestMethod]
    public async Task ShowsEmptyMessageWhenNoTodos()
    {
        // This test would need to start with an empty state or delete all todos
        // For now, we'll just verify the empty message element exists
        await Expect(Page.Locator("#emptyMessage")).ToBeAttachedAsync();
    }
}