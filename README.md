# Todo List API

Uma aplicação .NET Web API para gerenciamento de lista de tarefas (to-do list) com interface web moderna.

## Funcionalidades

- ✅ **Interface Web Completa** - Interface moderna e responsiva
- ✅ Criar novas tarefas
- ✅ Listar todas as tarefas
- ✅ Filtrar tarefas (Todas, Pendentes, Concluídas)
- ✅ Obter uma tarefa específica por ID
- ✅ Atualizar tarefas existentes
- ✅ Marcar/desmarcar tarefas como concluídas
- ✅ Deletar tarefas
- ✅ Documentação automática com Swagger/OpenAPI
- ✅ Notificações em tempo real
- ✅ Design responsivo para mobile

## Acesso Rápido

- **🌐 Interface Web:** http://localhost:5146
- **📚 API Docs (Swagger):** http://localhost:5146/api/docs

## Estrutura do Projeto

```
├── Controllers/        # Controladores da API
│   └── TodosController.cs
├── DTOs/              # Data Transfer Objects
│   └── TodoItemDtos.cs
├── Models/            # Modelos de dados
│   └── TodoItem.cs
├── Services/          # Serviços de negócio
│   └── TodoService.cs
├── wwwroot/           # Interface Web
│   ├── index.html     # Página principal
│   ├── styles.css     # Estilos CSS
│   └── script.js      # JavaScript da interface
├── Program.cs         # Configuração da aplicação
└── TodoListApp.http   # Exemplos de requisições HTTP
```

## Como Executar

1. **Pré-requisitos:**
   - .NET 8.0 SDK instalado

2. **Executar a aplicação:**
   ```bash
   dotnet run
   ```

3. **Acessar a aplicação:**
   - **Interface Web:** http://localhost:5146
   - **Documentação da API:** http://localhost:5146/api/docs

## Interface Web

A interface web oferece:

### 🎨 Design Moderno
- Interface limpa e intuitiva
- Gradient de cores atrativo
- Ícones Font Awesome
- Animações suaves

### 📱 Responsivo
- Funciona perfeitamente em desktop, tablet e mobile
- Layout adaptativo
- Botões otimizados para touch

### ⚡ Funcionalidades Interativas
- **Adicionar Tarefas** - Formulário com validação
- **Filtrar Tarefas** - Todas, Pendentes, Concluídas
- **Editar Tarefas** - Modal de edição completo
- **Marcar como Concluída** - Toggle rápido
- **Excluir Tarefas** - Com confirmação
- **Notificações** - Toast messages para feedback
- **Atualização Automática** - Sincronização com a API

### 🔄 Integração com API
- Comunicação assíncrona com a API
- Tratamento de erros
- Loading states
- Validação de dados

## Endpoints da API

### GET /api/todos
Retorna todas as tarefas.

### GET /api/todos/{id}
Retorna uma tarefa específica por ID.

### POST /api/todos
Cria uma nova tarefa.

**Body:**
```json
{
  "title": "Título da tarefa",
  "description": "Descrição opcional"
}
```

### PUT /api/todos/{id}
Atualiza uma tarefa existente.

**Body:**
```json
{
  "title": "Novo título",
  "description": "Nova descrição",
  "isCompleted": true
}
```

### PATCH /api/todos/{id}/toggle
Alterna o status de conclusão de uma tarefa.

### DELETE /api/todos/{id}
Remove uma tarefa.

## Modelo de Dados

### TodoItem
```json
{
  "id": 1,
  "title": "Título da tarefa",
  "description": "Descrição da tarefa",
  "isCompleted": false,
  "createdAt": "2025-09-03T10:30:00Z",
  "completedAt": null,
  "updatedAt": "2025-09-03T10:30:00Z"
}
```

## Tecnologias Utilizadas

### Backend
- .NET 8.0
- ASP.NET Core Web API
- Swagger/OpenAPI para documentação
- CORS configurado
- Armazenamento em memória

### Frontend
- HTML5 semântico
- CSS3 com Flexbox/Grid
- JavaScript ES6+
- Font Awesome Icons
- Fetch API para requisições
- Design responsivo

## Dados de Exemplo

A aplicação inicia com 3 tarefas de exemplo:
1. **"Estudar .NET"** (pendente)
2. **"Fazer exercícios"** (concluída)
3. **"Ler documentação"** (pendente)

## Screenshots das Funcionalidades

### 📝 Adicionar Tarefas
- Formulário intuitivo com validação
- Campos para título e descrição
- Feedback visual de sucesso

### 🔍 Filtros Inteligentes
- Botões para filtrar por status
- Contadores visuais
- Transições suaves

### ✏️ Edição Inline
- Modal elegante para edição
- Todos os campos editáveis
- Validação em tempo real

### 📱 Mobile First
- Interface otimizada para mobile
- Botões grandes para facilitar o toque
- Layout empilhado em telas pequenas

## Próximos Passos

Para uma aplicação de produção, considere implementar:

- Persistência em banco de dados (Entity Framework Core)
- Autenticação e autorização
- Paginação para listagem de tarefas
- Busca e filtros avançados
- Categorias/tags para tarefas
- Datas de vencimento
- Notificações push
- Modo offline (PWA)
- Testes unitários e de integração
- Docker para containerização