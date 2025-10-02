#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Deploy Todo List Application Infrastructure using Bicep

.DESCRIPTION
    This script deploys the Todo List application infrastructure to Azure using Bicep templates.
    It creates all necessary resources including App Service, SQL Database, Application Insights,
    and Managed Identity with proper security configurations.

.PARAMETER SubscriptionId
    The Azure subscription ID where resources will be deployed

.PARAMETER ResourceGroupName  
    The name of the resource group (will be created if it doesn't exist)

.PARAMETER Location
    The Azure region where resources will be deployed (default: East US)

.PARAMETER Environment
    The environment name (dev, test, prod) - default: dev

.PARAMETER SqlAdminPassword
    The SQL Server administrator password (will prompt securely if not provided)

.EXAMPLE
    .\deploy.ps1 -SubscriptionId "12345678-1234-1234-1234-123456789012" -ResourceGroupName "TDC-dev-rg"

.EXAMPLE
    .\deploy.ps1 -SubscriptionId "12345678-1234-1234-1234-123456789012" -ResourceGroupName "TDC-prod-rg" -Environment "prod" -Location "West US 2"
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$SubscriptionId,
    
    [Parameter(Mandatory = $true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory = $false)]
    [string]$Location = "East US",
    
    [Parameter(Mandatory = $false)]
    [ValidateSet("dev", "test", "prod")]
    [string]$Environment = "dev",
    
    [Parameter(Mandatory = $false)]
    [SecureString]$SqlAdminPassword
)

# Set error action preference
$ErrorActionPreference = "Stop"

Write-Host "🚀 Starting Todo List Application Infrastructure Deployment" -ForegroundColor Green
Write-Host "Environment: $Environment" -ForegroundColor Cyan
Write-Host "Location: $Location" -ForegroundColor Cyan
Write-Host "Resource Group: $ResourceGroupName" -ForegroundColor Cyan

# Check if Azure CLI is installed
try {
    $azVersion = az version --output json | ConvertFrom-Json
    Write-Host "✅ Azure CLI version: $($azVersion.'azure-cli')" -ForegroundColor Green
}
catch {
    Write-Error "❌ Azure CLI is not installed or not found in PATH. Please install Azure CLI first."
    exit 1
}

# Login to Azure (if not already logged in)
Write-Host "🔐 Checking Azure authentication..." -ForegroundColor Yellow
$accountInfo = az account show --output json 2>$null | ConvertFrom-Json
if (-not $accountInfo) {
    Write-Host "Please login to Azure..." -ForegroundColor Yellow
    az login
}

# Set subscription
Write-Host "📋 Setting Azure subscription..." -ForegroundColor Yellow
az account set --subscription $SubscriptionId

# Verify subscription
$currentSubscription = az account show --output json | ConvertFrom-Json
Write-Host "✅ Using subscription: $($currentSubscription.name) ($($currentSubscription.id))" -ForegroundColor Green

# Create resource group if it doesn't exist
Write-Host "📁 Creating resource group if it doesn't exist..." -ForegroundColor Yellow
az group create --name $ResourceGroupName --location $Location --output none
Write-Host "✅ Resource group '$ResourceGroupName' is ready" -ForegroundColor Green

# Get SQL Admin password if not provided
if (-not $SqlAdminPassword) {
    $SqlAdminPassword = Read-Host "Enter SQL Server administrator password" -AsSecureString
}

# Convert SecureString to plain text for Azure CLI (this is necessary for the deployment)
$SqlAdminPasswordPlainText = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($SqlAdminPassword))

# Validate Bicep templates
Write-Host "🔍 Validating Bicep templates..." -ForegroundColor Yellow

# Check if we should use modular or monolithic template
$templateFile = "main.bicep"
if (Test-Path "main-modular.bicep") {
    $choice = Read-Host "Use modular template? (y/N)"
    if ($choice -eq 'y' -or $choice -eq 'Y') {
        $templateFile = "main-modular.bicep"
    }
}

try {
    az deployment group validate `
        --resource-group $ResourceGroupName `
        --template-file $templateFile `
        --parameters location="$Location" environmentName="$Environment" sqlAdminLogin="tdcadmin" sqlAdminPassword="$SqlAdminPasswordPlainText" `
        --output none
    Write-Host "✅ Bicep template validation passed" -ForegroundColor Green
}
catch {
    Write-Error "❌ Bicep template validation failed: $_"
    exit 1
}

# Deploy infrastructure
Write-Host "🏗️ Deploying infrastructure..." -ForegroundColor Yellow
$deploymentName = "TDC-$Environment-$(Get-Date -Format 'yyyyMMdd-HHmmss')"

try {
    $deploymentOutput = az deployment group create `
        --resource-group $ResourceGroupName `
        --name $deploymentName `
        --template-file $templateFile `
        --parameters location="$Location" environmentName="$Environment" sqlAdminLogin="tdcadmin" sqlAdminPassword="$SqlAdminPasswordPlainText" `
        --output json | ConvertFrom-Json
    
    Write-Host "✅ Infrastructure deployment completed successfully!" -ForegroundColor Green
    
    # Display deployment outputs
    Write-Host "`n📋 Deployment Results:" -ForegroundColor Cyan
    Write-Host "Resource Group: $($deploymentOutput.properties.outputs.resourceGroupName.value)" -ForegroundColor White
    Write-Host "App Service Name: $($deploymentOutput.properties.outputs.appServiceName.value)" -ForegroundColor White  
    Write-Host "App Service URL: $($deploymentOutput.properties.outputs.appServiceUrl.value)" -ForegroundColor White
    Write-Host "SQL Server: $($deploymentOutput.properties.outputs.sqlServerName.value)" -ForegroundColor White
    Write-Host "SQL Database: $($deploymentOutput.properties.outputs.sqlDatabaseName.value)" -ForegroundColor White
    Write-Host "Application Insights: $($deploymentOutput.properties.outputs.applicationInsightsName.value)" -ForegroundColor White
    Write-Host "Managed Identity: $($deploymentOutput.properties.outputs.managedIdentityName.value)" -ForegroundColor White
    
    # Clear password from memory
    $SqlAdminPasswordPlainText = $null
    
    Write-Host "`n🎉 Deployment completed successfully!" -ForegroundColor Green
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "1. Deploy your application code to: $($deploymentOutput.properties.outputs.appServiceUrl.value)" -ForegroundColor White
    Write-Host "2. Configure your application settings in the Azure portal" -ForegroundColor White
    Write-Host "3. Set up continuous deployment from your source repository" -ForegroundColor White
}
catch {
    Write-Error "❌ Infrastructure deployment failed: $_"
    
    # Clear password from memory
    $SqlAdminPasswordPlainText = $null
    exit 1
}