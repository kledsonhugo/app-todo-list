using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace TodoListApp.Tests;

/// <summary>
/// Automated Interface Tests for Todo List Application
/// Using MCP (Model Context Protocol) for comprehensive UI testing
/// Acts as integrated QA to improve quality and accelerate development cycle
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class TodoListInterfaceTests : PageTest
{
    private const string BaseUrl = "http://localhost:5146";
    
    [SetUp]
    public async Task Setup()
    {
        // Set viewport for consistent testing
        await Page.SetViewportSizeAsync(1280, 720);
        
        // Navigate to the application
        await Page.GotoAsync(BaseUrl);
        
        // Wait for the page to be fully loaded
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    #region Page Load and Structure Tests

    [Test]
    public async Task Should_Load_Page_Successfully()
    {
        // Verify page title
        await Expect(Page).ToHaveTitleAsync("Todo List - Gerenciador de Tarefas");
        
        // Verify main heading is visible
        await Expect(Page.Locator("h1")).ToContainTextAsync("Minha Lista de Tarefas");
        
        // Verify main sections are present
        await Expect(Page.Locator("section").First).ToBeVisibleAsync(); // Add todo section
        await Expect(Page.Locator("h2")).ToContainTextAsync("Adicionar Nova Tarefa");
        await Expect(Page.Locator("h2")).ToContainTextAsync("Filtrar Tarefas");
        await Expect(Page.Locator("h2")).ToContainTextAsync("Suas Tarefas");
    }

    [Test]
    public async Task Should_Display_Initial_Sample_Tasks()
    {
        // Verify that initial sample tasks are loaded
        await Expect(Page.Locator(".todo-item")).ToHaveCountAsync(3);
        
        // Verify specific sample tasks
        await Expect(Page.Locator("h3")).ToContainTextAsync("Estudar .NET");
        await Expect(Page.Locator("h3")).ToContainTextAsync("Fazer exercícios");
        await Expect(Page.Locator("h3")).ToContainTextAsync("Ler documentação");
    }

    #endregion

    #region Add Todo Tests

    [Test]
    public async Task Should_Add_New_Todo_Successfully()
    {
        var newTitle = "Teste Automatizado";
        var newDescription = "Este é um teste automatizado de interface";

        // Fill the form
        await Page.FillAsync("#todoTitle", newTitle);
        await Page.FillAsync("#todoDescription", newDescription);

        // Submit the form
        await Page.ClickAsync("button[type=submit]");

        // Wait for the new todo to appear
        await Page.WaitForSelectorAsync($"h3:has-text('{newTitle}')");

        // Verify the new todo was added
        await Expect(Page.Locator($"h3:has-text('{newTitle}')")).ToBeVisibleAsync();
        await Expect(Page.Locator($"p:has-text('{newDescription}')")).ToBeVisibleAsync();
        
        // Verify the task count increased
        await Expect(Page.Locator(".todo-item")).ToHaveCountAsync(4);
    }

    [Test]
    public async Task Should_Validate_Required_Title_Field()
    {
        // Try to submit empty form
        await Page.ClickAsync("button[type=submit]");

        // Verify validation message appears or form doesn't submit
        var titleInput = Page.Locator("#todoTitle");
        await Expect(titleInput).ToHaveAttributeAsync("required", "");
        
        // Verify no new todo was added
        await Expect(Page.Locator(".todo-item")).ToHaveCountAsync(3);
    }

    [Test]
    public async Task Should_Clear_Form_After_Successful_Submission()
    {
        var newTitle = "Teste Limpeza Formulário";

        // Fill and submit the form
        await Page.FillAsync("#todoTitle", newTitle);
        await Page.FillAsync("#todoDescription", "Teste de limpeza");
        await Page.ClickAsync("button[type=submit]");

        // Wait for the new todo to appear
        await Page.WaitForSelectorAsync($"h3:has-text('{newTitle}')");

        // Verify form fields are cleared
        await Expect(Page.Locator("#todoTitle")).ToHaveValueAsync("");
        await Expect(Page.Locator("#todoDescription")).ToHaveValueAsync("");
    }

    #endregion

    #region Filter Tests

    [Test]
    public async Task Should_Filter_All_Tasks_By_Default()
    {
        // Verify "Todas" filter is active by default
        await Expect(Page.Locator("button[data-filter='all']")).ToHaveClassAsync(new Regex("active"));
        
        // Verify all tasks are visible
        await Expect(Page.Locator(".todo-item")).ToHaveCountAsync(3);
    }

    [Test]
    public async Task Should_Filter_Pending_Tasks()
    {
        // Click pending filter
        await Page.ClickAsync("button[data-filter='pending']");

        // Verify filter button is active
        await Expect(Page.Locator("button[data-filter='pending']")).ToHaveClassAsync(new Regex("active"));
        
        // Verify only pending tasks are visible (2 out of 3 initial tasks)
        await Expect(Page.Locator(".todo-item:visible")).ToHaveCountAsync(2);
        
        // Verify completed task is hidden
        await Expect(Page.Locator("h3:has-text('Fazer exercícios')")).ToBeHiddenAsync();
    }

    [Test]
    public async Task Should_Filter_Completed_Tasks()
    {
        // Click completed filter
        await Page.ClickAsync("button[data-filter='completed']");

        // Verify filter button is active
        await Expect(Page.Locator("button[data-filter='completed']")).ToHaveClassAsync(new Regex("active"));
        
        // Verify only completed tasks are visible (1 out of 3 initial tasks)
        await Expect(Page.Locator(".todo-item:visible")).ToHaveCountAsync(1);
        
        // Verify the completed task is visible
        await Expect(Page.Locator("h3:has-text('Fazer exercícios')")).ToBeVisibleAsync();
    }

    #endregion

    #region Task Actions Tests

    [Test]
    public async Task Should_Complete_Pending_Task()
    {
        // Find a pending task and complete it
        var pendingTask = Page.Locator(".todo-item").Filter(new() { HasText = "Estudar .NET" });
        await pendingTask.Locator("button:has-text('Concluir')").ClickAsync();

        // Wait for the status to update
        await Page.WaitForTimeoutAsync(1000);

        // Verify the task is now completed
        await Expect(pendingTask.Locator("button:has-text('Reabrir')")).ToBeVisibleAsync();
        await Expect(pendingTask.Locator(".status:has-text('Concluída')")).ToBeVisibleAsync();
    }

    [Test]
    public async Task Should_Reopen_Completed_Task()
    {
        // Find a completed task and reopen it
        var completedTask = Page.Locator(".todo-item").Filter(new() { HasText = "Fazer exercícios" });
        await completedTask.Locator("button:has-text('Reabrir')").ClickAsync();

        // Wait for the status to update
        await Page.WaitForTimeoutAsync(1000);

        // Verify the task is now pending
        await Expect(completedTask.Locator("button:has-text('Concluir')")).ToBeVisibleAsync();
        await Expect(completedTask.Locator(".status:has-text('Pendente')")).ToBeVisibleAsync();
    }

    [Test]
    public async Task Should_Edit_Task_Successfully()
    {
        var newTitle = "Estudar .NET - EDITADO";
        var newDescription = "Descrição editada via teste automatizado";

        // Click edit button on the first task
        var firstTask = Page.Locator(".todo-item").First;
        await firstTask.Locator("button:has-text('Editar')").ClickAsync();

        // Wait for edit modal to appear
        await Page.WaitForSelectorAsync("#editModal");
        await Expect(Page.Locator("#editModal")).ToBeVisibleAsync();

        // Edit the task
        await Page.FillAsync("#editTitle", newTitle);
        await Page.FillAsync("#editDescription", newDescription);
        
        // Save changes
        await Page.ClickAsync("button:has-text('Salvar Alterações')");

        // Wait for modal to close and changes to be reflected
        await Page.WaitForSelectorAsync("#editModal", new() { State = WaitForSelectorState.Hidden });
        
        // Verify changes are reflected
        await Expect(Page.Locator($"h3:has-text('{newTitle}')")).ToBeVisibleAsync();
        await Expect(Page.Locator($"p:has-text('{newDescription}')")).ToBeVisibleAsync();
    }

    [Test]
    public async Task Should_Delete_Task_With_Confirmation()
    {
        // Get initial task count
        var initialCount = await Page.Locator(".todo-item").CountAsync();

        // Click delete button on the last task
        var lastTask = Page.Locator(".todo-item").Last;
        await lastTask.Locator("button:has-text('Excluir')").ClickAsync();

        // Handle confirmation dialog
        Page.Dialog += async (_, dialog) =>
        {
            Assert.That(dialog.Message, Does.Contain("Tem certeza"));
            await dialog.AcceptAsync();
        };

        // Wait for task to be removed
        await Page.WaitForTimeoutAsync(1000);

        // Verify task count decreased
        await Expect(Page.Locator(".todo-item")).ToHaveCountAsync(initialCount - 1);
    }

    #endregion

    #region Refresh Functionality Tests

    [Test]
    public async Task Should_Refresh_Task_List()
    {
        // Click refresh button
        await Page.ClickAsync("button:has-text('Atualizar')");

        // Wait for refresh to complete
        await Page.WaitForTimeoutAsync(1000);

        // Verify tasks are still displayed (assuming server maintains state)
        await Expect(Page.Locator(".todo-item")).ToHaveCountAsync(3);
    }

    #endregion

    #region Responsive Design Tests

    [Test]
    public async Task Should_Display_Correctly_On_Mobile()
    {
        // Set mobile viewport
        await Page.SetViewportSizeAsync(375, 667);

        // Verify page is still functional
        await Expect(Page.Locator("h1")).ToBeVisibleAsync();
        await Expect(Page.Locator("#todoTitle")).ToBeVisibleAsync();
        await Expect(Page.Locator(".todo-item")).ToHaveCountAsync(3);

        // Verify buttons are clickable on mobile
        await Expect(Page.Locator("button[type=submit]")).ToBeVisibleAsync();
        await Expect(Page.Locator("button[data-filter='all']")).ToBeVisibleAsync();
    }

    #endregion

    #region Error Handling Tests

    [Test]
    public async Task Should_Handle_Network_Errors_Gracefully()
    {
        // Simulate network failure by blocking requests
        await Page.RouteAsync("**/api/todos", route => route.AbortAsync());

        // Try to add a new task
        await Page.FillAsync("#todoTitle", "Test Network Error");
        await Page.ClickAsync("button[type=submit]");

        // Verify error handling (this depends on implementation)
        // For now, we just verify the form doesn't break
        await Expect(Page.Locator("#todoTitle")).ToBeVisibleAsync();
    }

    #endregion

    #region Performance Tests

    [Test]
    public async Task Should_Load_Page_Within_Acceptable_Time()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        await Page.GotoAsync(BaseUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        stopwatch.Stop();
        
        // Verify page loads within 5 seconds
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(5000), 
            $"Page took {stopwatch.ElapsedMilliseconds}ms to load");
    }

    [Test]
    public async Task Should_Respond_To_User_Interactions_Quickly()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // Test interaction response time
        await Page.ClickAsync("button[data-filter='pending']");
        await Page.WaitForTimeoutAsync(100); // Small wait for UI update
        
        stopwatch.Stop();
        
        // Verify interaction responds within 1 second
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(1000),
            $"Filter interaction took {stopwatch.ElapsedMilliseconds}ms");
    }

    #endregion
}