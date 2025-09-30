# 🚀 Playwright Performance Optimization

## 📊 Configurações de Paralelização

### Workers Strategy

#### CI Environment (GitHub Actions)
- **Chromium-only** (`playwright-chromium.config.js`): **2 workers**
  - Execução rápida para feedback imediato
  - Otimizado para pipeline principal
  - Tempo estimado: ~1-2 minutos

- **Multi-browser** (`playwright-simple.config.js`): **1 worker**  
  - Execução sequencial por projeto de navegador
  - Evita conflitos de recursos em CI
  - Tempo estimado: ~3-4 minutos

#### Local Development
- **Chromium-only**: **50% dos cores** disponíveis
- **Multi-browser**: **25% dos cores** disponíveis

### Paralelização Settings

```javascript
fullyParallel: true        // Permite paralelização total
workers: process.env.CI    // Workers dinâmicos por ambiente
  ? [config-specific]      // CI: valores fixos otimizados
  : 'percentage%'          // Local: porcentagem dos cores
```

## ⚡ Otimizações de Performance

### Browser Launch Arguments (CI)
- `--no-sandbox` - Remove sandbox para CI
- `--disable-setuid-sandbox` - Desabilita sandbox adicional
- `--disable-dev-shm-usage` - Evita problemas de memória compartilhada
- `--disable-gpu` - Remove aceleração GPU desnecessária
- `--no-first-run` - Pula configuração inicial
- `--deterministic-fetch` - Torna fetches mais determinísticos
- `--disable-features=TranslateUI` - Remove recursos desnecessários

### Timeouts Otimizados
- **Action timeout**: 15s (chromium) / 20s (multi-browser)
- **Navigation timeout**: 30s (chromium) / 45s (multi-browser)  
- **Test timeout**: 45s (chromium) / 60s (multi-browser)

### Retry Strategy
- **CI**: 2 retries (maior confiabilidade)
- **Local**: 1 retry (desenvolvimento rápido)

## 📈 Performance Impact

### Antes (Sequential)
- 16 testes × 1 worker = ~8-10 minutos
- Sem paralelização = tempo linear

### Depois (Parallel)
- **Chromium CI**: 16 testes ÷ 2 workers = ~2-3 minutos ⚡
- **Multi-browser CI**: 16 testes × 3 browsers ÷ 1 worker = ~4-5 minutos
- **Local Chromium**: 16 testes ÷ 4+ workers = ~1-2 minutos ⚡⚡

### Speedup Estimado
- **CI Chromium**: ~70% reduction (10min → 3min)
- **CI Multi-browser**: ~50% reduction (15min → 7min)
- **Local Development**: ~80% reduction (10min → 2min)

## 🎯 Best Practices Implemented

### 1. Environment-Aware Configuration
```javascript
workers: process.env.CI ? 2 : '50%'
retries: process.env.CI ? 2 : 1
```

### 2. Smart Resource Management
- CI: Fixed workers para previsibilidade
- Local: Percentage workers para flexibilidade

### 3. Browser-Specific Optimizations
- Chromium: Argumentos otimizados para velocidade
- Multi-browser: Configuração conservadora para estabilidade

### 4. Monitoring & Debugging
- Screenshots apenas em falhas
- Vídeos apenas em falhas
- Relatórios HTML detalhados em CI

## 📋 Monitoramento

### Métricas de Performance
1. **Tempo de execução total**
2. **Taxa de sucesso dos testes**
3. **Uso de recursos (CPU/Memory)**
4. **Flakiness rate** com paralelização

### Logs de Performance
```bash
# Ver estatísticas de execução
npx playwright test --reporter=html

# Executar com debug de performance
DEBUG=pw:api npx playwright test
```

## 🔧 Troubleshooting

### Se testes ficarem instáveis:
1. Reduzir workers: `workers: 1`
2. Aumentar timeouts
3. Verificar race conditions nos testes
4. Usar `test.describe.serial()` para testes dependentes

### Para otimizar ainda mais:
1. Usar `test.beforeAll()` para setup compartilhado
2. Implementar data isolation entre testes
3. Usar page objects para reduzir duplicação
4. Considerar test sharding para suites grandes

## 📊 Configuração por Cenário

| Cenário | Config File | Workers | Tempo Estimado |
|---------|-------------|---------|----------------|
| CI Fast Feedback | `playwright-chromium.config.js` | 2 | ~2-3 min |
| CI Multi-browser | `playwright-simple.config.js` | 1 | ~4-5 min |
| Local Development | `playwright-chromium.config.js` | 50% cores | ~1-2 min |
| Local Full Test | `playwright-simple.config.js` | 25% cores | ~3-4 min |