# Todo List Web Interface Tests

Este projeto contém testes automatizados usando Playwright para validar todas as funcionalidades da interface web da aplicação Todo List.

## Pré-requisitos

1. .NET 8.0 SDK
2. Aplicação Todo List rodando em `http://localhost:5146`
3. Playwright browsers instalados

## Estrutura dos Testes

### `TodoListWebTests.cs`
Testes das funcionalidades principais da interface:
- ✅ Carregamento da página
- ✅ Adicionar nova tarefa
- ✅ Validação de campos obrigatórios
- ✅ Filtros de tarefas (Todas, Pendentes, Concluídas)
- ✅ Toggle de conclusão de tarefas
- ✅ Modal de edição
- ✅ Exclusão de tarefas com confirmação
- ✅ Botão de atualização

### `TodoListResponsiveTests.cs`
Testes de design responsivo:
- 📱 Layout desktop (1200px)
- 📱 Layout tablet (768px)
- 📱 Layout mobile (375px)
- 📱 Layout mobile pequeno (320px)
- 📱 Interação responsiva dos formulários
- 📱 Botões de filtro em diferentes tamanhos

### `TodoListErrorHandlingTests.cs`
Testes de tratamento de erros e casos extremos:
- ❌ Erro de servidor indisponível
- ❌ Erro de API (500)
- ❌ Validação de campos
- ❌ Textos longos
- ❌ Lista vazia
- ❌ JSON inválido
- ❌ Resposta lenta da rede
- ❌ Operações concorrentes
- ❌ Caracteres especiais
- ⌨️ Navegação por teclado
- ⌨️ Tecla Escape no modal

## Como Executar

### 1. Instalar Playwright (primeira vez)
```bash
dotnet tool install --global Microsoft.Playwright.CLI
playwright install
```

### 2. Iniciar a aplicação
```bash
cd /home/runner/work/app-todo-list/app-todo-list
dotnet run --project TodoListApp.csproj
```

### 3. Executar os testes
Em outro terminal:
```bash
cd /home/runner/work/app-todo-list/app-todo-list/tests
dotnet test
```

### Executar testes específicos
```bash
# Apenas testes de funcionalidades principais
dotnet test --filter "TestClass=TodoListWebTests"

# Apenas testes responsivos
dotnet test --filter "TestClass=TodoListResponsiveTests"

# Apenas testes de tratamento de erros
dotnet test --filter "TestClass=TodoListErrorHandlingTests"
```

### Executar com relatório detalhado
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Browsers Suportados

Os testes são executados automaticamente em:
- ✅ Chromium (Chrome/Edge)
- ✅ Firefox
- ✅ WebKit (Safari)

## Funcionalidades Testadas

### Interface Principal
- [x] Carregamento e renderização da página
- [x] Formulário de adição de tarefas
- [x] Lista de tarefas dinâmica
- [x] Botões de filtro (Todas/Pendentes/Concluídas)
- [x] Botões de ação (Completar/Editar/Excluir)
- [x] Modal de edição
- [x] Notificações (toast messages)
- [x] Indicadores de carregamento

### Validações
- [x] Campo título obrigatório
- [x] Limites de caracteres (título: 200, descrição: 500)
- [x] Confirmação de exclusão
- [x] Escape de caracteres especiais

### UX/UI
- [x] Design responsivo
- [x] Navegação por teclado
- [x] Acessibilidade básica
- [x] Feedback visual (loading, success, error)
- [x] Animações e transições

### Integração com API
- [x] Criação de tarefas
- [x] Listagem de tarefas
- [x] Atualização de tarefas
- [x] Exclusão de tarefas
- [x] Toggle de status
- [x] Tratamento de erros da API
- [x] Sincronização automática

## Relatórios de Teste

Os testes geram relatórios automáticos que incluem:
- Resultados por browser
- Screenshots em caso de falha
- Logs detalhados de execução
- Métricas de performance

## Manutenção

### Atualizar Playwright
```bash
playwright install --with-deps
```

### Debug dos testes
```bash
dotnet test --logger "console;verbosity=detailed" --filter "TestMethod=CanAddNewTodo"
```

### Executar com interface gráfica (apenas para desenvolvimento)
Modifique `GlobalSetup.cs` e altere `Headless = false` para ver os testes executando.

## Cobertura de Testes

✅ **100% das funcionalidades** da interface web estão cobertas:

1. **Adicionar Tarefas** - Formulário, validação, integração API
2. **Visualizar Tarefas** - Lista, filtros, estados visuais
3. **Editar Tarefas** - Modal, campos, salvamento
4. **Excluir Tarefas** - Confirmação, integração API
5. **Completar Tarefas** - Toggle, feedback visual
6. **Filtrar Tarefas** - Todos os filtros disponíveis
7. **Atualizar Lista** - Sincronização manual
8. **Responsividade** - Desktop, tablet, mobile
9. **Tratamento de Erros** - Rede, API, validação
10. **Acessibilidade** - Teclado, screen readers

## Contribuição

Para adicionar novos testes:
1. Identifique a funcionalidade a testar
2. Escolha a classe de teste apropriada
3. Siga o padrão AAA (Arrange, Act, Assert)
4. Use seletores CSS robustos
5. Adicione waits apropriados
6. Teste em múltiplos browsers