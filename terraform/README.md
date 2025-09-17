# Todo List Application - Terraform Infrastructure

Este diretório contém a configuração Terraform para deploy da aplicação Todo List no Microsoft Azure.

## 🚀 Quick Start

### 1. Validar Configuração
```bash
./scripts/validate.sh
```

### 2. Deploy
```bash
# Produção
./scripts/deploy.sh production

# Desenvolvimento  
./scripts/deploy.sh dev

# Homologação
./scripts/deploy.sh staging
```

### 3. Destruir (Cuidado!)
```bash
./scripts/destroy.sh dev
```

## 📁 Estrutura

- `main.tf` - Configuração principal
- `variables.tf` - Definição de variáveis
- `external-vars.tfvars` - Variáveis de produção
- `environments/` - Configurações por ambiente
- `modules/` - Módulos reutilizáveis
- `scripts/` - Scripts de automação

## 📖 Documentação Completa

Veja [README.iac.md](../README.iac.md) para documentação completa com diagramas de arquitetura.

## ✅ Requisitos Atendidos

- ✅ Variáveis externas em `external-vars.tfvars`
- ✅ Recursos de rede privada com private endpoints
- ✅ Módulos organizados em pastas separadas
- ✅ Diagrama Mermaid no README.iac.md

## 🔧 Comandos Úteis

```bash
# Inicializar
terraform init

# Validar
terraform validate  

# Planejar
terraform plan -var-file="external-vars.tfvars"

# Aplicar
terraform apply -var-file="external-vars.tfvars"

# Formatar código
terraform fmt -recursive
```