# 🚀 CI/CD Pipelines - Todo List App

Este projeto possui uma estratégia completa de CI/CD com múltiplos pipelines especializados para diferentes cenários e ambientes.

## 📋 Visão Geral dos Pipelines

### 1. 🧪 **playwright-tests.yml** - Testes E2E Principais
**Trigger:** Push para qualquer branch
**Objetivo:** Validação contínua com testes E2E básicos
```yaml
Execução: A cada push
Browsers: Chromium (otimizado para velocidade)
Timeout: 30 minutos
Artefatos: Relatórios HTML, screenshots de falhas
```

**Casos de Uso:**
- Desenvolvimento diário
- Pull requests
- Validação rápida de mudanças

### 2. 🌐 **multi-browser-tests.yml** - Testes Cross-Browser
**Trigger:** Agendado (diário às 2:00 UTC) + Manual
**Objetivo:** Compatibilidade entre navegadores
```yaml
Execução: Agendada + workflow_dispatch
Browsers: Chromium, Firefox, WebKit (configurável)
Timeout: 45 minutos
Estratégia: fail-fast: false (continua mesmo com falhas)
```

**Casos de Uso:**
- Testes de compatibilidade
- Validação antes de releases importantes
- Debugging específico por browser

### 3. 🚀 **production-release.yml** - Pipeline de Produção
**Trigger:** Push para main + Tags + Releases
**Objetivo:** Validação completa e preparação para produção
```yaml
Execução: Push main, tags v*.*.*, releases
Stages: Code Quality → API Tests → E2E Tests → Security → Build
Timeout: Personalizado por job
Artefatos: Build de produção + relatórios completos
```

**Casos de Uso:**
- Releases para produção
- Validação de segurança
- Artefatos para deploy

## 🎯 Estratégia de Testes

### Pyramid de Testes Automatizados
```
           🎭 E2E Tests (UI + API)
          ┌─────────────────────┐
         │    Playwright Tests   │
        └─────────────────────────┘
       
      🧪 Integration Tests (API)
    ┌─────────────────────────────┐
   │     cURL + HTTP Tests        │
  └─────────────────────────────────┘
  
 🔧 Unit Tests (Code Quality)
┌───────────────────────────────────┐
│  dotnet format + Security Scans   │
└───────────────────────────────────┘
```

### Cobertura de Testes por Pipeline

| Pipeline | UI Tests | API Tests | Security | Multi-Browser | Artifacts |
|----------|----------|-----------|----------|---------------|-----------|
| **playwright-tests** | ✅ | ✅ | ❌ | ❌ | Basic |
| **multi-browser** | ✅ | ✅ | ❌ | ✅ | Per Browser |
| **production-release** | ✅ | ✅ | ✅ | ❌ | Full Build |

## 🔧 Configuração e Uso

### Execução Manual
```bash
# Pipeline principal (qualquer branch)
git push origin feature-branch

# Multi-browser (manual)
# Vá para Actions → Multi-Browser E2E Tests → Run workflow

# Pipeline de produção
git push origin main
# ou
git tag v1.0.0 && git push origin v1.0.0
```

### Parâmetros Configuráveis

#### Multi-Browser Pipeline
- **browsers**: `chromium`, `firefox`, `webkit` ou combinações
- **schedule**: Configurável no cron (padrão: diário 2:00 UTC)

#### Production Pipeline  
- **security-scan**: Habilitado apenas no branch main
- **artifacts**: Retention configurable (30-90 dias)

## 📊 Monitoramento e Relatórios

### Artefatos Gerados
```
playwright-tests.yml:
├── playwright-report/          # Relatório HTML principal
├── test-results/              # Screenshots de falhas
└── logs/                      # Logs de execução

multi-browser-tests.yml:
├── playwright-report-chromium/  # Por browser
├── playwright-report-firefox/
├── playwright-report-webkit/
└── playwright-screenshots-*/    # Screenshots por browser

production-release.yml:
├── todo-app-release-{sha}/     # Build para produção
├── production-e2e-report/     # Testes de produção
└── security-reports/          # Análises de segurança
```

### Dashboard de Status
Acesse em **Actions** tab no GitHub:
- ✅ Verde: Todos os testes passando
- ⚠️ Amarelo: Alguns browsers falhando (multi-browser)
- ❌ Vermelho: Falhas críticas

## 🚨 Troubleshooting

### Problemas Comuns

#### Timeouts
```yaml
# Se testes demoram muito:
timeout-minutes: 45  # Aumente conforme necessário
```

#### Falhas de Browser
```yaml
# Use apenas Chromium se WebKit/Firefox falharem:
matrix:
  browser: ["chromium"]
```

#### Problemas de Startup
```bash
# Verifique health check:
timeout 60 bash -c 'until curl -f http://localhost:5146/api/todos; do sleep 2; done'
```

### Debug de Pipelines
1. **Baixe artefatos** para análise local
2. **Screenshots** mostram estado da UI em falhas  
3. **Logs** contêm detalhes do console/network
4. **Use workflow_dispatch** para testes manuais

## 🔒 Considerações de Segurança

### Security Scans Incluídos
- **TruffleHog**: Detecção de secrets/credenciais
- **dotnet list package --vulnerable**: Vulnerabilidades em dependências
- **Code formatting**: Prevenção de issues de qualidade

### Secrets Management
```yaml
# Configure secrets no GitHub:
SONAR_TOKEN=          # Para análise de código
DEPLOY_KEY=           # Para deploy automático
NOTIFICATION_WEBHOOK= # Para notificações
```

## 📈 Melhorias Futuras

### Roadmap
- [ ] **Performance Tests**: Adicionar testes de carga
- [ ] **Accessibility Tests**: Validação de acessibilidade
- [ ] **Mobile Testing**: Testes em dispositivos mobile
- [ ] **Deployment**: Deploy automático para staging/produção
- [ ] **Monitoring**: Integração com ferramentas de APM

### Extensões Sugeridas
```yaml
# lighthouse-ci: Performance e SEO
# codecov: Cobertura de código  
# dependabot: Atualizações automáticas
# sonarcloud: Análise de qualidade avançada
```

## 🎉 Conclusão

Esta estratégia de CI/CD oferece:
- ✅ **Feedback rápido** em desenvolvimento (playwright-tests)
- ✅ **Compatibilidade garantida** entre browsers (multi-browser)  
- ✅ **Qualidade de produção** com segurança (production-release)
- ✅ **Flexibilidade** para diferentes cenários
- ✅ **Observabilidade** completa com artefatos e relatórios

Cada pipeline tem seu propósito específico, permitindo um desenvolvimento ágil com qualidade enterprise.