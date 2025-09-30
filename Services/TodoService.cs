using TodoListApp.Models;

namespace TodoListApp.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoItem>> GetAllTodosAsync();
        Task<TodoItem?> GetTodoByIdAsync(int id);
        Task<TodoItem> CreateTodoAsync(TodoItem todoItem);
        Task<TodoItem?> UpdateTodoAsync(int id, TodoItem todoItem);
        Task<bool> DeleteTodoAsync(int id);
        Task<TodoItem?> ToggleCompletionAsync(int id);
    }

    public class TodoService : ITodoService
    {
        private readonly List<TodoItem> _todos = new();
        private readonly IIconService _iconService;
        private int _nextId = 1;

        public TodoService(IIconService iconService)
        {
            _iconService = iconService;
            // Adicionar algumas tarefas de exemplo para facilitar os testes
            SeedData();
        }

        private void SeedData()
        {
            var sampleTodos = new[]
            {
                new TodoItem
                {
                    Id = _nextId++,
                    Title = "Estudar .NET",
                    Description = "Aprender sobre desenvolvimento de APIs com .NET",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow.AddHours(-2),
                    UpdatedAt = DateTime.UtcNow.AddHours(-2)
                },
                new TodoItem
                {
                    Id = _nextId++,
                    Title = "Fazer exercícios",
                    Description = "30 minutos de caminhada",
                    IsCompleted = true,
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                    UpdatedAt = DateTime.UtcNow.AddMinutes(-30),
                    CompletedAt = DateTime.UtcNow.AddMinutes(-30)
                },
                new TodoItem
                {
                    Id = _nextId++,
                    Title = "Ler documentação",
                    Description = "Ler a documentação do ASP.NET Core",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                    UpdatedAt = DateTime.UtcNow.AddMinutes(-30)
                }
            };

            // Set icons for each todo based on content
            foreach (var todo in sampleTodos)
            {
                todo.Icon = _iconService.GetIconForTask(todo.Title, todo.Description);
            }

            _todos.AddRange(sampleTodos);
        }

        public Task<IEnumerable<TodoItem>> GetAllTodosAsync()
        {
            return Task.FromResult(_todos.AsEnumerable());
        }

        public Task<TodoItem?> GetTodoByIdAsync(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            return Task.FromResult(todo);
        }

        public Task<TodoItem> CreateTodoAsync(TodoItem todoItem)
        {
            todoItem.Id = _nextId++;
            todoItem.CreatedAt = DateTime.UtcNow;
            todoItem.UpdatedAt = DateTime.UtcNow;

            // Auto-assign icon based on content if not provided
            if (string.IsNullOrEmpty(todoItem.Icon))
            {
                todoItem.Icon = _iconService.GetIconForTask(todoItem.Title, todoItem.Description);
            }

            _todos.Add(todoItem);
            return Task.FromResult(todoItem);
        }

        public Task<TodoItem?> UpdateTodoAsync(int id, TodoItem updatedTodo)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
                return Task.FromResult<TodoItem?>(null);

            todo.Title = updatedTodo.Title;
            todo.Description = updatedTodo.Description;
            todo.IsCompleted = updatedTodo.IsCompleted;
            todo.UpdatedAt = DateTime.UtcNow;

            if (updatedTodo.IsCompleted && todo.CompletedAt == null)
                todo.CompletedAt = DateTime.UtcNow;
            else if (!updatedTodo.IsCompleted)
                todo.CompletedAt = null;

            return Task.FromResult<TodoItem?>(todo);
        }

        public Task<bool> DeleteTodoAsync(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
                return Task.FromResult(false);

            _todos.Remove(todo);
            return Task.FromResult(true);
        }

        public Task<TodoItem?> ToggleCompletionAsync(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
                return Task.FromResult<TodoItem?>(null);

            todo.IsCompleted = !todo.IsCompleted;
            todo.UpdatedAt = DateTime.UtcNow;

            if (todo.IsCompleted)
                todo.CompletedAt = DateTime.UtcNow;
            else
                todo.CompletedAt = null;

            return Task.FromResult<TodoItem?>(todo);
        }
    }
}
