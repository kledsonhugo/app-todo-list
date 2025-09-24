using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using System.Text.RegularExpressions;

namespace TodoListApp.Tests;

[TestClass]
public class TodoListResponsiveTests : PageTest
{
    private const string BaseUrl = "http://localhost:5146";
    
    [TestInitialize]
    public async Task Setup()
    {
        // Navigate to the todo list page
        await Page.GotoAsync(BaseUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [TestMethod]
    public async Task DesktopLayoutDisplaysCorrectly()
    {
        // Set desktop viewport
        await Page.SetViewportSizeAsync(1200, 800);
        
        // Verify header is visible
        await Expect(Page.Locator("header h1")).ToBeVisibleAsync();
        
        // Verify all main sections are visible
        await Expect(Page.Locator(".add-todo-section")).ToBeVisibleAsync();
        await Expect(Page.Locator(".filters-section")).ToBeVisibleAsync();
        await Expect(Page.Locator(".todos-section")).ToBeVisibleAsync();
        
        // Verify filter buttons are in a horizontal layout
        var filterButtons = Page.Locator(".filter-buttons");
        await Expect(filterButtons).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task TabletLayoutDisplaysCorrectly()
    {
        // Set tablet viewport
        await Page.SetViewportSizeAsync(768, 1024);
        
        // Verify the page adapts to tablet size
        await Expect(Page.Locator("header h1")).ToBeVisibleAsync();
        await Expect(Page.Locator(".add-todo-section")).ToBeVisibleAsync();
        
        // Check that content is still accessible
        var todoForm = Page.Locator("#addTodoForm");
        await Expect(todoForm).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task MobileLayoutDisplaysCorrectly()
    {
        // Set mobile viewport
        await Page.SetViewportSizeAsync(375, 667);
        
        // Verify the page adapts to mobile size
        await Expect(Page.Locator("header h1")).ToBeVisibleAsync();
        
        // Verify form elements are still usable
        await Expect(Page.Locator("#todoTitle")).ToBeVisibleAsync();
        await Expect(Page.Locator("#todoDescription")).ToBeVisibleAsync();
        
        // Verify buttons are appropriately sized
        var addButton = Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar Tarefa" });
        await Expect(addButton).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task SmallMobileLayoutDisplaysCorrectly()
    {
        // Set very small mobile viewport
        await Page.SetViewportSizeAsync(320, 568);
        
        // Verify essential elements are still visible and functional
        await Expect(Page.Locator("header h1")).ToBeVisibleAsync();
        await Expect(Page.Locator("#todoTitle")).ToBeVisibleAsync();
        
        // Verify the layout doesn't break
        var container = Page.Locator(".container");
        await Expect(container).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task ResponsiveFormInteractionWorks()
    {
        // Test form interaction across different screen sizes
        var viewports = new[]
        {
            new { Width = 1200, Height = 800 }, // Desktop
            new { Width = 768, Height = 1024 },  // Tablet
            new { Width = 375, Height = 667 }    // Mobile
        };

        foreach (var viewport in viewports)
        {
            await Page.SetViewportSizeAsync(viewport.Width, viewport.Height);
            
            // Verify form can be filled and submitted
            await Page.FillAsync("#todoTitle", $"Test Task {viewport.Width}px");
            await Page.FillAsync("#todoDescription", "Responsive test description");
            
            // Verify submit button is clickable
            var submitButton = Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar Tarefa" });
            await Expect(submitButton).ToBeVisibleAsync();
            await Expect(submitButton).ToBeEnabledAsync();
            
            // Click submit
            await submitButton.ClickAsync();
            
            // Wait for the task to appear
            await Page.WaitForTimeoutAsync(1000);
            
            // Clear the form for next iteration
            await Page.FillAsync("#todoTitle", "");
            await Page.FillAsync("#todoDescription", "");
        }
    }

    [TestMethod]
    public async Task ResponsiveFilterButtonsWork()
    {
        var viewports = new[]
        {
            new { Width = 1200, Height = 800 }, // Desktop
            new { Width = 768, Height = 1024 },  // Tablet
            new { Width = 375, Height = 667 }    // Mobile
        };

        foreach (var viewport in viewports)
        {
            await Page.SetViewportSizeAsync(viewport.Width, viewport.Height);
            
            // Test all filter buttons
            var filters = new[] { "all", "pending", "completed" };
            
            foreach (var filter in filters)
            {
                var filterButton = Page.Locator($"button[data-filter='{filter}']");
                await Expect(filterButton).ToBeVisibleAsync();
                await Expect(filterButton).ToBeEnabledAsync();
                
                await filterButton.ClickAsync();
                await Page.WaitForTimeoutAsync(500);
                
                // Verify the button becomes active
                await Expect(filterButton).ToHaveClassAsync(new Regex(".*active.*"));
            }
        }
    }
}