using Microsoft.AspNetCore.Mvc;
using TodoListApp.DTOs;
using TodoListApp.Models;
using TodoListApp.Services;

namespace TodoListApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ITodoService _todoService;
        
        public TodosController(ITodoService todoService)
        {
            _todoService = todoService;
        }
        
        /// <summary>
        /// Obter todas as tarefas
        /// </summary>
        /// <returns>Lista de todas as tarefas</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodos()
        {
            var todos = await _todoService.GetAllTodosAsync();
            var todoDtos = todos.Select(MapToDto);
            return Ok(todoDtos);
        }
        
        /// <summary>
        /// Obter uma tarefa específica por ID
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <returns>Tarefa específica</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDto>> GetTodo(int id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
                return NotFound();
                
            return Ok(MapToDto(todo));
        }
        
        /// <summary>
        /// Criar uma nova tarefa
        /// </summary>
        /// <param name="createTodoDto">Dados da nova tarefa</param>
        /// <returns>Tarefa criada</returns>
        [HttpPost]
        public async Task<ActionResult<TodoItemDto>> CreateTodo(CreateTodoItemDto createTodoDto)
        {
            var todoItem = new TodoItem
            {
                Title = createTodoDto.Title,
                Description = createTodoDto.Description
            };
            
            var createdTodo = await _todoService.CreateTodoAsync(todoItem);
            var todoDto = MapToDto(createdTodo);
            
            return CreatedAtAction(nameof(GetTodo), new { id = createdTodo.Id }, todoDto);
        }
        
        /// <summary>
        /// Atualizar uma tarefa existente
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <param name="updateTodoDto">Dados atualizados da tarefa</param>
        /// <returns>Tarefa atualizada</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<TodoItemDto>> UpdateTodo(int id, UpdateTodoItemDto updateTodoDto)
        {
            var todoItem = new TodoItem
            {
                Title = updateTodoDto.Title,
                Description = updateTodoDto.Description,
                IsCompleted = updateTodoDto.IsCompleted
            };
            
            var updatedTodo = await _todoService.UpdateTodoAsync(id, todoItem);
            if (updatedTodo == null)
                return NotFound();
                
            return Ok(MapToDto(updatedTodo));
        }
        
        /// <summary>
        /// Deletar uma tarefa
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var deleted = await _todoService.DeleteTodoAsync(id);
            if (!deleted)
                return NotFound();
                
            return NoContent();
        }
        
        /// <summary>
        /// Alternar o status de conclusão de uma tarefa
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <returns>Tarefa com status atualizado</returns>
        [HttpPatch("{id}/toggle")]
        public async Task<ActionResult<TodoItemDto>> ToggleCompletion(int id)
        {
            var updatedTodo = await _todoService.ToggleCompletionAsync(id);
            if (updatedTodo == null)
                return NotFound();
                
            return Ok(MapToDto(updatedTodo));
        }
        
        private static TodoItemDto MapToDto(TodoItem todoItem)
        {
            return new TodoItemDto
            {
                Id = todoItem.Id,
                Title = todoItem.Title,
                Description = todoItem.Description,
                IsCompleted = todoItem.IsCompleted,
                CreatedAt = todoItem.CreatedAt,
                CompletedAt = todoItem.CompletedAt,
                UpdatedAt = todoItem.UpdatedAt
            };
        }
    }
}
