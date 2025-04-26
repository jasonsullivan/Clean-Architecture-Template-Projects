# Technical Context: Clean Architecture Template with Authentication

## Framework and Tools

### Core Framework
- **.NET 9**: Latest version of the .NET platform
- **ASP.NET Core 9**: Web framework for building APIs and web applications
- **Entity Framework Core 9**: ORM for data access

### Authentication and Authorization
- **Microsoft.AspNetCore.Identity**: Framework for local identity management
- **Microsoft.Identity.Web**: Library for Microsoft Entra ID integration
- **Microsoft Graph SDK**: For accessing Microsoft Entra ID user and group data

### Architecture and Design
- **MediatR**: Implementation of the mediator pattern for CQRS
- **FluentValidation**: Validation library for request validation
- **AutoMapper**: Object-to-object mapping library

### API and Web UI
- **ASP.NET Core MVC**: For API controllers
- **ASP.NET Core Blazor**: For web UI components
- **Swagger/OpenAPI**: For API documentation

### Testing
- **xUnit**: Testing framework
- **Moq**: Mocking library for unit tests
- **FluentAssertions**: Fluent assertions for tests
- **Respawn**: Database reset tool for integration tests

### Development and Deployment
- **Aspire**: For service configuration and orchestration
- **Docker**: Containerization for deployment
- **GitHub Actions**: CI/CD pipeline

## Technical Requirements

### Security Requirements
- **HTTPS Enforcement**: All communication must be encrypted
- **Anti-Forgery Protection**: Prevention of CSRF attacks
- **Secure Secret Storage**: Using user secrets or key vault
- **OAuth 2.0 and OpenID Connect**: Standards compliance for authentication
- **Claims-Based Authorization**: For fine-grained access control

### Performance Requirements
- **Efficient Queries**: Optimized database access
- **Caching**: Where appropriate for performance
- **Asynchronous Operations**: For non-blocking I/O

### Scalability Requirements
- **Stateless Design**: For horizontal scaling
- **Database Independence**: Support for different providers
- **Configuration Flexibility**: For different deployment scenarios

### Maintainability Requirements
- **Clean Architecture**: Clear separation of concerns
- **Comprehensive Testing**: Unit and integration tests
- **Consistent Coding Standards**: Using .editorconfig
- **Documentation**: XML comments and README files

## Development Environment

### Required Tools
- **Visual Studio 2025** or **Visual Studio Code**: IDE
- **SQL Server** or **PostgreSQL**: Database
- **.NET 9 SDK**: Development kit
- **Docker Desktop**: For containerization
- **Git**: Version control

### Development Workflow
1. **Setup**: Clone repository and restore packages
2. **Configuration**: Set up user secrets for local development
3. **Database**: Apply migrations to create local databases
4. **Run**: Start the application with the appropriate configuration

### Project Configuration
- **appsettings.json**: Base configuration
- **appsettings.Development.json**: Development-specific settings
- **User Secrets**: Local secrets (connection strings, API keys)
- **Environment Variables**: For deployment environments

## Database Configuration

### Database Providers
- **SQL Server**: Primary supported provider
- **PostgreSQL**: Alternative provider
- **SQLite**: For testing and development

### Connection Strings
- **IdentityConnection**: For identity database (EF Core Identity)
- **ApplicationConnection**: For application database

### Migration Strategy
- **Code-First Migrations**: Using EF Core migrations
- **Migration Worker**: For automated deployment migrations
- **Separate Migration Projects**: For identity and application databases

## Authentication Configuration

### EF Core Identity Configuration
- **User and Role Classes**: ApplicationUser and ApplicationRole
- **Password Policies**: Configurable password requirements
- **Token Providers**: For email confirmation, password reset, etc.
- **Cookie Authentication**: For web applications
- **JWT Authentication**: For APIs

### Microsoft Entra ID Configuration
- **App Registration**: Required in Azure portal
- **Authentication Flow**: Authorization code flow with PKCE
- **Group Claims**: For role-based authorization
- **Token Validation**: JWT validation parameters
- **Synchronization Settings**: For user and group synchronization

## Development Approach

### Code Organization
- **Feature Folders**: Organizing code by feature rather than type
- **CQRS Separation**: Commands and queries in separate files
- **Interface Segregation**: Small, focused interfaces
- **Dependency Injection**: For loose coupling

### Coding Standards
- **C# Coding Conventions**: Following Microsoft guidelines
- **Nullable Reference Types**: Enabled for all projects
- **Async/Await**: Used consistently for asynchronous operations
- **XML Documentation**: For public APIs and interfaces

### Testing Strategy
- **Unit Tests**: For business logic and application services
- **Integration Tests**: For repositories and API endpoints
- **Test Data Builders**: For creating test data
- **Mock Objects**: For isolating dependencies

### Error Handling
- **Global Exception Handling**: Middleware for API exceptions
- **Structured Error Responses**: Consistent error format
- **Logging**: Using built-in logging framework
- **Validation Errors**: Returned as 400 Bad Request with details

## Deployment Considerations

### Docker Support
- **Dockerfile**: For containerization
- **Docker Compose**: For local multi-container setup
- **Environment Variables**: For container configuration

### CI/CD Pipeline
- **Build**: Compile and run tests
- **Package**: Create deployment artifacts
- **Deploy**: Deploy to target environment
- **Verify**: Run smoke tests

### Environment Configuration
- **Development**: Local development environment
- **Testing**: For automated tests
- **Staging**: Pre-production environment
- **Production**: Live environment

### Monitoring and Diagnostics
- **Logging**: Structured logging with Serilog
- **Health Checks**: Endpoint for monitoring
- **Metrics**: For performance monitoring
- **Tracing**: For request tracking
