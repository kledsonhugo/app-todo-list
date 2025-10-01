# Configurações de Teste Playwright

Este projeto possui múltiplas configurações de teste Playwright para diferentes cenários e ambientes, organizadas com nomenclatura consistente.

## 📁 Estrutura de Arquivos

```
tests/
├── playwright.chromium.config.js         # Local - Chromium apenas
├── playwright.multi.config.js            # Local - Multi-browser  
├── playwright.azure.chromium.config.ts   # Azure - Chromium apenas
├── playwright.azure.multi.config.ts      # Azure - Multi-browser
├── package.json                          # Scripts npm
└── README.md                             # Esta documentação
```

## 🏠 Testes Locais

### Chromium Apenas (Rápido)
```bash
npm run test:local:chromium
# ou
npx playwright test --config=playwright.chromium.config.js
```
- **Arquivo**: `playwright.chromium.config.js`
- **Uso**: Desenvolvimento diário, testes rápidos
- **Browser**: Chromium apenas
- **Workers**: 4 localmente
- **Timeout**: 45 segundos

### Multi-Browser (Completo)
```bash
npm run test:local:multi
# ou
npx playwright test --config=playwright.multi.config.js
```
- **Arquivo**: `playwright.multi.config.js`
- **Uso**: Validação completa antes de commits
- **Browsers**: Chromium, Firefox, WebKit
- **Workers**: Configurável
- **Timeout**: Padrão Playwright

## ☁️ Testes Azure Playwright

### Azure Chromium (CI/CD Otimizado)
```bash
npm run test:azure:chromium
# ou
npx playwright test --config=playwright.azure.chromium.config.ts
```
- **Arquivo**: `playwright.azure.chromium.config.ts`
- **Uso**: CI/CD, testes automatizados rápidos
- **Browser**: Chromium apenas
- **Workers**: 10 (configurável via PLAYWRIGHT_WORKERS)
- **Timeout**: 60 segundos
- **Otimizações**: Máxima performance no Azure

### Azure Chromium Ultra-Rápido
```bash
npm run test:azure:chromium:fast
# ou
npx playwright test --config=playwright.azure.chromium.config.ts --workers=20
```
- **Uso**: Validação rápida em ambiente Azure
- **Workers**: 20 (máxima paralelização)

### Azure Multi-Browser (Validação Completa)
```bash
npm run test:azure:multi
# ou
npx playwright test --config=playwright.azure.multi.config.ts
```
- **Arquivo**: `playwright.azure.multi.config.ts`
- **Uso**: Validação completa cross-browser no Azure
- **Browsers**: Chromium, Firefox, WebKit
- **Workers**: 8 (configurável via PLAYWRIGHT_WORKERS)
- **Timeout**: 90 segundos
- **Retries**: 2 tentativas em caso de falha

### Azure Multi-Browser Otimizado
```bash
npm run test:azure:multi:complete
# ou
npx playwright test --config=playwright.azure.multi.config.ts --workers=12
```
- **Uso**: Validação completa otimizada
- **Workers**: 12 (balanceado para multi-browser)

## 📊 Comparação das Configurações

| Configuração | Arquivo | Ambiente | Browsers | Workers | Timeout | Uso Recomendado |
|-------------|---------|----------|----------|---------|---------|-----------------|
| `local:chromium` | `playwright.chromium.config.js` | Local | Chromium | 4 | 45s | Desenvolvimento diário |
| `local:multi` | `playwright.multi.config.js` | Local | Chrome, Firefox, Safari | Variável | 30s | Validação pré-commit |
| `azure:chromium` | `playwright.azure.chromium.config.ts` | Azure | Chromium | 10 | 60s | CI/CD automatizado |
| `azure:chromium:fast` | `playwright.azure.chromium.config.ts` | Azure | Chromium | 20 | 60s | Validação rápida |
| `azure:multi` | `playwright.azure.multi.config.ts` | Azure | Chrome, Firefox, Safari | 8 | 90s | Validação cross-browser |
| `azure:multi:complete` | `playwright.azure.multi.config.ts` | Azure | Chrome, Firefox, Safari | 12 | 90s | Teste completo otimizado |

## 🛠️ Configurações Específicas do Azure

### Otimizações Aplicadas:
- **Timeouts estendidos** para compensar latência de rede
- **Args do Chrome otimizados** para ambiente Azure
- **Configurações de rede** para loopback exposure
- **Credenciais Azure** via DefaultAzureCredential
- **Retry automático** em caso de falhas temporárias

### Variáveis de Ambiente:
- `PLAYWRIGHT_WORKERS`: Número de workers paralelos
- `CI`: Detecta ambiente CI para ajustes automáticos

## 🎯 Estratégia de Uso Recomendada

### Desenvolvimento Local:
1. **Durante desenvolvimento**: `npm run test:local:chromium`
2. **Antes de commit**: `npm run test:local:multi`

### Pipeline CI/CD:
1. **Pull Request**: `npm run test:azure:chromium:fast`
2. **Release**: `npm run test:azure:multi:complete`
3. **Nightly**: `npm run test:azure:multi`

### Debugging:
1. **Local com UI**: `npm run test:ui`
2. **Debug específico**: `npm run test:debug`
3. **Relatórios**: `npm run report`