import { defineConfig, devices } from '@playwright/test';
import { createAzurePlaywrightConfig, ServiceOS } from '@azure/playwright';
import { DefaultAzureCredential } from '@azure/identity';

/* Azure Playwright Configuration - Multi-Browser (Complete Testing) */
const multiBrowserConfig = defineConfig({
  testDir: './e2e',
  fullyParallel: true,
  retries: process.env.CI ? 3 : 1, // Mais retries para Azure
  workers: process.env.CI 
    ? parseInt(process.env.PLAYWRIGHT_WORKERS || '8') || 8 // Menos workers para multi-browser
    : 4, // 4 workers para execução local
  reporter: process.env.CI ? [['html'], ['github']] : 'list',
  timeout: 120000, // Timeout maior para Azure multi-browser
  expect: {
    timeout: 15000, // Timeout de expectativa aumentado para Azure
  },
  use: {
    baseURL: 'http://localhost:5146',
    headless: true,
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    // Configurações otimizadas para Azure multi-browser
    actionTimeout: 30000, // Timeout de ação aumentado
    navigationTimeout: 90000, // Timeout de navegação aumentado
    // Configurações gerais para todos os browsers
  },

  projects: [
    {
      name: 'chromium',
      use: { 
        ...devices['Desktop Chrome'],
        viewport: { width: 1280, height: 720 },
        ignoreHTTPSErrors: true,
        // Argumentos específicos para Chromium no Azure
        launchOptions: {
          args: [
            '--no-sandbox',
            '--disable-setuid-sandbox',
            '--disable-dev-shm-usage',
            '--disable-gpu',
            '--no-first-run',
            '--no-zygote',
            '--deterministic-fetch',
            '--disable-features=TranslateUI',
            '--disable-extensions',
            '--disable-background-timer-throttling',
            '--disable-backgrounding-occluded-windows',
            '--disable-renderer-backgrounding',
            '--disable-ipc-flooding-protection',
          ],
        },
      },
    },
    {
      name: 'firefox',
      use: { 
        ...devices['Desktop Firefox'],
        viewport: { width: 1280, height: 720 },
        ignoreHTTPSErrors: true,
        // Firefox não precisa dos argumentos Chromium
      },
    },
    {
      name: 'webkit',
      use: { 
        ...devices['Desktop Safari'],
        viewport: { width: 1280, height: 720 },
        ignoreHTTPSErrors: true,
        // WebKit não aceita argumentos Chromium, mantém configuração limpa
      },
    },
  ],

  /* Run your local dev server before starting the tests */
  webServer: {
    command: 'dotnet run --project ../TodoListApp.csproj',
    url: 'http://localhost:5146',
    reuseExistingServer: !process.env.CI,
    timeout: 120 * 1000, // 2 minutes timeout
  },
});

/* Learn more about service configuration at https://aka.ms/pww/docs/config */
export default defineConfig(
  multiBrowserConfig,
  createAzurePlaywrightConfig(multiBrowserConfig, {
    exposeNetwork: '<loopback>',
    connectTimeout: 5 * 60 * 1000, // 5 minutes for Azure
    os: ServiceOS.LINUX,
    credential: new DefaultAzureCredential(),
  })
);