import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  fullyParallel: true, // Permitir paralelização total
  retries: process.env.CI ? 2 : 1, // Mais retries em CI
  workers: process.env.CI 
    ? parseInt(process.env.PLAYWRIGHT_WORKERS) || 4 // Converter para número
    : '50%', // 50% dos cores localmente
  reporter: process.env.CI ? [['html'], ['github']] : 'list',
  timeout: 45000, // Timeout otimizado
  use: {
    baseURL: 'http://localhost:5146',
    headless: process.env.CI ? true : false, // Headless em CI, headed localmente
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    // Configurações de performance
    actionTimeout: 15000,
    navigationTimeout: 30000,
    // Acelerar testes
    launchOptions: {
      args: process.env.CI ? [
        '--no-sandbox',
        '--disable-setuid-sandbox',
        '--disable-dev-shm-usage',
        '--disable-gpu',
        '--no-first-run',
        '--no-zygote',
        '--deterministic-fetch',
        '--disable-features=TranslateUI',
        '--disable-background-networking'
      ] : []
    }
  },
  projects: [
    {
      name: 'chromium',
      use: { 
        ...devices['Desktop Chrome'],
        viewport: { width: 1280, height: 720 }
      },
    },
  ],
  webServer: {
    command: 'echo "App should already be running"',
    url: 'http://localhost:5146',
    reuseExistingServer: true,
  },
});