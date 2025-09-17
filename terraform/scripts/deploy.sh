#!/bin/bash

# Deployment script for Todo List Application Infrastructure
# Usage: ./deploy.sh <environment>
# Example: ./deploy.sh production

set -e

ENVIRONMENT=${1:-production}
TERRAFORM_DIR="$(dirname "$0")/.."
VARS_FILE=""

echo "🚀 Starting Terraform deployment for environment: $ENVIRONMENT"

# Determine which variables file to use
case $ENVIRONMENT in
    "dev"|"development")
        VARS_FILE="environments/dev.tfvars"
        ;;
    "staging")
        VARS_FILE="environments/staging.tfvars"
        ;;
    "prod"|"production")
        VARS_FILE="external-vars.tfvars"
        ;;
    *)
        echo "❌ Unknown environment: $ENVIRONMENT"
        echo "Valid environments: dev, staging, production"
        exit 1
        ;;
esac

# Change to terraform directory
cd "$TERRAFORM_DIR"

echo "📁 Working directory: $(pwd)"
echo "📋 Using variables file: $VARS_FILE"

# Check if variables file exists
if [ ! -f "$VARS_FILE" ]; then
    echo "❌ Variables file not found: $VARS_FILE"
    exit 1
fi

# Initialize Terraform
echo "🔧 Initializing Terraform..."
terraform init

# Validate Terraform configuration
echo "✅ Validating Terraform configuration..."
terraform validate

# Plan the deployment
echo "📋 Planning deployment..."
terraform plan -var-file="$VARS_FILE" -out="tfplan-$ENVIRONMENT"

# Ask for confirmation before applying
echo ""
echo "🤔 Do you want to apply these changes? (yes/no)"
read -r confirmation

if [ "$confirmation" = "yes" ] || [ "$confirmation" = "y" ]; then
    echo "🚀 Applying changes..."
    terraform apply "tfplan-$ENVIRONMENT"
    
    echo ""
    echo "✅ Deployment completed successfully!"
    echo "📊 Getting outputs..."
    terraform output
else
    echo "❌ Deployment cancelled."
    rm -f "tfplan-$ENVIRONMENT"
    exit 0
fi

# Clean up plan file
rm -f "tfplan-$ENVIRONMENT"

echo ""
echo "🎉 Infrastructure deployment for $ENVIRONMENT completed!"
echo "🌐 Check the outputs above for application URLs and next steps."