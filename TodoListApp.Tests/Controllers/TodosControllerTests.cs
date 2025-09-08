using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoListApp.Controllers;
using TodoListApp.DTOs;
using TodoListApp.Models;
using TodoListApp.Services;

namespace TodoListApp.Tests.Controllers
{
    public class TodosControllerTests
    {
        private readonly Mock<ITodoService> _mockTodoService;
        private readonly TodosController _controller;

        public TodosControllerTests()
        {
            _mockTodoService = new Mock<ITodoService>();
            _controller = new TodosController(_mockTodoService.Object);
        }

        #region GetTodos Tests

        [Fact]
        public async Task GetTodos_ReturnsOkResult_WithListOfTodos()
        {
            // Arrange
            var todos = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Test Todo 1", Description = "Description 1" },
                new TodoItem { Id = 2, Title = "Test Todo 2", Description = "Description 2" }
            };
            _mockTodoService.Setup(s => s.GetAllTodosAsync()).ReturnsAsync(todos);

            // Act
            var result = await _controller.GetTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTodos = Assert.IsAssignableFrom<IEnumerable<TodoItemDto>>(okResult.Value);
            Assert.Equal(2, returnedTodos.Count());
        }

        [Fact]
        public async Task GetTodos_ReturnsOkResult_WithEmptyList_WhenNoTodos()
        {
            // Arrange
            _mockTodoService.Setup(s => s.GetAllTodosAsync()).ReturnsAsync(new List<TodoItem>());

            // Act
            var result = await _controller.GetTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTodos = Assert.IsAssignableFrom<IEnumerable<TodoItemDto>>(okResult.Value);
            Assert.Empty(returnedTodos);
        }

        #endregion

        #region GetTodo Tests

        [Fact]
        public async Task GetTodo_ReturnsOkResult_WhenTodoExists()
        {
            // Arrange
            var todo = new TodoItem { Id = 1, Title = "Test Todo", Description = "Test Description" };
            _mockTodoService.Setup(s => s.GetTodoByIdAsync(1)).ReturnsAsync(todo);

            // Act
            var result = await _controller.GetTodo(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTodo = Assert.IsType<TodoItemDto>(okResult.Value);
            Assert.Equal(1, returnedTodo.Id);
            Assert.Equal("Test Todo", returnedTodo.Title);
        }

        [Fact]
        public async Task GetTodo_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            _mockTodoService.Setup(s => s.GetTodoByIdAsync(1)).ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _controller.GetTodo(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        #endregion

        #region CreateTodo Tests

        [Fact]
        public async Task CreateTodo_ReturnsCreatedAtAction_WithValidTodo()
        {
            // Arrange
            var createDto = new CreateTodoItemDto { Title = "New Todo", Description = "New Description" };
            var createdTodo = new TodoItem { Id = 1, Title = "New Todo", Description = "New Description" };
            _mockTodoService.Setup(s => s.CreateTodoAsync(It.IsAny<TodoItem>())).ReturnsAsync(createdTodo);

            // Act
            var result = await _controller.CreateTodo(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedTodo = Assert.IsType<TodoItemDto>(createdResult.Value);
            Assert.Equal("New Todo", returnedTodo.Title);
            Assert.Equal("New Description", returnedTodo.Description);
            Assert.Equal(nameof(_controller.GetTodo), createdResult.ActionName);
        }

        #endregion

        #region UpdateTodo Tests

        [Fact]
        public async Task UpdateTodo_ReturnsOkResult_WhenTodoExists()
        {
            // Arrange
            var updateDto = new UpdateTodoItemDto { Title = "Updated Todo", Description = "Updated Description", IsCompleted = true };
            var updatedTodo = new TodoItem { Id = 1, Title = "Updated Todo", Description = "Updated Description", IsCompleted = true };
            _mockTodoService.Setup(s => s.UpdateTodoAsync(1, It.IsAny<TodoItem>())).ReturnsAsync(updatedTodo);

            // Act
            var result = await _controller.UpdateTodo(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTodo = Assert.IsType<TodoItemDto>(okResult.Value);
            Assert.Equal("Updated Todo", returnedTodo.Title);
            Assert.True(returnedTodo.IsCompleted);
        }

        [Fact]
        public async Task UpdateTodo_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdateTodoItemDto { Title = "Updated Todo", Description = "Updated Description", IsCompleted = true };
            _mockTodoService.Setup(s => s.UpdateTodoAsync(1, It.IsAny<TodoItem>())).ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _controller.UpdateTodo(1, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        #endregion

        #region DeleteTodo Tests

        [Fact]
        public async Task DeleteTodo_ReturnsNoContent_WhenTodoExists()
        {
            // Arrange
            _mockTodoService.Setup(s => s.DeleteTodoAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTodo(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTodo_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            _mockTodoService.Setup(s => s.DeleteTodoAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTodo(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region ToggleCompletion Tests

        [Fact]
        public async Task ToggleCompletion_ReturnsOkResult_WhenTodoExists()
        {
            // Arrange
            var toggledTodo = new TodoItem { Id = 1, Title = "Test Todo", IsCompleted = true, CompletedAt = DateTime.UtcNow };
            _mockTodoService.Setup(s => s.ToggleCompletionAsync(1)).ReturnsAsync(toggledTodo);

            // Act
            var result = await _controller.ToggleCompletion(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTodo = Assert.IsType<TodoItemDto>(okResult.Value);
            Assert.True(returnedTodo.IsCompleted);
            Assert.NotNull(returnedTodo.CompletedAt);
        }

        [Fact]
        public async Task ToggleCompletion_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            _mockTodoService.Setup(s => s.ToggleCompletionAsync(1)).ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _controller.ToggleCompletion(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        #endregion
    }
}