using System.ComponentModel.DataAnnotations;
using TodoListApp.DTOs;
using TodoListApp.Models;

namespace TodoListApp.Tests.Models
{
    public class ValidationTests
    {
        #region TodoItem Validation Tests

        [Fact]
        public void TodoItem_ValidModel_PassesValidation()
        {
            // Arrange
            var todo = new TodoItem
            {
                Title = "Valid Title",
                Description = "Valid Description"
            };

            // Act
            var validationResults = ValidateModel(todo);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact]
        public void TodoItem_EmptyTitle_FailsValidation()
        {
            // Arrange
            var todo = new TodoItem
            {
                Title = string.Empty,
                Description = "Valid Description"
            };

            // Act
            var validationResults = ValidateModel(todo);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(TodoItem.Title)));
        }

        [Fact]
        public void TodoItem_TitleTooLong_FailsValidation()
        {
            // Arrange
            var todo = new TodoItem
            {
                Title = new string('A', 201), // 201 characters, exceeds limit of 200
                Description = "Valid Description"
            };

            // Act
            var validationResults = ValidateModel(todo);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(TodoItem.Title)));
        }

        [Fact]
        public void TodoItem_DescriptionTooLong_FailsValidation()
        {
            // Arrange
            var todo = new TodoItem
            {
                Title = "Valid Title",
                Description = new string('A', 501) // 501 characters, exceeds limit of 500
            };

            // Act
            var validationResults = ValidateModel(todo);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(TodoItem.Description)));
        }

        [Fact]
        public void TodoItem_NullDescription_PassesValidation()
        {
            // Arrange
            var todo = new TodoItem
            {
                Title = "Valid Title",
                Description = null
            };

            // Act
            var validationResults = ValidateModel(todo);

            // Assert
            Assert.Empty(validationResults);
        }

        #endregion

        #region CreateTodoItemDto Validation Tests

        [Fact]
        public void CreateTodoItemDto_ValidDto_PassesValidation()
        {
            // Arrange
            var dto = new CreateTodoItemDto
            {
                Title = "Valid Title",
                Description = "Valid Description"
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact]
        public void CreateTodoItemDto_EmptyTitle_FailsValidation()
        {
            // Arrange
            var dto = new CreateTodoItemDto
            {
                Title = string.Empty,
                Description = "Valid Description"
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(CreateTodoItemDto.Title)));
        }

        [Fact]
        public void CreateTodoItemDto_TitleTooLong_FailsValidation()
        {
            // Arrange
            var dto = new CreateTodoItemDto
            {
                Title = new string('A', 201), // 201 characters, exceeds limit of 200
                Description = "Valid Description"
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(CreateTodoItemDto.Title)));
        }

        [Fact]
        public void CreateTodoItemDto_NullDescription_PassesValidation()
        {
            // Arrange
            var dto = new CreateTodoItemDto
            {
                Title = "Valid Title",
                Description = null
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.Empty(validationResults);
        }

        #endregion

        #region UpdateTodoItemDto Validation Tests

        [Fact]
        public void UpdateTodoItemDto_ValidDto_PassesValidation()
        {
            // Arrange
            var dto = new UpdateTodoItemDto
            {
                Title = "Valid Title",
                Description = "Valid Description",
                IsCompleted = true
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact]
        public void UpdateTodoItemDto_EmptyTitle_FailsValidation()
        {
            // Arrange
            var dto = new UpdateTodoItemDto
            {
                Title = string.Empty,
                Description = "Valid Description",
                IsCompleted = false
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(UpdateTodoItemDto.Title)));
        }

        [Fact]
        public void UpdateTodoItemDto_TitleTooLong_FailsValidation()
        {
            // Arrange
            var dto = new UpdateTodoItemDto
            {
                Title = new string('A', 201), // 201 characters, exceeds limit of 200
                Description = "Valid Description",
                IsCompleted = false
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(UpdateTodoItemDto.Title)));
        }

        [Fact]
        public void UpdateTodoItemDto_DescriptionTooLong_FailsValidation()
        {
            // Arrange
            var dto = new UpdateTodoItemDto
            {
                Title = "Valid Title",
                Description = new string('A', 501), // 501 characters, exceeds limit of 500
                IsCompleted = false
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(UpdateTodoItemDto.Description)));
        }

        #endregion

        #region TodoItemDto Tests (Property Mapping)

        [Fact]
        public void TodoItemDto_HasAllRequiredProperties()
        {
            // Arrange & Act
            var dto = new TodoItemDto
            {
                Id = 1,
                Title = "Test Title",
                Description = "Test Description",
                IsCompleted = true,
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Assert
            Assert.Equal(1, dto.Id);
            Assert.Equal("Test Title", dto.Title);
            Assert.Equal("Test Description", dto.Description);
            Assert.True(dto.IsCompleted);
            Assert.NotNull(dto.CreatedAt);
            Assert.NotNull(dto.CompletedAt);
            Assert.NotNull(dto.UpdatedAt);
        }

        [Fact]
        public void TodoItemDto_CanHaveNullOptionalProperties()
        {
            // Arrange & Act
            var dto = new TodoItemDto
            {
                Id = 1,
                Title = "Test Title",
                Description = null,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                CompletedAt = null,
                UpdatedAt = DateTime.UtcNow
            };

            // Assert
            Assert.Equal(1, dto.Id);
            Assert.Equal("Test Title", dto.Title);
            Assert.Null(dto.Description);
            Assert.False(dto.IsCompleted);
            Assert.NotNull(dto.CreatedAt);
            Assert.Null(dto.CompletedAt);
            Assert.NotNull(dto.UpdatedAt);
        }

        #endregion

        #region Helper Methods

        private static List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        #endregion
    }
}