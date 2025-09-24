// Configurações da API
const API_BASE_URL = 'http://localhost:5146/api';
const API_ENDPOINTS = {
    todos: `${API_BASE_URL}/todos`
};

// Estado da aplicação
let currentFilter = 'all';
let todos = [];

// Elementos do DOM
const elements = {
    addTodoForm: document.getElementById('addTodoForm'),
    todoTitle: document.getElementById('todoTitle'),
    todoDescription: document.getElementById('todoDescription'),
    todosList: document.getElementById('todosList'),
    loading: document.getElementById('loading'),
    emptyMessage: document.getElementById('emptyMessage'),
    refreshBtn: document.getElementById('refreshBtn'),
    filterButtons: document.querySelectorAll('.btn-filter'),
    editModal: document.getElementById('editModal'),
    editTodoForm: document.getElementById('editTodoForm'),
    editTodoId: document.getElementById('editTodoId'),
    editTodoTitle: document.getElementById('editTodoTitle'),
    editTodoDescription: document.getElementById('editTodoDescription'),
    editTodoCompleted: document.getElementById('editTodoCompleted'),
    closeModal: document.getElementById('closeModal'),
    cancelEdit: document.getElementById('cancelEdit'),
    toast: document.getElementById('toast'),
    toastMessage: document.getElementById('toastMessage'),
    closeToast: document.getElementById('closeToast')
};

// Inicialização da aplicação
document.addEventListener('DOMContentLoaded', function() {
    initializeApp();
});

function initializeApp() {
    setupEventListeners();
    loadTodos();
}

function setupEventListeners() {
    // Formulário de adicionar tarefa
    elements.addTodoForm.addEventListener('submit', handleAddTodo);
    
    // Botão de atualizar
    elements.refreshBtn.addEventListener('click', loadTodos);
    
    // Filtros
    elements.filterButtons.forEach(button => {
        button.addEventListener('click', handleFilterChange);
    });
    
    // Modal de edição
    elements.closeModal.addEventListener('click', closeEditModal);
    elements.cancelEdit.addEventListener('click', closeEditModal);
    elements.editTodoForm.addEventListener('submit', handleEditTodo);
    
    // Toast
    elements.closeToast.addEventListener('click', hideToast);
    
    // Fechar modal ao clicar fora
    elements.editModal.addEventListener('click', function(e) {
        if (e.target === elements.editModal) {
            closeEditModal();
        }
    });
    
    // Fechar modal ao pressionar Escape
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape' && !elements.editModal.classList.contains('hidden')) {
            closeEditModal();
        }
    });
    
    // Fechar toast automaticamente
    setTimeout(hideToast, 5000);
}

// Funções da API
async function apiRequest(url, options = {}) {
    try {
        showLoading();
        const response = await fetch(url, {
            headers: {
                'Content-Type': 'application/json',
                ...options.headers
            },
            ...options
        });
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const data = response.status !== 204 ? await response.json() : null;
        return data;
    } catch (error) {
        console.error('API Error:', error);
        showToast(`Erro na API: ${error.message}`, 'error');
        throw error;
    } finally {
        hideLoading();
    }
}

async function loadTodos() {
    try {
        const data = await apiRequest(API_ENDPOINTS.todos);
        todos = data || [];
        renderTodos();
    } catch (error) {
        console.error('Erro ao carregar tarefas:', error);
    }
}

async function createTodo(todoData) {
    try {
        const newTodo = await apiRequest(API_ENDPOINTS.todos, {
            method: 'POST',
            body: JSON.stringify(todoData)
        });
        todos.push(newTodo);
        renderTodos();
        showToast('Tarefa criada com sucesso!', 'success');
        return newTodo;
    } catch (error) {
        console.error('Erro ao criar tarefa:', error);
    }
}

async function updateTodo(id, todoData) {
    try {
        const updatedTodo = await apiRequest(`${API_ENDPOINTS.todos}/${id}`, {
            method: 'PUT',
            body: JSON.stringify(todoData)
        });
        
        const index = todos.findIndex(todo => todo.id === id);
        if (index !== -1) {
            todos[index] = updatedTodo;
        }
        
        renderTodos();
        showToast('Tarefa atualizada com sucesso!', 'success');
        return updatedTodo;
    } catch (error) {
        console.error('Erro ao atualizar tarefa:', error);
    }
}

async function deleteTodo(id) {
    try {
        await apiRequest(`${API_ENDPOINTS.todos}/${id}`, {
            method: 'DELETE'
        });
        
        todos = todos.filter(todo => todo.id !== id);
        renderTodos();
        showToast('Tarefa excluída com sucesso!', 'success');
    } catch (error) {
        console.error('Erro ao excluir tarefa:', error);
    }
}

async function toggleTodoCompletion(id) {
    try {
        const updatedTodo = await apiRequest(`${API_ENDPOINTS.todos}/${id}/toggle`, {
            method: 'PATCH'
        });
        
        const index = todos.findIndex(todo => todo.id === id);
        if (index !== -1) {
            todos[index] = updatedTodo;
        }
        
        renderTodos();
        const status = updatedTodo.isCompleted ? 'concluída' : 'reaberta';
        showToast(`Tarefa ${status} com sucesso!`, 'success');
    } catch (error) {
        console.error('Erro ao alterar status da tarefa:', error);
    }
}

// Event Handlers
async function handleAddTodo(e) {
    e.preventDefault();
    
    const title = elements.todoTitle.value.trim();
    const description = elements.todoDescription.value.trim();
    
    if (!title) {
        showToast('O título da tarefa é obrigatório!', 'error');
        return;
    }
    
    const todoData = {
        title,
        description: description || null
    };
    
    const newTodo = await createTodo(todoData);
    if (newTodo) {
        elements.addTodoForm.reset();
    }
}

function handleFilterChange(e) {
    const filter = e.target.getAttribute('data-filter');
    currentFilter = filter;
    
    // Atualizar botões ativos
    elements.filterButtons.forEach(btn => btn.classList.remove('active'));
    e.target.classList.add('active');
    
    renderTodos();
}

async function handleEditTodo(e) {
    e.preventDefault();
    
    const id = parseInt(elements.editTodoId.value);
    const title = elements.editTodoTitle.value.trim();
    const description = elements.editTodoDescription.value.trim();
    const isCompleted = elements.editTodoCompleted.checked;
    
    if (!title) {
        showToast('O título da tarefa é obrigatório!', 'error');
        return;
    }
    
    const todoData = {
        title,
        description: description || null,
        isCompleted
    };
    
    const updatedTodo = await updateTodo(id, todoData);
    if (updatedTodo) {
        closeEditModal();
    }
}

// Funções de renderização
function renderTodos() {
    const filteredTodos = filterTodos(todos);
    
    if (filteredTodos.length === 0) {
        elements.todosList.innerHTML = '';
        elements.emptyMessage.classList.remove('hidden');
        return;
    }
    
    elements.emptyMessage.classList.add('hidden');
    
    const todosHTML = filteredTodos.map(todo => createTodoHTML(todo)).join('');
    elements.todosList.innerHTML = todosHTML;
    
    // Adicionar event listeners para os botões
    attachTodoEventListeners();
}

function filterTodos(todos) {
    switch (currentFilter) {
        case 'completed':
            return todos.filter(todo => todo.isCompleted);
        case 'pending':
            return todos.filter(todo => !todo.isCompleted);
        default:
            return todos;
    }
}

function createTodoHTML(todo) {
    const createdDate = new Date(todo.createdAt).toLocaleDateString('pt-BR');
    const completedDate = todo.completedAt ? new Date(todo.completedAt).toLocaleDateString('pt-BR') : null;
    
    return `
        <div class="todo-item ${todo.isCompleted ? 'completed' : ''}" data-id="${todo.id}">
            <div class="todo-header">
                <h3 class="todo-title">${escapeHtml(todo.title)}</h3>
                <div class="todo-actions">
                    <button class="btn btn-small btn-success toggle-btn" data-id="${todo.id}">
                        <i class="fas ${todo.isCompleted ? 'fa-undo' : 'fa-check'}"></i>
                        ${todo.isCompleted ? 'Reabrir' : 'Concluir'}
                    </button>
                    <button class="btn btn-small btn-secondary edit-btn" data-id="${todo.id}">
                        <i class="fas fa-edit"></i> Editar
                    </button>
                    <button class="btn btn-small btn-danger delete-btn" data-id="${todo.id}">
                        <i class="fas fa-trash"></i> Excluir
                    </button>
                </div>
            </div>
            
            ${todo.description ? `<p class="todo-description">${escapeHtml(todo.description)}</p>` : ''}
            
            <div class="todo-meta">
                <div class="todo-dates">
                    <small>Criada em: ${createdDate}</small>
                    ${completedDate ? `<br><small>Concluída em: ${completedDate}</small>` : ''}
                </div>
                <div class="todo-status ${todo.isCompleted ? 'completed' : 'pending'}">
                    <i class="fas ${todo.isCompleted ? 'fa-check-circle' : 'fa-clock'}"></i>
                    ${todo.isCompleted ? 'Concluída' : 'Pendente'}
                </div>
            </div>
        </div>
    `;
}

function attachTodoEventListeners() {
    // Botões de toggle
    document.querySelectorAll('.toggle-btn').forEach(btn => {
        btn.addEventListener('click', function() {
            const id = parseInt(this.getAttribute('data-id'));
            toggleTodoCompletion(id);
        });
    });
    
    // Botões de editar
    document.querySelectorAll('.edit-btn').forEach(btn => {
        btn.addEventListener('click', function() {
            const id = parseInt(this.getAttribute('data-id'));
            openEditModal(id);
        });
    });
    
    // Botões de excluir
    document.querySelectorAll('.delete-btn').forEach(btn => {
        btn.addEventListener('click', function() {
            const id = parseInt(this.getAttribute('data-id'));
            if (confirm('Tem certeza que deseja excluir esta tarefa?')) {
                deleteTodo(id);
            }
        });
    });
}

// Funções do modal
function openEditModal(id) {
    const todo = todos.find(t => t.id === id);
    if (!todo) return;
    
    elements.editTodoId.value = todo.id;
    elements.editTodoTitle.value = todo.title;
    elements.editTodoDescription.value = todo.description || '';
    elements.editTodoCompleted.checked = todo.isCompleted;
    
    elements.editModal.classList.remove('hidden');
}

function closeEditModal() {
    elements.editModal.classList.add('hidden');
    elements.editTodoForm.reset();
}

// Funções de UI
function showLoading() {
    elements.loading.classList.remove('hidden');
}

function hideLoading() {
    elements.loading.classList.add('hidden');
}

function showToast(message, type = 'success') {
    elements.toastMessage.textContent = message;
    elements.toast.className = `toast ${type}`;
    elements.toast.classList.add('show');
    
    // Auto-hide após 5 segundos
    setTimeout(hideToast, 5000);
}

function hideToast() {
    elements.toast.classList.remove('show');
}

// Funções utilitárias
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}
