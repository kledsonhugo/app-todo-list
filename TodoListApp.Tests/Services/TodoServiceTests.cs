using TodoListApp.Models;
using TodoListApp.Services;

namespace TodoListApp.Tests.Services
{
    public class TodoServiceTests
    {
        [Fact]
        public async Task GetAllTodosAsync_ReturnsPreseededTodos()
        {
            // Arrange
            var service = new TodoService();

            // Act
            var result = await service.GetAllTodosAsync();

            // Assert
            var todos = result.ToList();
            Assert.Equal(3, todos.Count);
            Assert.Contains(todos, t => t.Title == "Estudar .NET");
            Assert.Contains(todos, t => t.Title == "Fazer exercícios");
            Assert.Contains(todos, t => t.Title == "Ler documentação");
        }

        [Fact]
        public async Task GetTodoByIdAsync_ReturnsCorrectTodo_WhenTodoExists()
        {
            // Arrange
            var service = new TodoService();

            // Act
            var result = await service.GetTodoByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Estudar .NET", result.Title);
        }

        [Fact]
        public async Task GetTodoByIdAsync_ReturnsNull_WhenTodoDoesNotExist()
        {
            // Arrange
            var service = new TodoService();

            // Act
            var result = await service.GetTodoByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateTodoAsync_CreatesTodoWithCorrectProperties()
        {
            // Arrange
            var service = new TodoService();
            var newTodo = new TodoItem
            {
                Title = "New Test Todo",
                Description = "Test Description"
            };

            // Act
            var result = await service.CreateTodoAsync(newTodo);

            // Assert
            Assert.Equal(4, result.Id); // Should be ID 4 since 3 todos are seeded
            Assert.Equal("New Test Todo", result.Title);
            Assert.Equal("Test Description", result.Description);
            Assert.False(result.IsCompleted);
            Assert.True(result.CreatedAt > DateTime.MinValue);
            Assert.True(result.UpdatedAt > DateTime.MinValue);
        }

        [Fact]
        public async Task CreateTodoAsync_AddsTodoToCollection()
        {
            // Arrange
            var service = new TodoService();
            var newTodo = new TodoItem
            {
                Title = "New Test Todo",
                Description = "Test Description"
            };

            // Act
            await service.CreateTodoAsync(newTodo);
            var allTodos = await service.GetAllTodosAsync();

            // Assert
            var todosList = allTodos.ToList();
            Assert.Equal(4, todosList.Count); // 3 seeded + 1 new
            Assert.Contains(todosList, t => t.Title == "New Test Todo");
        }

        [Fact]
        public async Task UpdateTodoAsync_UpdatesExistingTodo_WhenTodoExists()
        {
            // Arrange
            var service = new TodoService();
            var updateTodo = new TodoItem
            {
                Title = "Updated Title",
                Description = "Updated Description",
                IsCompleted = true
            };

            // Act
            var result = await service.UpdateTodoAsync(1, updateTodo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Updated Title", result.Title);
            Assert.Equal("Updated Description", result.Description);
            Assert.True(result.IsCompleted);
            Assert.NotNull(result.CompletedAt);
        }

        [Fact]
        public async Task UpdateTodoAsync_ReturnsNull_WhenTodoDoesNotExist()
        {
            // Arrange
            var service = new TodoService();
            var updateTodo = new TodoItem
            {
                Title = "Updated Title",
                Description = "Updated Description",
                IsCompleted = true
            };

            // Act
            var result = await service.UpdateTodoAsync(999, updateTodo);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateTodoAsync_SetsCompletedAt_WhenMarkingAsCompleted()
        {
            // Arrange
            var service = new TodoService();
            var updateTodo = new TodoItem
            {
                Title = "Test",
                Description = "Test",
                IsCompleted = true
            };

            // Act
            var result = await service.UpdateTodoAsync(1, updateTodo);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsCompleted);
            Assert.NotNull(result.CompletedAt);
        }

        [Fact]
        public async Task UpdateTodoAsync_ClearsCompletedAt_WhenMarkingAsIncomplete()
        {
            // Arrange
            var service = new TodoService();
            // First mark as completed
            var completeTodo = new TodoItem { Title = "Test", Description = "Test", IsCompleted = true };
            await service.UpdateTodoAsync(2, completeTodo); // Todo 2 is already completed

            // Now mark as incomplete
            var incompleteTodo = new TodoItem { Title = "Test", Description = "Test", IsCompleted = false };

            // Act
            var result = await service.UpdateTodoAsync(2, incompleteTodo);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsCompleted);
            Assert.Null(result.CompletedAt);
        }

        [Fact]
        public async Task DeleteTodoAsync_DeletesExistingTodo_WhenTodoExists()
        {
            // Arrange
            var service = new TodoService();

            // Act
            var result = await service.DeleteTodoAsync(1);

            // Assert
            Assert.True(result);
            
            // Verify it's actually deleted
            var deletedTodo = await service.GetTodoByIdAsync(1);
            Assert.Null(deletedTodo);
            
            // Verify total count decreased
            var allTodos = await service.GetAllTodosAsync();
            Assert.Equal(2, allTodos.Count());
        }

        [Fact]
        public async Task DeleteTodoAsync_ReturnsFalse_WhenTodoDoesNotExist()
        {
            // Arrange
            var service = new TodoService();

            // Act
            var result = await service.DeleteTodoAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ToggleCompletionAsync_TogglesFromIncompleteToComplete()
        {
            // Arrange
            var service = new TodoService();

            // Act
            var result = await service.ToggleCompletionAsync(1); // Todo 1 is initially incomplete

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsCompleted);
            Assert.NotNull(result.CompletedAt);
        }

        [Fact]
        public async Task ToggleCompletionAsync_TogglesFromCompleteToIncomplete()
        {
            // Arrange
            var service = new TodoService();

            // Act
            var result = await service.ToggleCompletionAsync(2); // Todo 2 is initially complete

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsCompleted);
            Assert.Null(result.CompletedAt);
        }

        [Fact]
        public async Task ToggleCompletionAsync_ReturnsNull_WhenTodoDoesNotExist()
        {
            // Arrange
            var service = new TodoService();

            // Act
            var result = await service.ToggleCompletionAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ToggleCompletionAsync_UpdatesTimestamp()
        {
            // Arrange
            var service = new TodoService();
            var originalTodo = await service.GetTodoByIdAsync(1);
            var originalUpdatedAt = originalTodo!.UpdatedAt;

            // Wait a moment to ensure timestamp difference
            await Task.Delay(10);

            // Act
            var result = await service.ToggleCompletionAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.UpdatedAt > originalUpdatedAt);
        }
    }
}