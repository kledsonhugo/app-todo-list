import { defineConfig, devices } from '@playwright/test';
import { createAzurePlaywrightConfig, ServiceOS } from '@azure/playwright';
import { DefaultAzureCredential } from '@azure/identity';

/* Azure Playwright Configuration - Chromium Only (Fast CI/CD) */
const chromiumConfig = defineConfig({
  testDir: './e2e',
  fullyParallel: true,
  retries: process.env.CI ? 2 : 2,
  workers: process.env.CI 
    ? parseInt(process.env.PLAYWRIGHT_WORKERS || '10') || 10 // Mais workers para Azure
    : 4,
  reporter: process.env.CI ? [['html'], ['github']] : 'list',
  timeout: 60000, // Timeout maior para Azure
  use: {
    baseURL: 'http://localhost:5146',
    headless: true, // Sempre headless no Azure
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    // Configurações otimizadas para Azure
    actionTimeout: 20000,
    navigationTimeout: 45000,
    // Configurações de performance para Azure
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
      ],
    },
  },

  projects: [
    {
      name: 'chromium',
      use: { 
        ...devices['Desktop Chrome'],
        // Configurações específicas para Azure Chromium
        viewport: { width: 1280, height: 720 },
        ignoreHTTPSErrors: true,
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
  chromiumConfig,
  createAzurePlaywrightConfig(chromiumConfig, {
    exposeNetwork: '<loopback>',
    connectTimeout: 5 * 60 * 1000, // 5 minutes for Azure
    os: ServiceOS.LINUX,
    credential: new DefaultAzureCredential(),
  })
);