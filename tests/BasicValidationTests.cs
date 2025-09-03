using NUnit.Framework;
using System.Net.Http;

namespace TodoListApp.Tests;

/// <summary>
/// Simple validation tests to ensure the test framework is working
/// These tests validate basic functionality without requiring browser automation
/// </summary>
[TestFixture]
public class BasicValidationTests
{
    [Test]
    public void Framework_Should_Be_Working()
    {
        // Arrange
        var expected = "Hello, Test Framework!";
        
        // Act
        var actual = "Hello, Test Framework!";
        
        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestHelpers_Should_Generate_Unique_Data()
    {
        // Act
        var (title1, description1) = TestHelpers.GenerateTestTodoData();
        Thread.Sleep(10); // Small delay to ensure different timestamps
        var (title2, description2) = TestHelpers.GenerateTestTodoData();
        
        // Assert
        Assert.That(title1, Is.Not.EqualTo(title2), "Titles should be unique");
        Assert.That(description1, Is.Not.EqualTo(description2), "Descriptions should be unique");
        Assert.That(title1, Does.StartWith("Test Todo"), "Title should start with 'Test Todo'");
        Assert.That(description1, Does.Contain("Automated test description"), "Description should contain expected text");
    }

    [Test]
    public void TestHelpers_Should_Have_Expected_Data()
    {
        // Assert
        Assert.That(TestHelpers.ExpectedData.InitialTodoCount, Is.EqualTo(3));
        Assert.That(TestHelpers.ExpectedData.InitialTodoTitles, Has.Length.EqualTo(3));
        Assert.That(TestHelpers.ExpectedData.PageTitle, Is.EqualTo("Todo List - Gerenciador de Tarefas"));
        Assert.That(TestHelpers.ExpectedData.MainHeading, Is.EqualTo("Minha Lista de Tarefas"));
    }

    [Test]
    public void TestHelpers_Should_Have_Valid_Selectors()
    {
        // Assert - verify selectors are not empty
        Assert.That(TestHelpers.Selectors.TodoTitle, Is.Not.Empty);
        Assert.That(TestHelpers.Selectors.TodoDescription, Is.Not.Empty);
        Assert.That(TestHelpers.Selectors.SubmitButton, Is.Not.Empty);
        Assert.That(TestHelpers.Selectors.TodoItem, Is.Not.Empty);
        
        // Verify selectors have correct format
        Assert.That(TestHelpers.Selectors.TodoTitle, Does.StartWith("#"), "ID selectors should start with #");
        Assert.That(TestHelpers.Selectors.TodoItem, Does.StartWith("."), "Class selectors should start with .");
    }

    [Test]
    public void TestHelpers_Should_Have_Reasonable_Timeouts()
    {
        // Assert - verify timeouts are reasonable
        Assert.That(TestHelpers.Timeouts.StandardWait, Is.GreaterThan(0));
        Assert.That(TestHelpers.Timeouts.LongWait, Is.GreaterThan(TestHelpers.Timeouts.StandardWait));
        Assert.That(TestHelpers.Timeouts.NetworkWait, Is.GreaterThan(0));
        
        // Verify timeouts are not too long
        Assert.That(TestHelpers.Timeouts.LongWait, Is.LessThan(10000), "Long wait should be less than 10 seconds");
    }

    [Test]
    public void HttpClient_Should_Be_Available()
    {
        // Act & Assert
        using var client = new HttpClient();
        Assert.That(client, Is.Not.Null);
        Assert.That(client.BaseAddress, Is.Null); // Should be null initially
    }
}