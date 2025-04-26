# Technical Context: Clean Architecture Template with Authentication

## Technologies Used

### Core Framework
- **.NET 9**: Latest version of the .NET platform
- **ASP.NET Core 9**: Web framework for building APIs and web applications
- **Entity Framework Core 9**: ORM for data access

### Authentication & Identity
- **Microsoft.AspNetCore.Identity**: Framework for local identity management
- **Microsoft.Identity.Web**: Library for Microsoft Entra ID integration
- **JWT Bearer Authentication**: Token-based authentication for APIs

### Architecture & Patterns
- **MediatR**: Implementation of mediator pattern for CQRS
- **FluentValidation**: Validation library for request validation
- **AutoMapper**: Object-to-object mapping library

### Database
- **SQL Server**: Primary database provider
- **PostgreSQL**: Alternative database provider
- **EF Core Migrations**: Database schema management

### UI & Frontend
- **ASP.NET Core Blazor**: Component-based UI framework
- **Bootstrap**: CSS framework for responsive design
- **JavaScript/TypeScript**: Client-side scripting

### Testing
- **xUnit**: Testing framework
- **Moq**: Mocking framework for unit tests
- **FluentAssertions**: Fluent assertions for tests
- **Respawn**: Database reset for integration tests

### DevOps & Deployment
- **Docker**: Containerization
- **Aspire**: .NET application hosting and deployment
- **GitHub Actions**: CI/CD pipeline

## Development Setup

### Required Tools
- **Visual Studio 2025** or **Visual Studio Code**
- **.NET 9 SDK**
- **Docker Desktop** (optional, for containerized development)
- **SQL Server** or **PostgreSQL** (local or containerized)

### Local Development Configuration
1. **Database Setup**:
   - Local SQL Server or PostgreSQL instance
   - Connection strings in `appsettings.Development.json`
   - Automatic migrations on startup (development only)

2. **Identity Provider Configuration**:
   - EF Core Identity (default for development)
   - Microsoft Entra ID (requires Azure subscription and app registration)

3. **Environment Variables**:
   - `ASPNETCORE_ENVIRONMENT`: Set to `Development`
   - `IdentityProvider`: `EFCore` or `AzureAD`
   - `DatabaseProvider`: `SqlServer` or `PostgreSQL`

### Project Structure Navigation
- **Solution Structure**: Organized by Clean Architecture layers
- **Project References**: Follow dependency rule (inner layers don't reference outer layers)
- **Namespace Convention**: `CleanArchitectureTemplate.{Layer}.{Module}`

## Technical Constraints

### Framework Constraints
- **.NET 9 Compatibility**: All libraries and components must be compatible
- **Clean Architecture Principles**: Must adhere to dependency rules
- **Cross-Platform Support**: Must run on Windows, macOS, and Linux

### Authentication Constraints
- **Dual Provider Support**: Must support both EF Core Identity and Microsoft Entra ID
- **Common Interface**: All identity operations must go through abstraction layer
- **Separate Storage**: Identity and application data must be stored separately

### Database Constraints
- **Provider Agnostic**: Core logic must not depend on specific database provider
- **Migration Support**: Must support automated and manual migrations
- **Performance**: Must follow EF Core best practices for performance

### Security Constraints
- **HTTPS Enforcement**: All communication must be over HTTPS
- **Token Security**: JWT tokens must follow security best practices
- **Password Policies**: Strong password policies for EF Core Identity
- **CSRF Protection**: Must implement anti-forgery protection
- **XSS Prevention**: Must sanitize user input and implement CSP

## Dependencies

### NuGet Packages

#### Core Layer
- **MediatR**
- **FluentValidation**
- **AutoMapper**

#### Infrastructure Layer
- **Microsoft.EntityFrameworkCore**
- **Microsoft.EntityFrameworkCore.SqlServer**
- **Npgsql.EntityFrameworkCore.PostgreSQL**
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore**
- **Microsoft.Identity.Web**

#### Presentation Layer
- **Microsoft.AspNetCore.Authentication.JwtBearer**
- **Microsoft.AspNetCore.Components.Web**
- **Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore**

#### Testing
- **xUnit**
- **Moq**
- **FluentAssertions**
- **Microsoft.EntityFrameworkCore.InMemory**

### External Dependencies
- **SQL Server** or **PostgreSQL** database
- **Microsoft Entra ID** tenant (for cloud identity)

## Tool Usage Patterns

### Entity Framework Core
- **Code-First Approach**: Domain entities define database schema
- **Migrations**: Generated and applied through dedicated worker
- **Repository Pattern**: Abstracts EF Core implementation details
- **Soft Delete**: Entities implement ISoftDelete for logical deletion

### Identity Management
- **User Management**: Through abstracted IUserService
- **Role-Based Authorization**: Consistent across both identity providers
- **Claims-Based Authorization**: For fine-grained permissions
- **Token Handling**: JWT generation and validation

### API Development
- **RESTful Principles**: Resource-based API design
- **CQRS Pattern**: Commands for writes, queries for reads
- **Validation Pipeline**: Request validation using FluentValidation
- **Error Handling**: Consistent error responses with problem details

### Testing Strategy
- **Unit Tests**: For domain and application logic
- **Integration Tests**: For infrastructure and API endpoints
- **Test Data Builders**: For creating test entities
- **In-Memory Database**: For repository testing

### Logging and Monitoring
- **Structured Logging**: Using Serilog
- **Application Insights**: For production monitoring
- **Health Checks**: Endpoint for monitoring service health
- **Correlation IDs**: For request tracking across components

## Development Workflow

### Feature Development
1. Define domain entities and interfaces
2. Implement application logic (commands/queries)
3. Create infrastructure implementations
4. Expose functionality through API/UI
5. Write tests at each layer

### Database Changes
1. Modify domain entities
2. Generate migration using EF Core tools
3. Test migration locally
4. Apply migration through worker or startup

### Authentication Integration
1. Configure identity provider in settings
2. Implement provider-specific service
3. Register service with dependency injection
4. Use ICurrentUserService in application logic
