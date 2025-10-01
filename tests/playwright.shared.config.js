// Base configuration shared between all Playwright configs
// This ensures consistency across local, Azure, and multi-browser pipelines

import { devices } from '@playwright/test';

export const sharedConfig = {
  // Timeouts padronizados
  timeout: 90000,
  expect: {
    timeout: 15000,
  },
  
  // Directory configuration
  testDir: './e2e',
  outputDir: 'test-results',
  
  // Paralelização 
  fullyParallel: true,
  retries: 2,
  
  // Base configurations for all environments
  use: {
    baseURL: 'http://localhost:5146',
    actionTimeout: 25000,
    navigationTimeout: 60000,
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    trace: 'retain-on-failure',
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
  
  // Global timeout
  globalTimeout: 60 * 60 * 1000, // 1 hour
  
  // Metadata base
  metadata: {
    'test-type': 'Todo List E2E Tests',
    'repo': process.env.GITHUB_REPOSITORY || 'local',
    'run-id': process.env.GITHUB_RUN_ID || 'local',
  },
};

// Browser projects configurations
export const browserProjects = {
  chromium: {
    name: 'chromium',
    use: { 
      ...devices['Desktop Chrome'],
      viewport: { width: 1280, height: 720 },
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
  },
  
  firefox: {
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
  
  webkit: {
    name: 'webkit',
    use: { 
      ...devices['Desktop Safari'],
      viewport: { width: 1280, height: 720 },
      actionTimeout: 30000,
    },
  },
  
  // Mobile browsers for Azure Playwright Workspaces
  'Mobile Chrome': {
    name: 'Mobile Chrome',
    use: { 
      ...devices['Pixel 5'],
    },
  },
  
  'Mobile Safari': {
    name: 'Mobile Safari',
    use: { 
      ...devices['iPhone 12'],
    },
  },
};

// Worker configurations for different environments
export const workerConfigs = {
  local: '25%',
  ci: 4,
  azure: 20,
};

// Get workers based on environment
export function getWorkers() {
  if (process.env.PLAYWRIGHT_SERVICE_URL) {
    return parseInt(process.env.PLAYWRIGHT_WORKERS || workerConfigs.azure.toString());
  }
  return process.env.CI 
    ? parseInt(process.env.PLAYWRIGHT_WORKERS || workerConfigs.ci.toString())
    : workerConfigs.local;
}

// Azure Playwright Workspaces connection options
export function getAzureConnectionOptions() {
  if (process.env.PLAYWRIGHT_SERVICE_URL && (process.env.CI || process.env.PLAYWRIGHT_SERVICE_MODE)) {
    return {
      connectOptions: {
        wsEndpoint: process.env.PLAYWRIGHT_SERVICE_URL,
      },
    };
  }
  return {};
}