import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  fullyParallel: true,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 4 : 4,
  reporter: 'html',
  timeout: 45000,
  use: {
    baseURL: 'http://localhost:5146',
    headless: true,
    screenshot: 'only-on-failure',
    video: 'off',
    actionTimeout: 15000,
    navigationTimeout: 30000,
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
