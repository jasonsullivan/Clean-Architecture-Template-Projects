# Active Context: Clean Architecture Template with Authentication

## Current Development Focus

The project is currently in the domain model design phase. The solution structure has been created with all the necessary projects following Clean Architecture principles. The memory bank has been established to document the project's architecture, patterns, and progress, with detailed documentation of the domain model.

### Current Activities

1. **Domain Model Documentation**:
   - Comprehensive documentation of the domain model in the memory bank
   - Detailed design of value objects, entities, aggregates, and domain events
   - Visual representation of the domain model through class diagrams

2. **Solution Structure**:
   - Basic project structure is in place with core layers defined
   - Projects have been created but require implementation

3. **Architecture Planning**:
   - Finalizing the authentication architecture with dual provider support
   - Detailed design of the domain model for UserAccount and related entities
   - Planning the permission-based authorization system

## Recent Decisions and Context

### Authentication Architecture Decisions

1. **Dual Authentication Providers**:
   - Decision to support both EF Core Identity and Microsoft Entra ID
   - Implementation of a common interface (ICurrentUserService) to abstract provider details
   - Factory pattern to select the appropriate provider based on configuration

2. **Domain Model for Users**:
   - Creation of a provider-agnostic UserAccount entity in the domain layer
   - Separation of identity implementation details from domain logic
   - Mapping between provider-specific user representations and domain UserAccount

3. **Database Separation**:
   - Decision to separate identity data (for EF Core Identity) from application data
   - Implementation of distinct DbContext classes for identity and application data
   - Support for different database providers (SQL Server, PostgreSQL)

### Authorization System Decisions

1. **Permission-Based Authorization**:
   - Implementation of fine-grained permissions for authorization
   - Association of permissions with roles
   - Integration with ASP.NET Core authorization system

2. **Role Management**:
   - Synchronization of roles from both identity providers
   - Mapping between provider-specific roles and domain ApplicationRole entities
   - UI for role and permission management

## Active Considerations and Issues

### Implementation Considerations

1. **Authentication Provider Selection**:
   - How to implement the factory pattern for selecting the appropriate provider
   - Configuration options for specifying the active provider
   - Fallback mechanisms if the configured provider is unavailable

2. **User Synchronization**:
   - Strategies for synchronizing Microsoft Entra ID users and groups
   - Handling conflicts between local and cloud user data
   - Performance considerations for synchronization operations

3. **Permission System Granularity**:
   - Determining the appropriate level of granularity for permissions
   - Balancing flexibility with complexity
   - Performance implications of permission checks

### Open Questions

1. **Database Migration Strategy**:
   - How to handle migrations for different database providers
   - Approach for upgrading existing applications using the template
   - Testing strategy for database migrations

2. **Admin Setup UI**:
   - Design of the admin setup interface
   - Required configuration options
   - User experience for initial setup

3. **Security Group Filtering**:
   - Criteria for filtering Microsoft Entra ID security groups
   - UI for configuring group filters
   - Handling changes to security group membership

## Next Implementation Steps

### Immediate Next Steps

1. **Domain Model Implementation**:
   - Implement the UserAccount, ApplicationRole, and Permission entities as designed
   - Create the value objects for all primitive types
   - Implement domain events and event handlers

2. **Infrastructure Layer Setup**:
   - Implement repository interfaces
   - Create database contexts for identity and application data
   - Set up entity configurations and migrations

3. **Identity Provider Implementation**:
   - Implement EF Core Identity provider
   - Create mapping between ApplicationUser and UserAccount
   - Implement Microsoft Entra ID integration

### Short-Term Goals

1. **Authentication System**:
   - Implement authentication flows for both providers
   - Create token validation and generation services
   - Set up user registration and login processes

2. **Authorization System**:
   - Implement permission-based authorization
   - Create authorization handlers for permission checks
   - Set up role and permission management

3. **Admin Setup UI**:
   - Design and implement the configuration interface
   - Create database provider selection
   - Implement identity provider configuration

### Medium-Term Goals

1. **Testing and Validation**:
   - Create unit tests for core functionality
   - Implement integration tests for authentication and authorization
   - Validate the template with different configuration scenarios

2. **Documentation**:
   - Create comprehensive documentation for the template
   - Provide usage examples for different scenarios
   - Document configuration options and customization points

3. **Deployment and CI/CD**:
   - Set up Docker support
   - Create GitHub Actions workflows
   - Implement automated testing and deployment

## Project Insights and Learnings

### Key Insights

1. **Authentication Complexity**:
   - Supporting multiple authentication providers adds complexity but provides flexibility
   - Abstracting authentication details from domain logic is essential for maintainability
   - Clear interfaces between layers help manage this complexity

2. **Domain Model Independence**:
   - Keeping the domain model independent of authentication implementation details is challenging
   - The UserAccount entity provides a clean abstraction for user data
   - This approach allows for future authentication provider changes without affecting domain logic

3. **Configuration Flexibility**:
   - Balancing flexibility with simplicity in configuration options
   - Environment-specific configuration needs careful planning
   - Admin setup UI simplifies initial configuration but requires thoughtful design

### Patterns and Preferences

1. **Clean Architecture Boundaries**:
   - Strict enforcement of dependencies flowing inward
   - Use of interfaces to define dependencies from inner layers
   - Implementation details confined to outer layers

2. **CQRS Implementation**:
   - Separation of commands and queries
   - Use of MediatR for handling requests
   - Validation and authorization as pipeline behaviors

3. **Testing Approach**:
   - Unit tests for business logic and application services
   - Integration tests for repositories and API endpoints
   - Mock objects for isolating dependencies
