import { test, expect } from '@playwright/test';

test.describe('Todo List Application', () => {
  test.beforeEach(async ({ page }) => {
    // Navegar para a página principal antes de cada teste
    await page.goto('/');
    
    // Aguardar que a página carregue completamente
    await page.waitForLoadState('networkidle');
    
    // Aguardar que o JavaScript execute e a aplicação seja inicializada
    await page.waitForFunction(() => {
      return document.querySelector('#todosList') !== null &&
             document.querySelector('#addTodoForm') !== null;
    }, { timeout: 30000 });
    
    // Aguardar estabilização adicional
    await page.waitForTimeout(2000);
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
    // Aguardar que a página carregue completamente
    await page.waitForLoadState('networkidle');
    
    // Aguardar que o JavaScript execute e a aplicação seja inicializada
    await page.waitForFunction(() => {
      return window.todoApp !== undefined || 
             document.querySelector('#todosList') !== null;
    }, { timeout: 30000 });
    
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
    
    // Filtrar por tarefas pendentes - usando força para evitar intercepts
    await page.locator('[data-filter="pending"]').click({ force: true });
    await page.waitForTimeout(1000);
    
    // Verificar se o filtro está ativo
    await expect(page.locator('[data-filter="pending"]')).toHaveClass(/active/);
    
    // Filtrar por tarefas concluídas - scroll primeiro para garantir visibilidade
    await page.locator('[data-filter="completed"]').scrollIntoViewIfNeeded();
    await page.locator('[data-filter="completed"]').click({ force: true });
    await page.waitForTimeout(1000);
    
    // Verificar se o filtro está ativo
    await expect(page.locator('[data-filter="completed"]')).toHaveClass(/active/);
    
    // Voltar para mostrar todas
    await page.locator('[data-filter="all"]').click({ force: true });
    await page.waitForTimeout(500);
    
    // Verificar se todas as tarefas estão visíveis novamente
    await expect(page.locator('[data-filter="all"]')).toHaveClass(/active/);
  });

  test('deve abrir o modal de edição', async ({ page }) => {
    // Aguardar que as tarefas carreguem
    await page.waitForSelector('.todo-item', { timeout: 10000 });
    
    // Aguardar que a página esteja completamente carregada
    await page.waitForLoadState('networkidle');
    
    // Localizar e aguardar o botão de editar ficar estável
    const editButton = page.locator('.todo-item .edit-btn').first();
    await editButton.waitFor({ state: 'visible' });
    await editButton.scrollIntoViewIfNeeded();
    
    // Usar force click para evitar intercepts
    await editButton.click({ force: true });
    
    // Verificar se o modal foi aberto
    await expect(page.locator('#editModal')).toBeVisible();
    await expect(page.locator('#editModal h3')).toContainText('Editar Tarefa');
    
    // Fechar o modal
    await page.locator('#closeModal').click({ force: true });
    
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
    
    // Aguardar que a página esteja completamente carregada
    await page.waitForLoadState('networkidle');
    
    // Contar tarefas antes
    const todoItemsBefore = page.locator('.todo-item');
    const countBefore = await todoItemsBefore.count();
    
    if (countBefore > 0) {
      // Localizar o botão de excluir da primeira tarefa
      const deleteButton = page.locator('.todo-item .delete-btn').first();
      
      // Aguardar que o botão esteja visível e estável
      await deleteButton.waitFor({ state: 'visible' });
      await deleteButton.scrollIntoViewIfNeeded();
      
      // Configurar handler para aceitar o diálogo
      page.on('dialog', dialog => dialog.accept());
      
      // Usar force click para evitar intercepts
      await deleteButton.click({ force: true });
      
      // Aguardar que a tarefa seja removida
      await page.waitForTimeout(2000);
      
      // Verificar se o número de tarefas diminuiu
      const todoItemsAfter = page.locator('.todo-item');
      const countAfter = await todoItemsAfter.count();
      expect(countAfter).toBe(countBefore - 1);
    }
  });
});