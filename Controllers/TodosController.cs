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
        
        /// <summary>
        /// ⚠️ MÉTODO VULNERÁVEL - APENAS PARA FINS EDUCACIONAIS ⚠️
        /// Este método demonstra uma vulnerabilidade de SQL Injection
        /// NUNCA USE ESTA ABORDAGEM EM PRODUÇÃO!
        /// </summary>
        /// <param name="searchTerm">Termo de busca (VULNERÁVEL)</param>
        /// <returns>Resultado da busca vulnerável</returns>
        [HttpGet("vulnerable-search")]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> VulnerableSearch(string searchTerm)
        {
            // ⚠️ CÓDIGO VULNERÁVEL - SQL INJECTION ⚠️
            // Este é um exemplo de como NÃO fazer uma consulta
            
            // Simulação de uma query SQL vulnerável que seria executada
            var vulnerableSqlQuery = $"SELECT * FROM Todos WHERE Title LIKE '%{searchTerm}%' OR Description LIKE '%{searchTerm}%'";
            
            // Log da query vulnerável para demonstrar o problema
            Console.WriteLine($"⚠️ Query Vulnerável Executada: {vulnerableSqlQuery}");
            Console.WriteLine($"⚠️ Se searchTerm fosse: \"'; DROP TABLE Todos; --\"");
            Console.WriteLine($"⚠️ A query seria: SELECT * FROM Todos WHERE Title LIKE '%'; DROP TABLE Todos; --%'");
            
            // Para fins de demonstração, vamos simular alguns ataques comuns
            if (searchTerm.Contains("'") || searchTerm.Contains(";") || 
                searchTerm.ToLower().Contains("drop") || searchTerm.ToLower().Contains("delete") ||
                searchTerm.ToLower().Contains("insert") || searchTerm.ToLower().Contains("update"))
            {
                Console.WriteLine("🚨 ATAQUE DE SQL INJECTION DETECTADO! 🚨");
                Console.WriteLine($"🚨 Entrada maliciosa: {searchTerm}");
                
                // Em um cenário real, isso poderia resultar em:
                // - Vazamento de dados
                // - Exclusão de tabelas
                // - Modificação não autorizada
                // - Acesso a informações sensíveis
                
                return BadRequest(new { 
                    error = "SQL Injection detectado!", 
                    message = "Em um sistema real, este ataque poderia comprometer toda a base de dados.",
                    vulnerableQuery = vulnerableSqlQuery,
                    educationalNote = "Este é um exemplo educacional. NUNCA construa queries assim em produção!"
                });
            }
            
            // Simulação da busca "segura" apenas para demonstração
            var todos = await _todoService.GetAllTodosAsync();
            var filteredTodos = todos.Where(t => 
                t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (t.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
            );
            
            var todoDtos = filteredTodos.Select(MapToDto);
            
            return Ok(new {
                results = todoDtos,
                educationalInfo = new {
                    vulnerableQuery = vulnerableSqlQuery,
                    warning = "Esta query seria vulnerável a SQL Injection em um cenário real!",
                    correctApproach = "Use parâmetros ou ORMs como Entity Framework"
                }
            });
        }

        /// <summary>
        /// ✅ MÉTODO SEGURO - Como fazer a busca corretamente
        /// </summary>
        /// <param name="searchTerm">Termo de busca</param>
        /// <returns>Resultado da busca segura</returns>
        [HttpGet("secure-search")]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> SecureSearch(string searchTerm)
        {
            // ✅ Abordagem segura usando parâmetros
            Console.WriteLine("✅ Busca segura executada com parâmetros");
            
            // Em um cenário real com Entity Framework, seria algo como:
            // var todos = await _context.Todos
            //     .Where(t => EF.Functions.Like(t.Title, $"%{searchTerm}%") || 
            //                 EF.Functions.Like(t.Description, $"%{searchTerm}%"))
            //     .ToListAsync();
            
            var todos = await _todoService.GetAllTodosAsync();
            var filteredTodos = todos.Where(t => 
                t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (t.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
            );
            
            var todoDtos = filteredTodos.Select(MapToDto);
            return Ok(todoDtos);
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
