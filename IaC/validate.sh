#!/bin/bash

# Bicep Template Validation Script
# This script validates all Bicep templates without deploying them

set -e

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

print_status() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_info() {
    echo -e "${CYAN}📋 $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_header() {
    echo -e "${GREEN}🔍 $1${NC}"
}

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    print_error "Azure CLI is not installed. Please install it first."
    exit 1
fi

print_header "Validating Bicep Templates"

# Create a temporary resource group for validation
TEMP_RG="temp-validation-rg-$(date +%s)"
LOCATION="East US"
SUBSCRIPTION_ID=$(az account show --query 'id' -o tsv)

print_info "Using subscription: $(az account show --query 'name' -o tsv)"
print_info "Creating temporary resource group: $TEMP_RG"

# Create temporary resource group
az group create --name "$TEMP_RG" --location "$LOCATION" --output none

# Test parameters
TEST_PARAMS="location='$LOCATION' environmentName='dev' sqlAdminLogin='testadmin' sqlAdminPassword='TestPassword123!'"

# Validate main monolithic template
print_info "Validating main.bicep..."
if az deployment group validate \
    --resource-group "$TEMP_RG" \
    --template-file "main.bicep" \
    --parameters $TEST_PARAMS \
    --output none 2>/dev/null; then
    print_status "main.bicep validation passed"
else
    print_error "main.bicep validation failed"
    VALIDATION_FAILED=1
fi

# Validate modular template
print_info "Validating main-modular.bicep..."
if az deployment group validate \
    --resource-group "$TEMP_RG" \
    --template-file "main-modular.bicep" \
    --parameters $TEST_PARAMS \
    --output none 2>/dev/null; then
    print_status "main-modular.bicep validation passed"
else
    print_error "main-modular.bicep validation failed"
    VALIDATION_FAILED=1
fi

# Validate individual modules
print_info "Validating individual modules..."

# Note: Individual modules need their dependencies, so we'll do a basic syntax check
modules=("modules/appService.bicep" "modules/sqlDatabase.bicep" "modules/monitoring.bicep" "modules/identity.bicep" "modules/roleAssignments.bicep")

for module in "${modules[@]}"; do
    if [[ -f "$module" ]]; then
        print_info "Checking syntax for $module..."
        if az bicep build --file "$module" --stdout > /dev/null 2>&1; then
            print_status "$module syntax is valid"
        else
            print_error "$module has syntax errors"
            VALIDATION_FAILED=1
        fi
    else
        print_error "$module not found"
        VALIDATION_FAILED=1
    fi
done

# Validate parameter files
print_info "Validating parameter files..."
param_files=("parameters/dev.parameters.json" "parameters/prod.parameters.json")

for param_file in "${param_files[@]}"; do
    if [[ -f "$param_file" ]]; then
        print_info "Validating $param_file..."
        if jq empty "$param_file" 2>/dev/null; then
            print_status "$param_file is valid JSON"
        else
            print_error "$param_file has invalid JSON syntax"
            VALIDATION_FAILED=1
        fi
    else
        print_error "$param_file not found"
        VALIDATION_FAILED=1
    fi
done

# Clean up temporary resource group
print_info "Cleaning up temporary resource group..."
az group delete --name "$TEMP_RG" --yes --no-wait

# Final result
if [[ "$VALIDATION_FAILED" == "1" ]]; then
    print_error "Some validations failed. Please check the errors above."
    exit 1
else
    print_status "All Bicep templates and parameter files are valid!"
    print_info "Templates are ready for deployment."
fi