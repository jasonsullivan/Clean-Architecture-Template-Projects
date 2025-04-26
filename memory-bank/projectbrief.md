# Project Brief: Clean Architecture Template with Authentication

## Project Purpose

This project aims to deliver a reusable C# solution template based on Clean Architecture principles, featuring integrated dual authentication support using EF Core Identity and Microsoft Entra ID (formerly Azure AD). The template enables developers to rapidly initiate new applications with enterprise-grade architecture and security, while offering flexibility for various deployment and configuration needs.

## Key Features

- **Dual Authentication Options**: 
  - EF Core Identity for local identity management
  - Microsoft Entra ID for cloud-based identity management
  
- **Clean Architecture Implementation**:
  - Clear separation of concerns across layers
  - Domain-driven design principles
  - CQRS pattern with MediatR
  
- **Identity Management**:
  - Distinct separation of identity and application data storage
  - Provider-agnostic UserAccount entity in domain model
  - Role and permission-based authorization system
  
- **Flexibility and Modularity**:
  - Support for multiple database providers (SQL Server, PostgreSQL)
  - Configurable authentication providers
  - Modular design for customization and scalability
  
- **Admin Setup and Configuration**:
  - Admin setup UI for streamlined initial configuration
  - Environment variable configuration for deployment scenarios
  - Role mapping and permission management

## Solution Structure

```
Solution 'Clean Architecture Template Projects'
├── aspire
│   ├── CleanArchitectureTemplate.AppHost
│   └── CleanArchitectureTemplate.ServiceDefaults
├── src
│   ├── Core
│   │   ├── CleanArchitectureTemplate.Application
│   │   └── CleanArchitectureTemplate.Domain
│   ├── Infrastructure
│   │   ├── CleanArchitectureTemplate.Infrastructure
│   │   ├── CleanArchitectureTemplate.Infrastructure.Identity.EFCore
│   │   ├── CleanArchitectureTemplate.Infrastructure.Identity.AzureAD
│   │   └── CleanArchitectureTemplate.Infrastructure.MigrationWorker
│   ├── Presentation
│   │   ├── CleanArchitectureTemplate.API
│   │   └── CleanArchitectureTemplate.Web
│   └── Shared
│       └── CleanArchitectureTemplate.Shared
└── tests
    ├── Core
    │   ├── CleanArchitectureTemplate.Application.Tests
    │   └── CleanArchitectureTemplate.Domain.Tests
    ├── Infrastructure
    │   └── CleanArchitectureTemplate.Infrastructure.Tests
    └── Presentation
        ├── CleanArchitectureTemplate.API.Tests
        └── CleanArchitectureTemplate.Web.Tests
```

## Core Deliverables

1. **Complete Solution Template**:
   - Full project structure with all layers implemented
   - Ready-to-use template for new applications

2. **Authentication System**:
   - Dual provider support (EF Core Identity and Microsoft Entra ID)
   - Common interface for authentication services
   - Provider-agnostic domain model for users and roles

3. **Authorization System**:
   - Permission-based authorization
   - Role management and synchronization
   - Integration with ASP.NET Core authorization

4. **Configuration and Setup**:
   - Admin UI for initial configuration
   - Database provider selection
   - Identity provider configuration

5. **Documentation and Examples**:
   - Comprehensive documentation
   - Usage examples for different scenarios
   - Test coverage across all components

## Project Scope

The template is designed to serve as a foundation for enterprise applications requiring:
- Robust authentication and authorization
- Clean, maintainable architecture
- Flexibility in deployment and configuration
- Scalability for growing applications

The project focuses on providing a solid architectural foundation while allowing for customization to meet specific application needs.
