# Project Brief: Clean Architecture Template with Authentication

## Overview
This project delivers a reusable C# solution template based on Clean Architecture principles, featuring integrated dual authentication support using EF Core Identity and Microsoft Entra ID (formerly Azure AD). The template enables developers to rapidly initiate new applications with enterprise-grade architecture and security, while offering flexibility for various deployment and configuration needs.

## Core Requirements

### Authentication
- Dual authentication options: EF Core Identity and Microsoft Entra ID
- Distinct separation of identity and application data storage
- Common interface abstraction (ICurrentUserService)

### Architecture
- Clean Architecture with well-defined separation of concerns
- Modular design to facilitate customization and scalability
- Support for multiple database providers (e.g., SQL Server, PostgreSQL)

### User Experience
- Admin setup UI for streamlined initial configuration
- Simplified developer onboarding
- Flexible deployment options

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

## Deliverables
- Full solution template with all projects and layers
- Mechanism for selecting authentication providers
- EF Core Identity implementation with UI
- Microsoft Entra ID integration
- Features for user and role management
- Configuration files for the template
- Comprehensive documentation and examples
- Test coverage across all components

## Development Approach
The development will proceed iteratively, emphasizing:
- Initial focus on EF Core Identity implementation
- Use of GitHub Copilot for code generation
- Adherence to standard ASP.NET Core Identity interfaces
- Compliance with official ASP.NET Core security guidelines
- Test-driven development for security-critical components
- Integration of Microsoft Entra ID following EF Core Identity completion

## Project Scope
This template is designed to serve as a foundation for enterprise applications requiring robust authentication, while maintaining flexibility for various deployment scenarios. It balances enterprise requirements with simplicity for smaller projects.
