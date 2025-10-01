import { defineConfig, devices } from '@playwright/test';
import dotenv from 'dotenv';

dotenv.config();

// Detectar se deve usar Azure Workspaces
const serviceUrl = process.env.PLAYWRIGHT_SERVICE_URL;
const isCI = process.env.CI === 'true';
const useAzureWorkspaces = !!(serviceUrl && serviceUrl.trim() && isCI);

// Log da configuração
console.log('🎭 Playwright Configuration:');
console.log(`   Azure Workspaces: ${useAzureWorkspaces ? '✅ Enabled (cloud mode)' : '❌ Disabled (local mode)'}`);
if (useAzureWorkspaces) {
  console.log(`   Service URL: ${serviceUrl}`);
  console.log(`   Workers: ${process.env.PLAYWRIGHT_WORKERS || '20'}`);
  console.log(`   Repository: ${process.env.GITHUB_REPOSITORY || 'unknown'}`);
} else {
  console.log(`   Reason: ${!isCI ? 'Not in CI environment' : 'Service URL not configured'}`);
  console.log(`   Workers: 4 (local)`);
}
if (useAzureWorkspaces) {
  console.log(`   Service URL: ${process.env.PLAYWRIGHT_SERVICE_URL}`);
}

export default defineConfig({
  // Azure-specific worker configuration
  workers: useAzureWorkspaces ? parseInt(process.env.PLAYWRIGHT_WORKERS || '20') : 4,
  
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
  
  // Base URL and conditional Azure connection
  use: {
    baseURL: 'http://localhost:5146',
    actionTimeout: 25000,
    navigationTimeout: 60000,
    screenshot: 'only-on-failure' as const,
    video: 'retain-on-failure' as const,
    trace: 'retain-on-failure' as const,
    
    // Azure Playwright service connection - apenas se configurado corretamente
    ...(useAzureWorkspaces ? {
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
  
  // Projects configuration - Azure Workspaces ou local baseado na disponibilidade
  projects: useAzureWorkspaces ? [
    // Azure Playwright Workspaces: 5 browsers including mobile
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
  ] : [
    // Local mode: apenas browsers desktop
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
      },
    },
    {
      name: 'webkit',
      use: { 
        ...devices['Desktop Safari'],
        viewport: { width: 1280, height: 720 },
      },
    },
  ],
  
  // Metadata with service detection
  metadata: {
    'test-type': useAzureWorkspaces ? 'Azure Playwright Workspaces' : 'Local Playwright',
    'repo': process.env.GITHUB_REPOSITORY || 'local',
    'run-id': process.env.GITHUB_RUN_ID || 'local',
    'service-url': process.env.PLAYWRIGHT_SERVICE_URL || 'not-configured',
    'workers': useAzureWorkspaces ? parseInt(process.env.PLAYWRIGHT_WORKERS || '20') : 4,
    'mode': useAzureWorkspaces ? 'azure-workspaces' : 'local-browsers',
  },
});