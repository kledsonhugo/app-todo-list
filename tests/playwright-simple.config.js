import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  fullyParallel: true, // Permitir paralelização total
  retries: process.env.CI ? 2 : 1, // Mais retries em CI
  workers: process.env.CI 
    ? parseInt(process.env.PLAYWRIGHT_WORKERS) || 4 // Converter para número
    : '25%', // 25% localmente
  reporter: process.env.CI ? [['html'], ['github']] : 'list',
  timeout: 90000, // Timeout maior para acomodar todos os browsers
  expect: {
    timeout: 15000, // Timeout para expects
  },
  globalTimeout: process.env.CI ? 600000 : 300000, // 10min CI, 5min local
  use: {
    baseURL: 'http://localhost:5146',
    headless: process.env.CI ? true : false, // Headless em CI, headed localmente
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    // Configurações de performance globais
    actionTimeout: 25000, // Timeout base para ações
    navigationTimeout: 60000, // Timeout maior para navegação
    // Configurações de retry para estabilidade
    trace: 'retain-on-failure',
    // Aguardar elementos serem estáveis antes de interagir
    waitForSelectorTimeout: 30000,
    // Acelerar testes
    launchOptions: {
      args: process.env.CI ? [
        '--no-sandbox',
        '--disable-setuid-sandbox',
        '--disable-dev-shm-usage',
        '--disable-gpu',
        '--no-first-run'
      ] : []
    }
  },
  // Projetos baseados no ambiente
  projects: process.env.CI ? [
    // CI: Todos os browsers com configuração robusta
    {
      name: 'chromium',
      use: { 
        ...devices['Desktop Chrome'],
        viewport: { width: 1280, height: 720 },
        launchOptions: {
          args: [
            '--no-sandbox',
            '--disable-setuid-sandbox',
            '--disable-dev-shm-usage',
            '--disable-gpu',
            '--no-first-run'
          ]
        }
      },
    },
    {
      name: 'firefox',
      use: { 
        ...devices['Desktop Firefox'],
        viewport: { width: 1280, height: 720 },
        actionTimeout: 25000,
        launchOptions: {
          firefoxUserPrefs: {
            'dom.ipc.processCount': 1,
            'browser.cache.disk.enable': false,
            'browser.cache.memory.enable': false
          }
        }
      },
    },
    {
      name: 'webkit',
      use: { 
        ...devices['Desktop Safari'],
        actionTimeout: 30000,
        // WebKit não suporta argumentos do Chrome - usar configuração limpa
        launchOptions: {
          // Argumentos mínimos apenas para headless mode
          args: ['--headless']
        }
      },
    },
  ] : [
    // Local: Apenas Chromium e Firefox (WebKit problemático localmente)
    {
      name: 'chromium',
      use: { 
        ...devices['Desktop Chrome'],
        viewport: { width: 1280, height: 720 }
      },
    },
    {
      name: 'firefox',
      use: { 
        ...devices['Desktop Firefox'],
        viewport: { width: 1280, height: 720 },
        actionTimeout: 25000,
        launchOptions: {
          firefoxUserPrefs: {
            'dom.ipc.processCount': 1,
            'browser.cache.disk.enable': false,
            'browser.cache.memory.enable': false
          }
        }
      },
    },
  ],
  webServer: {
    command: 'echo "App should already be running"',
    url: 'http://localhost:5146',
    reuseExistingServer: true,
  },
});