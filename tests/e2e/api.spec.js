import { test, expect } from '@playwright/test';

test.describe('Todo List API Tests', () => {
  const baseURL = 'http://localhost:5146';

  test('deve listar todos os TODOs via API', async ({ request }) => {
    const response = await request.get(`${baseURL}/api/todos`);
    
    expect(response.status()).toBe(200);
    
    const todos = await response.json();
    expect(Array.isArray(todos)).toBe(true);
    expect(todos.length).toBeGreaterThan(0);
    
    // Verificar estrutura do primeiro TODO
    const firstTodo = todos[0];
    expect(firstTodo).toHaveProperty('id');
    expect(firstTodo).toHaveProperty('title');
    expect(firstTodo).toHaveProperty('description');
    expect(firstTodo).toHaveProperty('isCompleted');
    expect(firstTodo).toHaveProperty('createdAt');
    expect(firstTodo).toHaveProperty('updatedAt');
  });

  test('deve criar um novo TODO via API', async ({ request }) => {
    const newTodo = {
      title: 'Teste API - Novo TODO',
      description: 'Descrição do TODO criado via teste de API'
    };

    const response = await request.post(`${baseURL}/api/todos`, {
      data: newTodo
    });

    expect(response.status()).toBe(201);
    
    const createdTodo = await response.json();
    expect(createdTodo.title).toBe(newTodo.title);
    expect(createdTodo.description).toBe(newTodo.description);
    expect(createdTodo.isCompleted).toBe(false);
    expect(createdTodo.id).toBeDefined();
  });

  test('deve obter um TODO específico via API', async ({ request }) => {
    // Primeiro, listar todos os TODOs para obter um ID
    const listResponse = await request.get(`${baseURL}/api/todos`);
    expect(listResponse.status()).toBe(200);
    
    // Verificar se a resposta é válida antes de fazer parse
    const responseText = await listResponse.text();
    let todos;
    try {
      todos = JSON.parse(responseText);
    } catch (error) {
      throw new Error(`Failed to parse JSON response: ${responseText.substring(0, 100)}...`);
    }
    
    expect(todos.length).toBeGreaterThan(0);
    const todoId = todos[0].id;

    // Obter o TODO específico
    const response = await request.get(`${baseURL}/api/todos/${todoId}`);
    
    expect(response.status()).toBe(200);
    
    const todo = await response.json();
    expect(todo.id).toBe(todoId);
    expect(todo).toHaveProperty('title');
    expect(todo).toHaveProperty('description');
  });

  test('deve atualizar um TODO via API', async ({ request }) => {
    // Primeiro, criar um TODO para atualizar
    const newTodo = {
      title: 'TODO para Atualizar',
      description: 'Descrição original'
    };

    const createResponse = await request.post(`${baseURL}/api/todos`, {
      data: newTodo
    });
    const createdTodo = await createResponse.json();

    // Atualizar o TODO
    const updatedData = {
      title: 'TODO Atualizado',
      description: 'Descrição atualizada',
      isCompleted: true
    };

    const updateResponse = await request.put(`${baseURL}/api/todos/${createdTodo.id}`, {
      data: updatedData
    });

    expect(updateResponse.status()).toBe(200);
    
    const updatedTodo = await updateResponse.json();
    expect(updatedTodo.title).toBe(updatedData.title);
    expect(updatedTodo.description).toBe(updatedData.description);
    expect(updatedTodo.isCompleted).toBe(true);
  });

  test('deve alternar status de completado via API', async ({ request }) => {
    // Primeiro, listar todos os TODOs para obter um ID
    const listResponse = await request.get(`${baseURL}/api/todos`);
    const todos = await listResponse.json();
    const todo = todos[0];
    const originalStatus = todo.isCompleted;

    // Alternar o status
    const response = await request.patch(`${baseURL}/api/todos/${todo.id}/toggle`);
    
    expect(response.status()).toBe(200);
    
    const updatedTodo = await response.json();
    expect(updatedTodo.isCompleted).toBe(!originalStatus);
  });

  test('deve deletar um TODO via API', async ({ request }) => {
    // Primeiro, criar um TODO para deletar
    const newTodo = {
      title: 'TODO para Deletar',
      description: 'Este TODO será deletado'
    };

    const createResponse = await request.post(`${baseURL}/api/todos`, {
      data: newTodo
    });
    const createdTodo = await createResponse.json();

    // Deletar o TODO
    const deleteResponse = await request.delete(`${baseURL}/api/todos/${createdTodo.id}`);
    
    expect(deleteResponse.status()).toBe(204);

    // Verificar se o TODO foi realmente deletado
    const getResponse = await request.get(`${baseURL}/api/todos/${createdTodo.id}`);
    expect(getResponse.status()).toBe(404);
  });

  test('deve retornar 404 para TODO inexistente', async ({ request }) => {
    const nonExistentId = 99999;
    
    const response = await request.get(`${baseURL}/api/todos/${nonExistentId}`);
    
    expect(response.status()).toBe(404);
  });

  test('deve validar campos obrigatórios ao criar TODO', async ({ request }) => {
    const invalidTodo = {
      description: 'TODO sem título'
      // title é obrigatório
    };

    const response = await request.post(`${baseURL}/api/todos`, {
      data: invalidTodo
    });

    expect(response.status()).toBe(400);
  });
});