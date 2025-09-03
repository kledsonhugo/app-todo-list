# Todo List API - .NET 8.0 Web Application

A .NET 8.0 ASP.NET Core Web API application for managing todo lists with a modern web interface. The application provides REST API endpoints and a responsive HTML/CSS/JavaScript frontend.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Bootstrap and Build
- Restore dependencies: `dotnet restore app-todo-list.sln` -- takes 5-10 seconds
- Build application: `dotnet build app-todo-list.sln` -- takes ~10 seconds. NEVER CANCEL. Set timeout to 60+ seconds.
- Run application: `dotnet run --project TodoListApp.csproj` -- starts in ~3 seconds on http://localhost:5146

### Development Workflow
- ALWAYS use the solution file (`app-todo-list.sln`) for dotnet commands to avoid "multiple project" errors
- The application runs on port 5146 by default (configured in Properties/launchSettings.json)
- No test infrastructure exists - this is a simple demonstration application
- Code formatting issues exist - run `dotnet format app-todo-list.sln` to fix whitespace formatting

### Essential Commands
```bash
# Navigate to project root
cd /home/runner/work/app-todo-list/app-todo-list

# Restore and build (NEVER CANCEL - timeout 60+ seconds)
dotnet restore app-todo-list.sln
dotnet build app-todo-list.sln

# Run the application 
dotnet run --project TodoListApp.csproj

# Format code (fixes whitespace issues)
dotnet format app-todo-list.sln

# Check formatting without changes
dotnet format app-todo-list.sln --verify-no-changes
```

## Validation

### Manual Testing Scenarios
After making ANY changes, ALWAYS validate the complete application functionality:

1. **Build and Start Application**
   ```bash
   dotnet build app-todo-list.sln
   dotnet run --project TodoListApp.csproj
   ```

2. **Test API Endpoints** (application must be running)
   ```bash
   # List all todos (should return 3 default todos)
   curl -s http://localhost:5146/api/todos
   
   # Get specific todo
   curl -s http://localhost:5146/api/todos/1
   
   # Create new todo
   curl -s -X POST http://localhost:5146/api/todos \
     -H "Content-Type: application/json" \
     -d '{"title": "Test todo", "description": "Test description"}'
   
   # Update todo
   curl -s -X PUT http://localhost:5146/api/todos/1 \
     -H "Content-Type: application/json" \
     -d '{"title": "Updated", "description": "Updated desc", "isCompleted": true}'
   
   # Toggle completion
   curl -s -X PATCH http://localhost:5146/api/todos/1/toggle
   
   # Delete todo
   curl -s -X DELETE http://localhost:5146/api/todos/1
   ```

3. **Test Web Interface**
   - Navigate to http://localhost:5146 in browser or curl
   - Verify HTML page loads with CSS and JavaScript
   - The interface should show a modern todo list with Font Awesome icons

4. **Verify Swagger Documentation**
   - Swagger JSON is available at http://localhost:5146/swagger/v1/swagger.json
   - Swagger UI may be configured at /api/docs but check availability in development environment
   - API documentation is also available in TodoListApp.http for REST Client testing

### Critical Validation Steps
- ALWAYS run through at least one complete end-to-end scenario after making changes
- Test creating, reading, updating, and deleting todos via API
- Verify the web interface loads and displays the todo list correctly
- ALWAYS run `dotnet format app-todo-list.sln` before committing to fix formatting issues

## Application Architecture

### Key Components
- **Controllers/TodosController.cs** - REST API endpoints for todo operations
- **Services/TodoService.cs** - Business logic and in-memory data storage
- **Models/TodoItem.cs** - Todo item data model
- **DTOs/TodoItemDtos.cs** - Data transfer objects for API requests/responses
- **wwwroot/** - Static web files (HTML, CSS, JavaScript)
- **Program.cs** - Application configuration and dependency injection

### Technologies Used
- .NET 8.0 ASP.NET Core Web API
- Swagger/OpenAPI for documentation (Swashbuckle.AspNetCore)
- In-memory data storage (no database)
- CORS enabled for web interface
- Static file serving for HTML/CSS/JS frontend

### Default Data
Application starts with 3 sample todos (IDs 1, 2, 3):
1. "Estudar .NET" (pending) 
2. "Fazer exercícios" (completed)
3. "Ler documentação" (pending)

## Common Tasks

### Project Structure
```
├── Controllers/           # API controllers
│   └── TodosController.cs
├── DTOs/                 # Data transfer objects  
│   └── TodoItemDtos.cs
├── Models/               # Data models
│   └── TodoItem.cs
├── Services/             # Business logic
│   └── TodoService.cs
├── wwwroot/              # Web interface
│   ├── index.html
│   ├── script.js
│   └── styles.css
├── Properties/
│   └── launchSettings.json  # Port configuration (5146)
├── Program.cs            # App configuration
├── TodoListApp.csproj    # Project file
├── app-todo-list.sln     # Solution file (USE THIS)
└── TodoListApp.http      # API testing examples
```

### Frequent Commands Output

#### Repository Root Listing
```
$ ls -la /home/runner/work/app-todo-list/app-todo-list/
Controllers/
DTOs/
Models/
Services/
wwwroot/
Properties/
Program.cs
TodoListApp.csproj
app-todo-list.sln
README.md
INTERFACE_WEB.md
TESTE.md
TodoListApp.http
appsettings.json
appsettings.Development.json
```

#### Project File Contents
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.19" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
  </ItemGroup>
</Project>
```

## Important Notes

### Timing and Timeouts
- **Build time**: ~10 seconds. NEVER CANCEL. Set timeout to 60+ seconds minimum.
- **Startup time**: ~3 seconds after build
- **API response time**: < 1 second for all endpoints
- **Formatting check**: ~10 seconds

### Known Issues
- **Formatting**: Code has whitespace formatting issues. Always run `dotnet format app-todo-list.sln` before committing
- **Multiple projects error**: Always specify solution file (`app-todo-list.sln`) in dotnet commands
- **No tests**: Application has no unit tests - this is intentional for this demo project
- **In-memory storage**: Data is lost when application restarts (not persisted to database)

### Port Configuration
- **Default port**: 5146 (HTTP)
- **HTTPS port**: 7264 (if using HTTPS profile)
- **Configured in**: Properties/launchSettings.json

### Documentation Files
- **README.md**: Portuguese documentation with full feature list and usage examples
- **INTERFACE_WEB.md**: Detailed web interface usage guide in Portuguese  
- **TESTE.md**: API testing instructions with curl examples in Portuguese
- **TodoListApp.http**: REST Client examples for VS Code

## Quick Start for New Developers

1. **Clone and setup**: Navigate to `/home/runner/work/app-todo-list/app-todo-list`
2. **Install dependencies**: `dotnet restore app-todo-list.sln`
3. **Build**: `dotnet build app-todo-list.sln` (wait 60+ seconds, NEVER CANCEL)
4. **Run**: `dotnet run --project TodoListApp.csproj`
5. **Test**: Open http://localhost:5146 and http://localhost:5146/api/docs
6. **Validate**: Run the complete API testing scenario above

Remember: Always use solution file commands, wait for builds to complete, and test both API and web interface after any changes.