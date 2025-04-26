# System Patterns: Clean Architecture Template with Authentication

## System Architecture

This template implements Clean Architecture, a design philosophy that emphasizes separation of concerns and dependency rules. The architecture is organized into concentric layers:

### Core Layers (Inner)

1. **Domain Layer** (`CleanArchitectureTemplate.Domain`)
   - Contains enterprise-wide business rules and entities
   - Has no dependencies on other layers or external frameworks
   - Defines core domain models, value objects, and domain events
   - Establishes repository interfaces for data access

2. **Application Layer** (`CleanArchitectureTemplate.Application`)
   - Contains application-specific business rules
   - Orchestrates the flow of data to and from domain entities
   - Implements use cases through services and commands/queries
   - Depends only on the Domain layer
   - Defines interfaces that are implemented by outer layers

### Infrastructure Layers (Outer)

3. **Infrastructure Layer** (`CleanArchitectureTemplate.Infrastructure`)
   - Implements interfaces defined in the Application layer
   - Contains data access implementations, external service integrations
   - Manages database contexts and repositories
   - Handles cross-cutting concerns like logging and caching

4. **Identity Infrastructure Layers**
   - **EF Core Identity** (`CleanArchitectureTemplate.Infrastructure.Identity.EFCore`)
     - Implements local identity management using Entity Framework Core
     - Manages users, roles, and claims
   
   - **Azure AD Identity** (`CleanArchitectureTemplate.Infrastructure.Identity.AzureAD`)
     - Implements cloud-based identity management via Microsoft Entra ID
     - Handles token validation and user information retrieval

5. **Migration Worker** (`CleanArchitectureTemplate.Infrastructure.MigrationWorker`)
   - Handles database migrations for both identity and application databases
   - Runs as a separate process during deployment or setup

### Presentation Layers

6. **API Layer** (`CleanArchitectureTemplate.API`)
   - Exposes application functionality through RESTful endpoints
   - Handles HTTP requests and responses
   - Manages authentication and authorization at the API level

7. **Web Layer** (`CleanArchitectureTemplate.Web`)
   - Provides user interface for the application
   - Includes admin setup UI for configuration
   - Implements user authentication flows

### Shared Layer

8. **Shared Layer** (`CleanArchitectureTemplate.Shared`)
   - Contains DTOs and models shared between layers
   - Includes utilities and helpers used across the application
   - Defines constants and enumerations

## Key Technical Decisions

### Authentication Architecture

1. **Dual Provider Support**
   - Both EF Core Identity and Microsoft Entra ID are supported
   - Selection is made at configuration time
   - Common interfaces abstract provider-specific details

2. **Identity Data Separation**
   - Identity data is stored separately from application data
   - EF Core Identity uses a dedicated identity database
   - Microsoft Entra ID stores identity in the cloud

3. **User Context Abstraction**
   - `ICurrentUserService` provides a unified interface for user information
   - Implementation varies based on the selected identity provider
   - Application code remains unchanged regardless of provider

### Database Architecture

1. **Multiple Database Support**
   - Template supports various database providers (SQL Server, PostgreSQL)
   - Provider selection is made at configuration time
   - Entity Framework Core is used for data access

2. **Separate Contexts**
   - `IdentityDbContext` manages identity data (for EF Core Identity)
   - `ApplicationDbContext` manages application-specific data
   - Contexts can use different database providers if needed

3. **Migration Strategy**
   - Migrations are handled by a dedicated worker
   - Supports both automatic and manual migration application
   - Ensures database schema matches application requirements

## Design Patterns in Use

### Architectural Patterns

1. **Clean Architecture**
   - Separation of concerns through layers
   - Dependency rule: inner layers know nothing about outer layers
   - Use of interfaces to maintain the dependency rule

2. **CQRS (Command Query Responsibility Segregation)**
   - Commands for state changes
   - Queries for data retrieval
   - Implemented using MediatR

3. **Dependency Injection**
   - Services are registered with the DI container
   - Dependencies are injected rather than created
   - Facilitates testing and loose coupling

### Design Patterns

1. **Repository Pattern**
   - Abstracts data access logic
   - Defined in Domain layer, implemented in Infrastructure
   - Provides collection-like interface for domain entities

2. **Unit of Work**
   - Coordinates operations across multiple repositories
   - Ensures transactional consistency
   - Implemented in Infrastructure layer

3. **Factory Pattern**
   - Used for identity provider selection
   - Creates appropriate implementation based on configuration
   - Abstracts creation logic from consumers

4. **Strategy Pattern**
   - Applied to authentication logic
   - Different strategies for different identity providers
   - Common interface for all strategies

5. **Mediator Pattern**
   - Implemented using MediatR
   - Decouples request senders from handlers
   - Supports pipeline behaviors for cross-cutting concerns

## Component Relationships

### Authentication Flow

```
User -> Web/API -> Identity Provider -> ICurrentUserService -> Application Services -> Domain Logic
```

1. User initiates authentication through Web or API
2. Request is routed to appropriate identity provider
3. Upon successful authentication, user information is available via ICurrentUserService
4. Application services use ICurrentUserService for user context
5. Domain logic receives user information as needed

### Configuration Flow

```
Admin -> Setup UI -> Configuration Service -> Database/Identity Configuration -> Application Startup
```

1. Administrator accesses setup UI
2. Configuration choices are submitted to Configuration Service
3. Service updates configuration settings
4. Application uses configuration during startup
5. Appropriate database and identity providers are initialized

## Critical Implementation Paths

### Identity Provider Integration

1. **Identity Abstraction Layer**
   - `ICurrentUserService` interface in Application layer
   - Provider-specific implementations in Identity layers
   - Factory for selecting appropriate implementation

2. **Authentication Middleware**
   - Configures authentication based on selected provider
   - Handles token validation and user information extraction
   - Populates HttpContext with user details

3. **User Context Propagation**
   - User information flows from authentication to application logic
   - Domain entities capture audit information (e.g., CreatedBy)
   - Authorization decisions based on user identity and roles

### Database Configuration

1. **Provider Selection**
   - Configuration determines database provider
   - Entity Framework Core provider registration
   - Connection string management

2. **Migration Application**
   - Migration worker detects schema changes
   - Applies migrations to both identity and application databases
   - Handles initial seeding of data

3. **Repository Implementation**
   - Generic repository pattern implementation
   - Specific repositories for domain entities
   - Query specifications for complex queries
