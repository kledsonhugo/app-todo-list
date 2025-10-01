import { defineConfig, devices } from '@playwright/test';
import dotenv from 'dotenv';

dotenv.config();

export default defineConfig({
  // Azure-specific worker configuration
  workers: parseInt(process.env.PLAYWRIGHT_WORKERS || '20'),
  
  // Timeout configurations
  timeout: 90000,
  expect: {
    timeout: 15000,
  },
  
  // Test directory - same as local config
  testDir: './e2e',
  
  // Enable full parallelization
  fullyParallel: true,
  
  // Retry configuration
  retries: 2,
  
  // Base URL and Azure connection
  use: {
    baseURL: 'http://localhost:5146',
    actionTimeout: 25000,
    navigationTimeout: 60000,
    screenshot: 'only-on-failure' as const,
    video: 'retain-on-failure' as const,
    trace: 'retain-on-failure' as const,
    
    // Azure Playwright service connection
    ...(process.env.PLAYWRIGHT_SERVICE_URL && (process.env.CI || process.env.PLAYWRIGHT_SERVICE_MODE) ? {
      connectOptions: {
        wsEndpoint: process.env.PLAYWRIGHT_SERVICE_URL as string,
      },
    } : {}),
  },
  
  // WebServer configuration (reuse existing server)
  webServer: {
    command: 'echo "App should already be running"',
    url: 'http://localhost:5146',
    reuseExistingServer: true,
  },
  
  // Reporter configuration
  reporter: [
    ['html', { open: 'never' }],
    ['list'],
    ['junit', { outputFile: 'test-results/junit-results.xml' }],
  ],
  
  // Output directory
  outputDir: 'test-results',
  
  // Global timeout
  globalTimeout: 60 * 60 * 1000, // 1 hour
  
  // Azure Playwright Workspaces projects (all browsers including mobile)
  projects: [
    {
      name: 'chromium',
      use: { 
        ...devices['Desktop Chrome'],
        viewport: { width: 1280, height: 720 },
      },
    },
    {
      name: 'firefox',
      use: { 
        ...devices['Desktop Firefox'],
        viewport: { width: 1280, height: 720 },
        actionTimeout: 25000,
      },
    },
    {
      name: 'webkit',
      use: { 
        ...devices['Desktop Safari'],
        viewport: { width: 1280, height: 720 },
        actionTimeout: 30000,
      },
    },
    {
      name: 'Mobile Chrome',
      use: { 
        ...devices['Pixel 5'],
      },
    },
    {
      name: 'Mobile Safari',
      use: { 
        ...devices['iPhone 12'],
      },
    },
  ],
  
  // Azure-specific metadata
  metadata: {
    'test-type': 'Azure Playwright Workspaces',
    'repo': process.env.GITHUB_REPOSITORY || 'local',
    'run-id': process.env.GITHUB_RUN_ID || 'local',
    'service-url': process.env.PLAYWRIGHT_SERVICE_URL || 'not-configured',
    'workers': parseInt(process.env.PLAYWRIGHT_WORKERS || '20'),
  },
});