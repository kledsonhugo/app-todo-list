#!/bin/bash

# Todo List Application Infrastructure Deployment Script
# This script deploys the Todo List application infrastructure to Azure using Bicep templates

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_info() {
    echo -e "${CYAN}📋 $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️ $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_header() {
    echo -e "${GREEN}🚀 $1${NC}"
}

# Default values
LOCATION="East US"
ENVIRONMENT="dev"
TEMPLATE_FILE="main.bicep"

# Function to show usage
show_usage() {
    echo "Usage: $0 -s SUBSCRIPTION_ID -g RESOURCE_GROUP_NAME [OPTIONS]"
    echo ""
    echo "Required parameters:"
    echo "  -s, --subscription-id    Azure subscription ID"
    echo "  -g, --resource-group     Resource group name"
    echo ""
    echo "Optional parameters:"
    echo "  -l, --location          Azure region (default: East US)"
    echo "  -e, --environment       Environment name: dev, test, prod (default: dev)"
    echo "  -m, --modular          Use modular template instead of monolithic"
    echo "  -p, --password         SQL admin password (will prompt if not provided)"
    echo "  -h, --help             Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0 -s 12345678-1234-1234-1234-123456789012 -g TDC-dev-rg"
    echo "  $0 -s 12345678-1234-1234-1234-123456789012 -g TDC-prod-rg -e prod -l \"West US 2\" -m"
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -s|--subscription-id)
            SUBSCRIPTION_ID="$2"
            shift 2
            ;;
        -g|--resource-group)
            RESOURCE_GROUP_NAME="$2"
            shift 2
            ;;
        -l|--location)
            LOCATION="$2"
            shift 2
            ;;
        -e|--environment)
            ENVIRONMENT="$2"
            shift 2
            ;;
        -m|--modular)
            TEMPLATE_FILE="main-modular.bicep"
            shift
            ;;
        -p|--password)
            SQL_ADMIN_PASSWORD="$2"
            shift 2
            ;;
        -h|--help)
            show_usage
            exit 0
            ;;
        *)
            print_error "Unknown parameter: $1"
            show_usage
            exit 1
            ;;
    esac
done

# Validate required parameters
if [[ -z "$SUBSCRIPTION_ID" ]]; then
    print_error "Subscription ID is required"
    show_usage
    exit 1
fi

if [[ -z "$RESOURCE_GROUP_NAME" ]]; then
    print_error "Resource group name is required"
    show_usage
    exit 1
fi

# Validate environment
if [[ "$ENVIRONMENT" != "dev" && "$ENVIRONMENT" != "test" && "$ENVIRONMENT" != "prod" ]]; then
    print_error "Environment must be one of: dev, test, prod"
    exit 1
fi

print_header "Starting Todo List Application Infrastructure Deployment"
print_info "Environment: $ENVIRONMENT"
print_info "Location: $LOCATION"
print_info "Resource Group: $RESOURCE_GROUP_NAME"
print_info "Template: $TEMPLATE_FILE"

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    print_error "Azure CLI is not installed. Please install it first."
    exit 1
fi

AZ_VERSION=$(az version --query '"azure-cli"' -o tsv)
print_status "Azure CLI version: $AZ_VERSION"

# Check if logged in to Azure
print_info "Checking Azure authentication..."
if ! az account show &> /dev/null; then
    print_warning "Not logged in to Azure. Please login..."
    az login
fi

# Set subscription
print_info "Setting Azure subscription..."
az account set --subscription "$SUBSCRIPTION_ID"

# Verify subscription
CURRENT_SUBSCRIPTION=$(az account show --query 'name' -o tsv)
CURRENT_SUBSCRIPTION_ID=$(az account show --query 'id' -o tsv)
print_status "Using subscription: $CURRENT_SUBSCRIPTION ($CURRENT_SUBSCRIPTION_ID)"

# Create resource group if it doesn't exist
print_info "Creating resource group if it doesn't exist..."
az group create --name "$RESOURCE_GROUP_NAME" --location "$LOCATION" --output none
print_status "Resource group '$RESOURCE_GROUP_NAME' is ready"

# Get SQL Admin password if not provided
if [[ -z "$SQL_ADMIN_PASSWORD" ]]; then
    echo -n "Enter SQL Server administrator password: "
    read -s SQL_ADMIN_PASSWORD
    echo
fi

# Validate password strength
if [[ ${#SQL_ADMIN_PASSWORD} -lt 8 ]]; then
    print_error "Password must be at least 8 characters long"
    exit 1
fi

# Validate Bicep template
print_info "Validating Bicep template..."
if az deployment group validate \
    --resource-group "$RESOURCE_GROUP_NAME" \
    --template-file "$TEMPLATE_FILE" \
    --parameters location="$LOCATION" environmentName="$ENVIRONMENT" sqlAdminLogin="tdcadmin" sqlAdminPassword="$SQL_ADMIN_PASSWORD" \
    --output none; then
    print_status "Bicep template validation passed"
else
    print_error "Bicep template validation failed"
    exit 1
fi

# Deploy infrastructure
print_info "Deploying infrastructure..."
DEPLOYMENT_NAME="TDC-$ENVIRONMENT-$(date '+%Y%m%d-%H%M%S')"

if DEPLOYMENT_OUTPUT=$(az deployment group create \
    --resource-group "$RESOURCE_GROUP_NAME" \
    --name "$DEPLOYMENT_NAME" \
    --template-file "$TEMPLATE_FILE" \
    --parameters location="$LOCATION" environmentName="$ENVIRONMENT" sqlAdminLogin="tdcadmin" sqlAdminPassword="$SQL_ADMIN_PASSWORD" \
    --output json); then
    
    print_status "Infrastructure deployment completed successfully!"
    
    # Extract and display outputs
    APP_SERVICE_NAME=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.properties.outputs.appServiceName.value')
    APP_SERVICE_URL=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.properties.outputs.appServiceUrl.value')
    SQL_SERVER_NAME=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.properties.outputs.sqlServerName.value')
    SQL_DATABASE_NAME=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.properties.outputs.sqlDatabaseName.value')
    APPLICATION_INSIGHTS_NAME=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.properties.outputs.applicationInsightsName.value')
    MANAGED_IDENTITY_NAME=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.properties.outputs.managedIdentityName.value')
    
    echo
    print_info "Deployment Results:"
    echo "Resource Group: $RESOURCE_GROUP_NAME"
    echo "App Service Name: $APP_SERVICE_NAME"
    echo "App Service URL: $APP_SERVICE_URL"
    echo "SQL Server: $SQL_SERVER_NAME"
    echo "SQL Database: $SQL_DATABASE_NAME"
    echo "Application Insights: $APPLICATION_INSIGHTS_NAME"
    echo "Managed Identity: $MANAGED_IDENTITY_NAME"
    
    echo
    print_status "Deployment completed successfully!"
    print_warning "Next steps:"
    echo "1. Deploy your application code to: $APP_SERVICE_URL"
    echo "2. Configure your application settings in the Azure portal"
    echo "3. Set up continuous deployment from your source repository"
    
else
    print_error "Infrastructure deployment failed"
    exit 1
fi

# Clear password from memory
unset SQL_ADMIN_PASSWORD