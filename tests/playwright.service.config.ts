import { defineConfig, devices } from '@playwright/test';
import dotenv from 'dotenv';

dotenv.config();

export default defineConfig({
  // Maximum 20 workers when running on Azure Playwright Workspaces
  workers: parseInt(process.env.PLAYWRIGHT_WORKERS || '20'),
  
  // Timeout configurations optimized for cloud execution
  timeout: 60000,
  expect: {
    timeout: 30000,
  },
  
  // Test directory
  testDir: '.',
  
  // Base URL para os testes
  use: {
    // Base URL for all tests
    baseURL: 'http://localhost:5146',
    
    // Azure Playwright service automatically configures browsers
    // Only connect to service if URL is provided and we're in CI or service mode
    ...(process.env.PLAYWRIGHT_SERVICE_URL && (process.env.CI || process.env.PLAYWRIGHT_SERVICE_MODE) ? {
      connectOptions: {
        // The service endpoint URL from Azure portal
        wsEndpoint: process.env.PLAYWRIGHT_SERVICE_URL as string,
      },
    } : {}),
  },
  
  // WebServer configuration - starts the .NET app automatically
  webServer: {
    command: 'dotnet run --project ../TodoListApp.csproj',
    url: 'http://localhost:5146',
    reuseExistingServer: !process.env.CI,
    timeout: 120 * 1000, // 2 minutos para iniciar
    env: {
      ASPNETCORE_ENVIRONMENT: 'Development',
      ASPNETCORE_URLS: 'http://localhost:5146',
    },
  },
  
  // Report configuration
  reporter: [
    ['html', { open: 'never' }],
    ['list'],
    ['junit', { outputFile: 'test-results/junit-results.xml' }],
  ],
  
  // Output directory for artifacts
  outputDir: 'test-results',
  
  // Retry failed tests
  retries: process.env.CI ? 2 : 0,
  
  // Global test timeout
  globalTimeout: 60 * 60 * 1000, // 1 hour
  
  // Projects configuration for Azure Playwright Workspaces
  projects: [
    {
      name: 'chromium',
      use: { 
        ...devices['Desktop Chrome'],
      },
    },
    {
      name: 'firefox',
      use: { 
        ...devices['Desktop Firefox'],
      },
    },
    {
      name: 'webkit',
      use: { 
        ...devices['Desktop Safari'],
      },
    },
    // Mobile browsers
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
  
  // Azure Playwright Workspaces specific configurations
  metadata: {
    'test-type': 'Azure Playwright Workspaces',
    'repo': process.env.GITHUB_REPOSITORY || 'local',
    'run-id': process.env.GITHUB_RUN_ID || 'local',
  },
});