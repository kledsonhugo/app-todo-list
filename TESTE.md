# Teste da API Todo List

## Instruções de Teste

A aplicação está rodando em: http://localhost:5146

**IMPORTANTE:** A aplicação agora inicia com 3 tarefas de exemplo (IDs 1, 2 e 3) para facilitar os testes.

### 1. Abrir a documentação Swagger
Acesse: http://localhost:5146

### 2. Testar os endpoints usando curl

#### Listar todas as tarefas (deve mostrar 3 tarefas):
```bash
curl -X GET "http://localhost:5146/api/todos"
```

#### Obter uma tarefa específica (agora funcionará):
```bash
curl -X GET "http://localhost:5146/api/todos/1"
```

#### Atualizar uma tarefa (agora funcionará com IDs 1, 2 ou 3):
```bash
curl -X PUT "http://localhost:5146/api/todos/1" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Estudar .NET Core - ATUALIZADO",
    "description": "Aprender sobre desenvolvimento de APIs com .NET Core e Entity Framework",
    "isCompleted": true
  }'
```

#### Criar uma nova tarefa (receberá ID 4):
```bash
curl -X POST "http://localhost:5146/api/todos" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Nova tarefa",
    "description": "Esta é uma nova tarefa criada via API"
  }'
```

#### Marcar/desmarcar tarefa como concluída:
```bash
curl -X PATCH "http://localhost:5146/api/todos/1/toggle"
```

#### Deletar uma tarefa:
```bash
curl -X DELETE "http://localhost:5146/api/todos/3"
```

### 3. Usando o Swagger UI
1. Acesse http://localhost:5146
2. Use GET /api/todos para ver as 3 tarefas iniciais
3. Agora o PUT funcionará com IDs 1, 2 ou 3
4. Use qualquer endpoint com confiança

### 4. Tarefas de Exemplo Disponíveis:
- **ID 1:** "Estudar .NET" (não concluída)
- **ID 2:** "Fazer exercícios" (concluída)  
- **ID 3:** "Ler documentação" (não concluída)

### 5. Usando o arquivo .http
Você também pode usar o arquivo `TodoListApp.http` no VS Code com a extensão REST Client para testar os endpoints.
