# Todo List Application - Azure Architecture Diagram

## Infrastructure Overview

This diagram represents the Azure infrastructure deployed using the Bicep templates in this repository.

```mermaid
graph TB
    %% External User
    User[👤 User]
    
    %% Azure Services
    subgraph "Azure Cloud"
        subgraph "TDC Resource Group"
            
            %% App Service with Easy Auth
            subgraph "TDC-{env}-app"
                AppService[🌐 App Service<br/>Todo List API<br/>.NET 8.0]
                EasyAuth[🔐 Easy Auth<br/>Built-in Authentication]
            end
            
            %% Managed Identity
            ManagedIdentity[🆔 TDC-{env}-mi<br/>Managed Identity]
            
            %% SQL Database
            subgraph "TDC-{env}-sqlserver"
                SqlServer[🗃️ SQL Server]
                SqlDatabase[💾 TDC-{env}-sqldb<br/>SQL Database]
            end
            
            %% Monitoring
            subgraph "Monitoring"
                AppInsights[📊 TDC-{env}-ai<br/>Application Insights]
                LogAnalytics[📈 TDC-{env}-law<br/>Log Analytics<br/>Workspace]
            end
            
        end
        
        %% Microsoft Entra ID (External to RG)
        EntraID[🔑 Microsoft Entra ID<br/>Identity Provider]
    end
    
    %% Connections
    User -->|HTTPS<br/>domainname.azurewebsites.net| AppService
    AppService -->|Uses| EasyAuth
    EasyAuth -->|Authenticates with| EntraID
    AppService -->|Assigned| ManagedIdentity
    ManagedIdentity -->|Access| SqlDatabase
    SqlServer --> SqlDatabase
    AppService -->|Sends Telemetry| AppInsights
    AppInsights -->|Stores Data| LogAnalytics
    
    %% Styling
    classDef azure fill:#0078d4,stroke:#005a9e,stroke-width:2px,color:#fff
    classDef compute fill:#00bcf2,stroke:#0078d4,stroke-width:2px,color:#fff
    classDef data fill:#ff6b35,stroke:#e55100,stroke-width:2px,color:#fff
    classDef identity fill:#7b68ee,stroke:#5a4fcf,stroke-width:2px,color:#fff
    classDef monitor fill:#32cd32,stroke:#228b22,stroke-width:2px,color:#fff
    classDef user fill:#ffa500,stroke:#ff8c00,stroke-width:2px,color:#fff
    
    class AppService,EasyAuth compute
    class SqlServer,SqlDatabase data
    class ManagedIdentity,EntraID identity
    class AppInsights,LogAnalytics monitor
    class User user
```

## Architecture Components

### 🌐 Application Layer
- **TDC-{env}-app**: Azure App Service hosting the .NET 8.0 Todo List API
- **Easy Auth**: Built-in authentication service integrated with App Service
- **Custom Domain**: `https://domainname.azurewebsites.net`

### 🔐 Identity & Security
- **TDC-{env}-mi**: User Assigned Managed Identity for secure service-to-service authentication
- **Microsoft Entra ID**: Identity provider for user authentication
- **RBAC**: Role-based access control for SQL Database access

### 💾 Data Layer
- **TDC-{env}-sqlserver**: Azure SQL Server with managed identity integration
- **TDC-{env}-sqldb**: SQL Database for storing todo list data
- **Connection**: Secured using Managed Identity authentication (no connection strings)

### 📊 Monitoring & Observability
- **TDC-{env}-ai**: Application Insights for application performance monitoring
- **TDC-{env}-law**: Log Analytics Workspace for centralized logging
- **Integration**: Automatic telemetry collection from App Service

## Security Features

1. **🔐 Authentication**: Easy Auth with Microsoft Entra ID integration
2. **🆔 Identity**: Managed Identity eliminates the need for stored credentials
3. **🔒 Network**: HTTPS-only communication with TLS 1.2 minimum
4. **🛡️ Database**: SQL Database accessible only through Managed Identity
5. **🔑 RBAC**: Least privilege access using Azure role assignments

## Scalability & Performance

- **📈 Auto Scaling**: App Service can scale based on demand
- **⚡ Performance**: Application Insights provides performance monitoring
- **📊 Metrics**: Custom metrics and alerts through Azure Monitor
- **🚀 Availability**: Built-in high availability for Azure services

## Environment Variables

The infrastructure is deployed with environment-specific naming:
- `{env}` = Environment name (dev, test, prod)
- All resources follow the naming convention: `TDC-{env}-{resource-type}`

## Next Steps

1. Deploy application code to the App Service
2. Configure custom domain and SSL certificate
3. Set up CI/CD pipeline for automated deployments
4. Configure monitoring alerts and dashboards
5. Implement backup and disaster recovery strategies