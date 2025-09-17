#!/bin/bash

# Destruction script for Todo List Application Infrastructure
# Usage: ./destroy.sh <environment>
# Example: ./destroy.sh development

set -e

ENVIRONMENT=${1:-production}
TERRAFORM_DIR="$(dirname "$0")/.."
VARS_FILE=""

echo "🧨 Starting Terraform destruction for environment: $ENVIRONMENT"
echo "⚠️  WARNING: This will destroy all resources in the $ENVIRONMENT environment!"

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
        echo "⚠️  CRITICAL WARNING: You are about to destroy PRODUCTION infrastructure!"
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

# Show current state
echo "📋 Current resources:"
terraform state list 2>/dev/null || echo "No state found or Terraform not initialized"

# Double confirmation for destruction
echo ""
echo "💀 Are you absolutely sure you want to destroy all resources in $ENVIRONMENT? (yes/no)"
read -r first_confirmation

if [ "$first_confirmation" != "yes" ]; then
    echo "❌ Destruction cancelled."
    exit 0
fi

echo "🔄 Type the environment name '$ENVIRONMENT' to confirm:"
read -r env_confirmation

if [ "$env_confirmation" != "$ENVIRONMENT" ]; then
    echo "❌ Environment name mismatch. Destruction cancelled."
    exit 0
fi

# Initialize Terraform
echo "🔧 Initializing Terraform..."
terraform init

# Plan the destruction
echo "📋 Planning destruction..."
terraform plan -destroy -var-file="$VARS_FILE" -out="tfplan-destroy-$ENVIRONMENT"

# Final confirmation before destroying
echo ""
echo "🚨 FINAL WARNING: This will permanently delete all infrastructure!"
echo "🤔 Proceed with destruction? (yes/no)"
read -r final_confirmation

if [ "$final_confirmation" = "yes" ] || [ "$final_confirmation" = "y" ]; then
    echo "💥 Destroying infrastructure..."
    terraform apply "tfplan-destroy-$ENVIRONMENT"
    
    echo ""
    echo "✅ Destruction completed!"
else
    echo "❌ Destruction cancelled."
    rm -f "tfplan-destroy-$ENVIRONMENT"
    exit 0
fi

# Clean up plan file
rm -f "tfplan-destroy-$ENVIRONMENT"

echo ""
echo "🏁 Infrastructure destruction for $ENVIRONMENT completed!"