using Microsoft.Playwright;
using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.Fixtures)]

namespace TodoListApp.Tests;

/// <summary>
/// Global test setup for Playwright browser automation
/// Configures browsers and ensures proper cleanup
/// </summary>
[SetUpFixture]
public class PlaywrightSetup
{
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        // Install Playwright browsers if needed
        // This is typically done during CI/CD setup
        Microsoft.Playwright.Program.Main(new[] { "install" });
        
        // Wait a moment for installation to complete
        await Task.Delay(1000);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        // Cleanup after all tests
    }
}

/// <summary>
/// Base class for all Playwright tests with common configuration
/// Implements MCP best practices for browser testing
/// </summary>
public abstract class PlaywrightTestBase
{
    protected IBrowser Browser { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;

    [SetUp]
    public async Task SetUp()
    {
        // Launch browser with optimal settings for testing
        var playwright = await Playwright.CreateAsync();
        Browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true, // Set to false for debugging
            SlowMo = 50 // Add small delay between actions for stability
        });

        // Create browser context with realistic settings
        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 },
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            Locale = "pt-BR",
            TimezoneId = "America/Sao_Paulo"
        });

        // Create new page
        Page = await Context.NewPageAsync();

        // Set default timeout
        Page.SetDefaultTimeout(30000);
        Page.SetDefaultNavigationTimeout(30000);

        // Add event listeners for debugging
        Page.Console += (_, e) => TestContext.WriteLine($"Console {e.Type}: {e.Text}");
        Page.PageError += (_, e) => TestContext.WriteLine($"Page Error: {e}");
    }

    [TearDown]
    public async Task TearDown()
    {
        await Page?.CloseAsync();
        await Context?.CloseAsync();
        await Browser?.CloseAsync();
    }
}