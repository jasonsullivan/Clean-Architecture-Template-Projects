# System Patterns: Clean Architecture Template with Authentication

## Clean Architecture Implementation

This template implements Clean Architecture principles with a clear separation of concerns across the following layers:

### Core Layer
- **Domain Layer** (`CleanArchitectureTemplate.Domain`):
  - Contains enterprise-wide business rules and entities
  - Defines domain models, value objects, and domain events
  - Has no dependencies on other layers or external frameworks
  - Includes the provider-agnostic UserAccount and ApplicationRole entities

- **Application Layer** (`CleanArchitectureTemplate.Application`):
  - Implements application-specific business rules
  - Contains interfaces for infrastructure services (repositories, identity services)
  - Implements CQRS pattern using MediatR for commands and queries
  - Includes validation, authorization, and business logic

### Infrastructure Layer
- **Data Access** (`CleanArchitectureTemplate.Infrastructure`):
  - Implements repository interfaces defined in the Application layer
  - Contains database context, entity configurations, and migrations
  - Handles data persistence and retrieval
  - Implements the Unit of Work pattern

- **Identity Providers**:
  - **EF Core Identity** (`CleanArchitectureTemplate.Infrastructure.Identity.EFCore`):
    - Implements local identity management using ASP.NET Core Identity
    - Maps between ApplicationUser and domain UserAccount entities
  
  - **Microsoft Entra ID** (`CleanArchitectureTemplate.Infrastructure.Identity.AzureAD`):
    - Implements cloud-based identity using Microsoft.Identity.Web
    - Synchronizes user data and security groups with domain entities

- **Migration Worker** (`CleanArchitectureTemplate.Infrastructure.MigrationWorker`):
  - Handles database migrations for deployment scenarios
  - Supports different database providers

### Presentation Layer
- **API** (`CleanArchitectureTemplate.API`):
  - Exposes RESTful endpoints for application functionality
  - Handles authentication and authorization
  - Maps between API models and application DTOs

- **Web UI** (`CleanArchitectureTemplate.Web`):
  - Provides user interface for the application
  - Includes admin setup UI for configuration
  - Implements role and permission management interfaces

### Shared Layer
- **Shared** (`CleanArchitectureTemplate.Shared`):
  - Contains cross-cutting concerns and utilities
  - Defines constants and enumerations used across layers
  - Includes shared DTOs for communication between API and Web UI

## Authentication Architecture

The template supports two authentication providers through a common abstraction:

### Common Authentication Interface
- **ICurrentUserService**: Provides access to the current user's information
- **IIdentityService**: Handles user management operations
- **Factory Pattern**: Selects the appropriate identity provider based on configuration

### EF Core Identity Implementation
- Uses ApplicationUser class (inherits from IdentityUser) in the Identity layer
- Uses ApplicationRole class (inherits from IdentityRole) for roles
- Maps to domain UserAccount and ApplicationRole entities
- Stores identity data in a separate database from application data

### Microsoft Entra ID Implementation
- Integrates with Azure AD through Microsoft.Identity.Web
- Synchronizes user data and security groups to local domain entities
- Provides filtering and mapping for security groups
- Stores only application data locally (identity data in cloud)

### Authentication Flow
1. **Authentication**:
   - EF Core Identity: Users authenticate via local login endpoints
   - Microsoft Entra ID: Users are redirected to Azure AD for authentication
2. **Token Validation**:
   - API validates tokens and retrieves user details
3. **User Access**:
   - Application uses ICurrentUserService to access user information

## Database Architecture

The template separates identity and application data:

### Database Contexts
- **IdentityDbContext**: Stores identity data for EF Core Identity
- **ApplicationDbContext**: Stores domain entities and application data

### User Data Architecture
- **Domain Layer Entities**:
  - **UserAccount**: Domain representation of a user, independent of authentication provider
  - **ApplicationRole**: Domain representation of a role or security group
  - **UserRole**: Join entity for the many-to-many relationship between users and roles
  - **Permission**: Fine-grained access control entity
  - **RolePermission**: Join entity mapping permissions to roles

### Provider-Specific Implementations
- **For EF Core Identity**:
  - ApplicationUser extends IdentityUser but remains in the infrastructure layer
  - Mapper transforms ApplicationUser to UserAccount for domain use
  - Similar mapping for roles (IdentityRole to ApplicationRole)

- **For Microsoft Entra ID**:
  - Synchronization mechanisms pull user data from Microsoft Graph API
  - Security groups are synchronized to ApplicationRole entities
  - Filtering options determine which groups to include/exclude

### Database Provider Support
- SQL Server
- PostgreSQL
- Configurable through admin setup UI or environment variables

## Authorization System

The template implements a permission-based authorization system:

### Permission Model
- **Permissions**: Represent specific actions (Create, Read, Update, Delete, Execute)
- **Scopes**: Permissions can be scoped to specific entities or processes
- **Roles**: Collections of permissions assigned to users
- **Claims**: Generated from permissions for ASP.NET Core authorization

### Authorization Flow
1. **Permission Definition**:
   - Permissions are defined for entities and operations
   - Permissions are grouped into roles
2. **Role Assignment**:
   - Users are assigned to roles (directly or via security groups)
3. **Permission Checks**:
   - Application services check permissions before operations
   - API controllers use authorization attributes
4. **Claims-Based Authorization**:
   - Permissions are converted to claims
   - ASP.NET Core authorization handlers validate claims

## Key Design Patterns

### Repository Pattern
- Abstracts data access logic
- Defined in Application layer, implemented in Infrastructure
- Enables testability and separation from database implementation

### Unit of Work Pattern
- Coordinates transactions across repositories
- Ensures data consistency
- Implemented in Infrastructure layer

### CQRS Pattern
- Separates read and write operations
- Commands modify state, queries retrieve data
- Implemented using MediatR

### Mediator Pattern
- Decouples request handlers from controllers
- Enables cross-cutting concerns (validation, logging, authorization)
- Implemented using MediatR

### Factory Pattern
- Selects appropriate identity provider based on configuration
- Creates concrete implementations of identity interfaces

### Strategy Pattern
- Implements different authentication strategies
- Allows switching between identity providers without changing application code

### Mapper Pattern
- Maps between infrastructure entities and domain entities
- Keeps domain model clean from infrastructure concerns

## Configuration and Setup

### Admin Setup UI
- Provides interface for initial configuration
- Configures database providers and connection strings
- Sets up identity provider selection
- Manages role and permission definitions

### Configuration Options
- Settings stored in appsettings.json
- Environment variables for deployment scenarios
- In-memory configuration for testing

### Deployment Support
- Docker container configuration
- Traditional hosting options
- Environment-specific settings
