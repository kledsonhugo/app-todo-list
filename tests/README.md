# Testes de Interface - Todo List App

Este diretório contém a suite completa de testes end-to-end (E2E) para a aplicação Todo List usando Playwright.

## 📊 Cobertura de Testes

✅ **100% de cobertura** da interface web  
✅ **29 testes** passando  
⚡ **~30-35 segundos** de execução

## 🧪 Suite de Testes

### Funcionalidades Principais (8 testes)
- ✅ Carregamento da página principal
- ✅ Exibição de tarefas padrão
- ✅ Criação de nova tarefa
- ✅ Marcar tarefa como concluída
- ✅ Filtrar tarefas (Todas/Pendentes/Concluídas)
- ✅ Abrir modal de edição
- ✅ Atualizar lista (refresh)
- ✅ Excluir tarefa

### Modal de Edição (3 testes)
- ✅ Editar tarefa completamente
- ✅ Fechar modal clicando fora
- ✅ Fechar modal via botão Cancelar

### Funcionalidades de Status (1 teste)
- ✅ Reabrir tarefa concluída

### Validações de Formulário (2 testes)
- ✅ Validar título obrigatório (criar)
- ✅ Validar título obrigatório (editar)

### Notificações e Estados (3 testes)
- ✅ Exibir toast de sucesso
- ✅ Fechar toast via botão
- ✅ Exibir mensagem de estado vazio

### Recursos de Interface (2 testes)
- ✅ Carregamento de CSS
- ✅ Carregamento de ícones Font Awesome

### Estrutura e Elementos (2 testes)
- ✅ Verificar elementos principais
- ✅ Contador de tarefas correto

## 🚀 Como Executar

### Pré-requisitos
- Node.js 18+
- npm ou yarn
- Aplicação rodando em `http://localhost:5146`

### Instalação
```bash
cd tests
npm install
```

### Executar Testes

```bash
# Executar todos os testes
npm test

# Executar com interface gráfica
npm run test:ui

# Executar com browser visível
npm run test:headed

# Executar em modo debug
npm run test:debug

# Ver relatório HTML
npm run report

# Executar configuração específica
npm run test:local:chromium
npm run test:local:multi
```

## 📁 Estrutura

```
tests/
├── e2e/
│   ├── todo-app.spec.js    # Testes da interface web (21 testes)
│   └── api.spec.js         # Testes da API REST (8 testes)
├── playwright.config.js    # Configuração principal
├── playwright.*.config.js  # Configurações específicas
├── package.json
└── README.md
```

## ⚙️ Configuração

### playwright.config.js
- **Base URL**: http://localhost:5146
- **Browser**: Chromium headless
- **Workers**: 4 (paralelo)
- **Timeout**: 45s por teste
- **Retries**: Configurável (2 em CI)
- **Screenshots**: Apenas em falhas
- **Video**: Desabilitado

## 🎯 Boas Práticas Implementadas

1. **Testes Isolados**: Cada teste é independente
2. **Cleanup Automático**: Tarefas de teste são identificáveis
3. **Espera Inteligente**: Uso de `waitForSelector` e `waitForTimeout` apropriados
4. **Retry Logic**: Configurado para lidar com flakiness
5. **Assertions Claras**: Mensagens de erro descritivas
6. **Parallel Execution**: Testes executam em paralelo para velocidade

## 🐛 Debug

### Ver screenshots de falhas
```bash
ls -la test-results/
```

### Executar teste específico
```bash
npx playwright test --grep "deve criar uma nova tarefa"
```

### Executar com trace
```bash
npx playwright test --trace on
```

## 📝 Notas Técnicas

### Checkboxes Customizados
Os checkboxes da interface usam CSS customizado, então os testes usam JavaScript para marcar/desmarcar:
```javascript
await page.evaluate(() => {
  document.getElementById('editTodoCompleted').checked = true;
});
```

### Toast Notifications
O toast pode estar fora do viewport, então usamos JavaScript para clicar:
```javascript
await page.evaluate(() => {
  document.getElementById('closeToast').click();
});
```

### Validações HTML5
Os testes verificam validações nativas do HTML5:
```javascript
const isInvalid = await titleInput.evaluate((el) => !el.validity.valid);
```

## 🔄 CI/CD

Os testes estão configurados para rodar em pipelines CI/CD com:
- GitHub Actions support
- Azure Playwright integration
- Automatic retries
- HTML report generation

## 📈 Métricas

- **Total de Testes**: 29
- **Taxa de Sucesso**: 100%
- **Tempo Médio**: 30-35s
- **Cobertura**: 100% da interface

## 🤝 Contribuindo

Para adicionar novos testes:

1. Adicione o teste em `e2e/todo-app.spec.js`
2. Use a estrutura existente como referência
3. Execute `npm test` para validar
4. Certifique-se de que todos os testes passam

## 📚 Recursos

- [Playwright Documentation](https://playwright.dev)
- [Best Practices](https://playwright.dev/docs/best-practices)
- [API Reference](https://playwright.dev/docs/api/class-playwright)
