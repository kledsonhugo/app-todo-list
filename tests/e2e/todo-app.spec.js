import { test, expect } from '@playwright/test';

test.describe('Todo List Application', () => {
  test.beforeEach(async ({ page }) => {
    // Navegar para a página principal antes de cada teste
    await page.goto('/');
    
    // Aguardar que a página carregue completamente
    await page.waitForSelector('#todosList', { timeout: 10000 });
    await page.waitForTimeout(1000); // Aguardar estabilização
  });

  // Função auxiliar para limpar todas as tarefas
  async function clearAllTodos(page) {
    await page.goto('/');
    await page.waitForSelector('#todosList', { timeout: 10000 });
    
    // Tentar deletar até 10 tarefas (limite para evitar loops infinitos)
    for (let attempt = 0; attempt < 10; attempt++) {
      const todoItems = page.locator('.todo-item');
      const count = await todoItems.count();
      
      if (count === 0) break;
      
      try {
        const deleteButton = page.locator('.todo-item .delete-btn').first();
        await deleteButton.click({ timeout: 2000 });
        
        // Aguardar e aceitar diálogo
        await page.waitForTimeout(500);
        const dialog = await page.waitForEvent('dialog', { timeout: 2000 });
        await dialog.accept();
        
        await page.waitForTimeout(1000);
      } catch (error) {
        console.log(`Erro na tentativa ${attempt + 1}: ${error.message}`);
        break;
      }
    }
  }

  test('deve carregar a página principal corretamente', async ({ page }) => {
    // Verificar se o título da página está correto
    await expect(page).toHaveTitle('Todo List - Gerenciador de Tarefas');
    
    // Verificar se o cabeçalho principal está visível
    await expect(page.locator('h1')).toContainText('Minha Lista de Tarefas');
    
    // Verificar se o formulário de adicionar tarefa está presente
    await expect(page.locator('#addTodoForm')).toBeVisible();
    
    // Verificar se a seção de filtros está presente
    await expect(page.locator('.filters-section')).toBeVisible();
    
    // Verificar se a lista de tarefas está presente
    await expect(page.locator('#todosList')).toBeVisible();
  });

  test('deve exibir as tarefas padrão ao carregar', async ({ page }) => {
    // Aguardar que as tarefas sejam carregadas
    await page.waitForSelector('.todo-item', { timeout: 10000 });
    
    // Verificar se existem tarefas na lista
    const todoItems = page.locator('.todo-item');
    const count = await todoItems.count();
    await expect(count).toBeGreaterThan(0);
    
    // Verificar se a estrutura básica da interface está funcionando
    await expect(page.locator('.todo-item .todo-title').first()).toBeVisible();
    await expect(page.locator('.todo-item .toggle-btn').first()).toBeVisible();
  });

  test('deve criar uma nova tarefa', async ({ page }) => {
    // Aguardar que a página carregue
    await page.waitForSelector('#addTodoForm');
    
    // Contar tarefas antes da criação
    const todoItemsBefore = page.locator('.todo-item');
    const countBefore = await todoItemsBefore.count();
    
    // Preencher o formulário com título único
    const uniqueTitle = `Nova Tarefa Teste ${Date.now()}`;
    await page.fill('#todoTitle', uniqueTitle);
    await page.fill('#todoDescription', 'Descrição da nova tarefa criada pelo teste');
    
    // Submeter o formulário
    await page.click('button[type="submit"]');
    
    // Aguardar que a nova tarefa apareça na lista
    await page.waitForSelector(`text=${uniqueTitle}`, { timeout: 10000 });
    
    // Verificar se a nova tarefa foi adicionada
    await expect(page.locator(`text=${uniqueTitle}`).first()).toBeVisible();
    
    // Verificar se o número de tarefas aumentou
    const todoItemsAfter = page.locator('.todo-item');
    const countAfter = await todoItemsAfter.count();
    expect(countAfter).toBe(countBefore + 1);
  });

  test('deve marcar uma tarefa como concluída', async ({ page }) => {
    // Aguardar que as tarefas carreguem
    await page.waitForSelector('.todo-item', { timeout: 10000 });
    
    // Encontrar uma tarefa pendente e clicar no botão de concluir
    const toggleButton = page.locator('.todo-item .toggle-btn').filter({ hasText: 'Concluir' }).first();
    
    await toggleButton.click();
    
    // Aguardar um pouco para a atualização
    await page.waitForTimeout(2000);
    
    // Verificar se a tarefa foi marcada como concluída
    await expect(page.locator('.todo-item .toggle-btn').filter({ hasText: 'Reabrir' }).first()).toBeVisible();
  });

  test('deve filtrar tarefas por status', async ({ page }) => {
    // Aguardar que as tarefas carreguem
    await page.waitForSelector('.todo-item', { timeout: 10000 });
    
    // Contar todas as tarefas iniciais
    const allTodos = page.locator('.todo-item');
    const initialCount = await allTodos.count();
    
    // Filtrar por tarefas pendentes
    await page.click('[data-filter="pending"]');
    await page.waitForTimeout(500);
    
    // Verificar se o filtro está ativo
    await expect(page.locator('[data-filter="pending"]')).toHaveClass(/active/);
    
    // Filtrar por tarefas concluídas
    await page.click('[data-filter="completed"]');
    await page.waitForTimeout(500);
    
    // Verificar se o filtro está ativo
    await expect(page.locator('[data-filter="completed"]')).toHaveClass(/active/);
    
    // Voltar para mostrar todas
    await page.click('[data-filter="all"]');
    await page.waitForTimeout(500);
    
    // Verificar se todas as tarefas estão visíveis novamente
    await expect(page.locator('[data-filter="all"]')).toHaveClass(/active/);
  });

  test('deve abrir o modal de edição', async ({ page }) => {
    // Aguardar que as tarefas carreguem
    await page.waitForSelector('.todo-item', { timeout: 10000 });
    
    // Clicar no botão de editar da primeira tarefa
    const editButton = page.locator('.todo-item .edit-btn').first();
    await editButton.click();
    
    // Verificar se o modal foi aberto
    await expect(page.locator('#editModal')).toBeVisible();
    await expect(page.locator('#editModal h3')).toContainText('Editar Tarefa');
    
    // Fechar o modal
    await page.click('#closeModal');
    
    // Verificar se o modal foi fechado
    await expect(page.locator('#editModal')).toHaveClass(/hidden/);
  });

  test('deve atualizar a lista ao clicar em refresh', async ({ page }) => {
    // Aguardar que as tarefas carreguem
    await page.waitForSelector('.todo-item', { timeout: 10000 });
    
    // Clicar no botão de atualizar
    await page.click('#refreshBtn');
    
    // Verificar se o indicador de carregamento aparece (mesmo que brevemente)
    // e depois as tarefas são recarregadas
    await page.waitForSelector('.todo-item', { timeout: 10000 });
    
    // Verificar se ainda temos tarefas na lista
    const todoItems = page.locator('.todo-item');
    await expect(todoItems.first()).toBeVisible();
  });

  // Teste simplificado de exclusão de uma única tarefa
  test('deve excluir uma tarefa específica', async ({ page }) => {
    // Aguardar que as tarefas carreguem
    await page.waitForSelector('.todo-item', { timeout: 10000 });
    
    // Contar tarefas antes
    const todoItemsBefore = page.locator('.todo-item');
    const countBefore = await todoItemsBefore.count();
    
    if (countBefore > 0) {
      // Clicar no botão de excluir da primeira tarefa
      const deleteButton = page.locator('.todo-item .delete-btn').first();
      
      // Configurar handler para aceitar o diálogo
      page.on('dialog', dialog => dialog.accept());
      
      await deleteButton.click();
      
      // Aguardar que a tarefa seja removida
      await page.waitForTimeout(2000);
      
      // Verificar se o número de tarefas diminuiu
      const todoItemsAfter = page.locator('.todo-item');
      const countAfter = await todoItemsAfter.count();
      expect(countAfter).toBe(countBefore - 1);
    }
  });
});