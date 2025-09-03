# 🌐 Interface Web - Todo List

## Como Usar a Interface Web

A interface web está disponível em: **http://localhost:5146**

### 🚀 Começando

1. **Abra o navegador** e acesse http://localhost:5146
2. **Visualize as tarefas existentes** - A aplicação já vem com 3 exemplos
3. **Comece a gerenciar suas tarefas!**

### ✨ Funcionalidades Principais

#### 📝 Adicionar Nova Tarefa
1. Na seção "Adicionar Nova Tarefa"
2. Digite o **título** (obrigatório)
3. Adicione uma **descrição** (opcional)
4. Clique em **"Adicionar Tarefa"**
5. A tarefa aparecerá automaticamente na lista

#### 🔍 Filtrar Tarefas
- **Todas**: Mostra todas as tarefas
- **Pendentes**: Apenas tarefas não concluídas
- **Concluídas**: Apenas tarefas finalizadas

#### ✅ Marcar como Concluída
- Clique no botão **"Concluir"** (ícone de check)
- A tarefa ficará com fundo verde e texto riscado
- Para reabrir, clique em **"Reabrir"** (ícone de desfazer)

#### ✏️ Editar Tarefa
1. Clique no botão **"Editar"** (ícone de lápis)
2. Um modal abrirá com os dados da tarefa
3. Modifique o que desejar
4. Marque/desmarque "Tarefa concluída" se necessário
5. Clique em **"Salvar Alterações"**

#### 🗑️ Excluir Tarefa
1. Clique no botão **"Excluir"** (ícone de lixeira)
2. Confirme a exclusão no popup
3. A tarefa será removida permanentemente

#### 🔄 Atualizar Lista
- Clique no botão **"Atualizar"** para sincronizar com o servidor

### 🎨 Recursos Visuais

#### 🎯 Status das Tarefas
- **Verde com ✓**: Tarefa concluída
- **Laranja com 🕐**: Tarefa pendente
- **Texto riscado**: Tarefa finalizada

#### 📱 Design Responsivo
- **Desktop**: Layout completo com todas as funcionalidades
- **Tablet**: Layout adaptado para tela média
- **Mobile**: Interface otimizada para celular com botões maiores

#### 🔔 Notificações
- **Sucesso (verde)**: Operação realizada com êxito
- **Erro (vermelho)**: Problema na operação
- **Auto-close**: Notificações somem automaticamente em 5 segundos

### ⌨️ Atalhos e Dicas

#### 💡 Dicas de Uso
1. **Títulos descritivos**: Use títulos claros e objetivos
2. **Descrições úteis**: Adicione detalhes importantes na descrição
3. **Organize por filtros**: Use os filtros para focar no que importa
4. **Edite rapidamente**: Duplo-clique no título para editar rapidamente

#### 🚀 Produtividade
- **Trabalhe com filtros**: Use "Pendentes" para focar no que precisa fazer
- **Marque como concluída**: Assim que terminar uma tarefa, marque-a
- **Revise concluídas**: Use o filtro "Concluídas" para ver seu progresso
- **Atualize regularmente**: Mantenha a lista sempre atualizada

### 🔧 Solução de Problemas

#### ❌ Erro 404 ao tentar editar/excluir
- **Causa**: Tarefa não existe ou foi removida
- **Solução**: Clique em "Atualizar" para sincronizar a lista

#### ⚠️ Formulário não envia
- **Causa**: Título vazio (campo obrigatório)
- **Solução**: Digite um título para a tarefa

#### 🌐 Página não carrega
- **Causa**: Servidor não está rodando
- **Solução**: Execute `dotnet run` no terminal

#### 📱 Layout quebrado no mobile
- **Causa**: Zoom muito alto ou muito baixo
- **Solução**: Ajuste o zoom para 100% no navegador

### 🎯 Exemplos de Uso

#### 📚 Para Estudos
```
✅ Estudar .NET Core
✅ Fazer exercícios de programação
⏳ Ler documentação do ASP.NET
⏳ Praticar com APIs REST
```

#### 🏠 Para Casa
```
✅ Fazer compras no mercado
⏳ Organizar o escritório
⏳ Pagar as contas do mês
✅ Limpar a casa
```

#### 💼 Para Trabalho
```
⏳ Reunião com cliente às 14h
✅ Enviar relatório mensal
⏳ Revisar código do projeto
⏳ Planejar sprint da próxima semana
```

### 🎨 Personalização

A interface foi criada com:
- **Cores**: Gradient roxo/azul elegante
- **Ícones**: Font Awesome para clareza visual
- **Animações**: Transições suaves para melhor UX
- **Tipografia**: Fonte Segoe UI para legibilidade

### 🔄 Sincronização

A interface mantém sincronização automática com a API:
- **Criação**: Nova tarefa aparece imediatamente
- **Edição**: Mudanças são refletidas instantaneamente
- **Exclusão**: Tarefa some da lista em tempo real
- **Toggle**: Status atualiza imediatamente

### 📞 Suporte

Se encontrar problemas:
1. Verifique se o servidor está rodando (`dotnet run`)
2. Teste a API diretamente em http://localhost:5146/api/docs
3. Verifique o console do navegador (F12) para erros JavaScript
4. Recarregue a página (Ctrl+F5 ou Cmd+Shift+R)
