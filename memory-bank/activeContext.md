# Active Context: Clean Architecture Template with Authentication

## Current Development Focus

The project has progressed from domain model design to implementation phase. The solution structure has been created with all the necessary projects following Clean Architecture principles. The memory bank has been established to document the project's architecture, patterns, and progress, with detailed documentation of the domain model. Implementation of core domain entities and value objects is now complete, and we've defined the key interfaces for authentication in the Application layer. The EF Core Identity infrastructure implementation is complete, and we've implemented the role-based access control (RBAC) system in the domain layer. Current focus is on implementing the database migration worker and testing infrastructure.

### Current Activities

1. **Domain Model Implementation**:
   - Implementation of value objects (Email, PersonName, PhoneNumber, UserName, UserStatus, UserAccountId, ApplicationRoleId, PermissionId, PermissionName, RolePermissionId, UserRoleId) is complete
   - Implementation of the UserAccount aggregate root entity is complete
   - Implementation of the ApplicationRole, Permission, RolePermission, and UserRole entities is complete
   - Implementation of domain events for permissions and roles is complete
   - Implementation of domain service interfaces for authorization and role/permission management is complete

2. **Application Layer Implementation**:
   - Key identity interfaces have been defined (ICurrentUserService, IIdentityService, IIdentityProviderFactory)
   - Key authorization interfaces have been defined (IAuthorizationService, IRoleManagementService, IPermissionManagementService)
   - Implementation of the Result pattern for appropriate operations
   - Remaining application services and CQRS handlers to be implemented

3. **Identity Infrastructure Implementation**:
   - Implementation of EF Core Identity entities (ApplicationUser, ApplicationRole, etc.) is complete
   - Implementation of ApplicationIdentityDbContext is complete
   - Implementation of concrete identity services (EFCoreIdentityService, EFCoreCurrentUserService) is complete
   - Mapping between ApplicationUser and UserAccount entities is complete with robust error handling
   - Added comprehensive logging and error handling throughout identity services
   - Multi-database provider support implemented (SQL Server, PostgreSQL, SQLite)

4. **Database Migration Implementation**:
   - Implementation of DatabaseInitializerService as a background service
   - Transaction handling for safe migrations
   - Health checks for monitoring migration status
   - Integration with OpenTelemetry for observability and tracing

5. **Testing Infrastructure**:
   - Infrastructure tests project setup with xUnit
   - Support for in-memory database testing
   - Structure for testing both common infrastructure and identity components

6. **Architecture Implementation**:
   - Implementing the authentication architecture with dual provider support
   - Preparing for the permission-based authorization system implementation

## Recent Decisions and Context

### Role-Based Access Control Implementation

1. **Domain Model for Permissions and Roles**:
   - Implementation of Permission entity with name, description, and type
   - Implementation of ApplicationRole entity with name, description, and collection of permissions
   - Implementation of join entities (RolePermission, UserRole) for many-to-many relationships
   - Enhancement of UserAccount entity with role management capabilities

2. **Permission Naming Convention**:
   - Standardized format "Resource.Action" (e.g., "Users.Create", "Projects.Read")
   - Resource part identifies the entity or area being accessed
   - Action part identifies the operation being performed
   - Validation to ensure consistent naming

3. **Permission Types**:
   - Create: Permission to create resources
   - Read: Permission to read or view resources
   - Update: Permission to update or modify resources
   - Delete: Permission to delete resources
   - Execute: Permission to execute operations or processes
   - Manage: Permission to manage or administer resources
   - FullControl: Full control over resources

4. **Domain Events**:
   - Events for permission creation and modification
   - Events for role creation and permission assignment/removal
   - Events for user role assignment/removal
   - Support for event-driven architecture and audit logging

5. **Service Interfaces**:
   - IAuthorizationService: For checking user permissions and roles
   - IRoleManagementService: For managing roles and their permissions
   - IPermissionManagementService: For managing permissions
   - Clear separation of concerns between services

### Authentication Architecture Decisions

1. **Dual Authentication Providers**:
   - Decision to support both EF Core Identity and Microsoft Entra ID
   - Implementation of common interfaces (ICurrentUserService, IIdentityService) to abstract provider details
   - Factory pattern (IIdentityProviderFactory) to select the appropriate provider based on configuration

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
   - Domain model support for permission checks at the entity level

2. **Role Management**:
   - Synchronization of roles from both identity providers
   - Mapping between provider-specific roles and domain ApplicationRole entities
   - UI for role and permission management
   - Support for system-defined roles that cannot be modified

### Interface Design Decisions

1. **Result Pattern Usage**:
   - Command operations (state changes) return Result or Result<T>
   - Query operations that could fail for domain reasons return Result<T>
   - Simple boolean checks or property getters don't use Result pattern
   - Consistent approach across all interfaces for predictable error handling

2. **Cancellation Token Support**:
   - All asynchronous methods include CancellationToken parameter with default value
   - Supports proper cancellation of operations for responsive applications

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
   - Implemented standardized "Resource.Action" format for permission names
   - Defined clear permission types (Create, Read, Update, Delete, Execute, Manage, FullControl)
   - Added support for creating standard CRUD permissions for resources
   - Implemented efficient permission checking at the domain level
   - Balancing flexibility with complexity in permission design

4. **EF Core Identity Implementation**:
   - How to structure ApplicationIdentityDbContext
   - Mapping between ApplicationUser and domain UserAccount
   - Service registration for EF Core Identity

### Open Questions

1. **Database Migration Strategy**:
   - ✅ Implemented DatabaseInitializerService for handling migrations across providers
   - Approach for upgrading existing applications using the template
   - ✅ Testing strategy for database migrations with infrastructure tests

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

1. **Implement EF Core Identity Infrastructure**:
   - ✅ Complete ApplicationIdentityDbContext implementation
   - ✅ Implement EFCore-specific identity services (EFCoreIdentityService, EFCoreCurrentUserService)
   - ✅ Create mapping between ApplicationUser and UserAccount
   - ✅ Set up service registration extensions

2. **Complete Domain Model Implementation**:
   - ✅ Implement the ApplicationRole and Permission entities
   - ✅ Implement domain events and event handlers
   - ✅ Complete any remaining value objects
   - ✅ Implement domain service interfaces for authorization and role/permission management

3. **Infrastructure Layer Setup**:
   - Implement repository interfaces
   - Create application database context for domain entities
   - ✅ Set up entity configurations and migrations with multi-provider support
   - ✅ Implement database migration worker with health checks

### Short-Term Goals

1. **Authentication System**:
   - Implement authentication flows for both providers
   - Create token validation and generation services
   - Set up user registration and login processes

2. **Authorization System**:
   - ✅ Implement domain model for permission-based authorization
   - Implement concrete authorization service
   - Create authorization handlers for permission checks
   - Set up role and permission management UI

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

4. **Interface Design**:
   - Consistent use of the Result pattern across interfaces improves error handling
   - Proper use of nullable reference types enhances type safety
   - Cancellation token support improves application responsiveness

### Role-Based Access Control Insights

1. **Permission Naming Conventions**:
   - Standardized naming conventions improve clarity and maintainability
   - The "Resource.Action" format provides a clear and intuitive structure
   - Consistent naming enables pattern-based permission checks

2. **Domain-Driven Authorization**:
   - Keeping authorization logic in the domain layer ensures business rules are enforced consistently
   - Domain entities with permission-checking capabilities simplify authorization logic
   - Clear separation between domain authorization and infrastructure implementation

3. **Event-Driven Role Management**:
   - Domain events for role and permission changes enable audit logging and reactive systems
   - Events provide a clear history of authorization changes
   - Event-driven architecture supports decoupled systems and eventual consistency

4. **System-Defined vs. User-Defined Roles**:
   - Distinguishing between system-defined and user-defined roles improves security
   - System-defined roles cannot be modified, ensuring core functionality remains protected
   - User-defined roles provide flexibility for custom authorization scenarios

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
