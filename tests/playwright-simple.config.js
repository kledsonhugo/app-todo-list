import { defineConfig } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  fullyParallel: false, // Executar sequencialmente para evitar conflitos
  retries: 1,
  workers: 1, // Usar apenas 1 worker
  reporter: 'list',
  timeout: 60000, // Aumentar timeout
  use: {
    baseURL: 'http://localhost:5146',
    headless: process.env.CI ? true : false, // Headless em CI, headed localmente
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
  },
  projects: [
    {
      name: 'chromium',
      use: { 
        channel: 'chromium',
        viewport: { width: 1280, height: 720 }
      },
    },
    {
      name: 'firefox',
      use: { 
        channel: 'firefox',
        viewport: { width: 1280, height: 720 }
      },
    },
    {
      name: 'webkit',
      use: { 
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