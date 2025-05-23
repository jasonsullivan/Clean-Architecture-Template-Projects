# Clean Architecture Template with Authentication
## Development Plan & Scope Document

### 1. Project Overview
**Purpose:**

This project aims to deliver a reusable C# solution template based on Clean Architecture principles, featuring integrated dual authentication support using EF Core Identity and Microsoft Entra ID (formerly Azure AD). The template enables developers to rapidly initiate new applications with enterprise-grade architecture and security, while offering flexibility for various deployment and configuration needs.

**Key Features:**

- Dual authentication options: EF Core Identity and Microsoft Entra ID
- Distinct separation of identity and application data storage
- Clean Architecture with well-defined separation of concerns
- Modular design to facilitate customization and scalability
- Support for multiple database providers (e.g., SQL Server, PostgreSQL)
- Admin setup UI for streamlined initial configuration

### 2. Solution Structure
The solution is organized into layers adhering to Clean Architecture principles, promoting maintainability and clarity. The project structure is as follows:

```
Solution 'Clean Architecture Template Projects'
├── aspire
│   ├── CleanArchitectureTemplate.AppHost
│   └── CleanArchitectureTemplate.ServiceDefaults
├── Solution Items
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

**Explanation:**

- **Core**: Houses domain entities, value objects, and application logic, including service and repository interfaces.
- **Infrastructure**: Implements core interfaces, covering data access, identity providers, and migration tools.
- **Presentation**: Contains the API and web projects to expose application functionality.
- **Shared**: Includes utilities and configurations reusable across layers.
- **Tests**: Provides unit and integration tests for each layer to ensure reliability.

### 3. Authentication Architecture
The template supports two authentication providers:

- **EF Core Identity**: Local identity management using Entity Framework Core.
- **Microsoft Entra ID**: Cloud-based identity management via Azure AD.

**Key Decisions:**

- Both providers are abstracted through a common interface (ICurrentUserService), keeping application logic independent of the authentication provider.
- Identity and application data are stored separately:
  - EF Core Identity: Utilizes a dedicated identity database.
  - Microsoft Entra ID: Manages identity data in the cloud, with application data stored locally.
- A factory pattern determines the active identity provider based on configuration settings.
- Domain model includes a provider-agnostic UserAccount entity:
  - Keeps domain logic separate from identity implementation details
  - Provides consistent user representation regardless of authentication provider
  - Allows for application-specific user attributes and relationships

**User, Role, and Permission Management:**

- **EF Core Identity Approach**:
  - Uses ApplicationUser class (inherits from IdentityUser) in the Identity layer
  - Uses ApplicationRole class (inherits from IdentityRole) for roles
  - Implements fine-grained Permission entities for authorization
  - Maps ApplicationUser to domain UserAccount via mapper service
  - Maps IdentityRoles to domain ApplicationRole entities
  - Associates Permissions with Roles for granular access control
  - Domain code never works directly with Identity classes
  
- **Microsoft Entra ID Approach**:
  - Maintains a synchronized copy of user data in the UserAccount entity
  - Synchronizes Entra ID security groups to domain ApplicationRole entities
  - Implements Permission entities locally in the application
  - Maps Permissions to roles for fine-grained authorization control
  - Implements synchronization mechanisms (on-login, scheduled, or event-based)
  - Maps Entra ID groups to application roles based on configured filters
  - Provides admin UI for managing synchronization and role mappings

**Authentication Flow:**

1. **Authentication**:
   - EF Core Identity users authenticate via local login endpoints.
   - Microsoft Entra ID users are redirected to Azure AD for authentication.
2. **Authorization**:
   - The API validates tokens and retrieves user details.
3. **Application Logic**:
   - Leverages ICurrentUserService to access user information (e.g., user ID) for domain operations.

### 4. Database Architecture
The template separates identity and application data:

- **Identity Database** (EF Core Identity only): Stores user data (e.g., AspNetUsers, AspNetRoles).
- **Application Database**: Stores domain-specific entities (e.g., orders, products) including UserAccount.

**User Data Architecture:**

- **Domain Layer Entities**:
  - UserAccount: Domain representation of a user, independent of authentication provider
  - ApplicationRole: Domain representation of a role or security group
  - UserRole: Join entity representing the many-to-many relationship between users and roles
  - Permission: Fine-grained access control entity
  - RolePermission: Join entity mapping permissions to roles

- **Permission System**:
  - Permissions represent specific actions (Create, Read, Update, Delete, Execute)
  - Permissions can be scoped to specific entities or processes
  - Roles are assigned collections of permissions
  - Permission checks occur at the application service layer
  - Claims-based authorization integrates with the permission system

- **For EF Core Identity**:
  - ApplicationUser extends IdentityUser but remains in the infrastructure layer
  - A mapper transforms ApplicationUser to UserAccount for domain use
  - Similar mapping occurs for roles (IdentityRole to ApplicationRole)
  - Permissions are managed entirely within the application

- **For Microsoft Entra ID**:
  - Synchronization mechanisms pull user data from Microsoft Graph API
  - Security groups are synchronized to ApplicationRole entities
  - Permissions are associated with roles in the application
  - Filtering options determine which groups to include/exclude

**Configuration:**

- Two distinct DbContext classes are implemented:
  - IdentityDbContext for identity data.
  - ApplicationDbContext for application data (includes UserAccount).
- Connection strings are specified in appsettings.json or environment variables.

**Database Providers:**

- Supports multiple providers (e.g., SQL Server, PostgreSQL), configurable through the admin setup UI or environment variables.

### 5. Setup and Configuration
**Admin Setup UI:**

- An initial configuration interface allows setup of:
  - Database provider (e.g., SQL Server, PostgreSQL).
  - Identity provider (EF Core Identity or Microsoft Entra ID).
  - Connection strings for identity and application databases.
  - For Entra ID: Security group filtering settings (e.g., group prefix filters)
  - For EF Core Identity: Default roles and permissions
  - Default permissions for standard operations (CRUD)
  - Custom permission definitions for application-specific functions
- The UI activates on first launch if no configuration exists.

**Configuration Options:**

- Settings can be adjusted via the UI or environment variables (e.g., for Docker deployments).
- Default settings are provided for development and testing (e.g., EF Core Identity with SQL Server).
- Role mapping configuration for Entra ID includes group mappings and filtering settings.
- Permission configuration allows defining:
  - Entity-level permissions (e.g., CanCreateUser, CanUpdateProduct)
  - Operation-level permissions (e.g., CanApproveOrder, CanInitiateWorkflow)
  - System-level permissions (e.g., CanConfigureSettings, CanViewAuditLogs)

### 6. Deployment and Usage Scenarios
**Developer Usage:**

- Developers can use default settings (e.g., EF Core Identity with SQL Server) for quick onboarding.
- Abstractions like ICurrentUserService enable seamless integration of user data into domain models (e.g., CreatedBy fields).

**IT Manager, System Admin, and Business Systems Analyst Usage:**

- Deployment is supported via Docker, with configuration managed through the admin setup UI or environment variables.
- Database migrations are handled by a dedicated migration worker project or automatically during setup.
- System Administrators can configure security settings and manage deployment environments.
- Business Systems Analysts can configure role mappings and user access without requiring code changes.

### 7. Technical Requirements
**Framework & Tools:**

- .NET 9, ASP.NET Core 9, Entity Framework Core 9
- Microsoft.Identity.Web for Microsoft Entra ID integration
- Microsoft.AspNetCore.Identity for EF Core Identity
- MediatR for CQRS implementation
- Aspire for service configuration

**Design Patterns:**

- Clean Architecture
- CQRS and Mediator patterns
- Repository and Unit of Work patterns
- Factory pattern for identity provider selection
- Strategy pattern for authentication logic

**Security:**

- Enforces HTTPS
- Implements anti-forgery protection
- Ensures secure secret storage
- Adheres to OAuth 2.0 and OpenID Connect standards
- Permission-based authorization system
- Claims-based authorization pipeline integration
- Auditing of security-related actions
- Authorization handlers for fine-grained access control

### 8. Deliverables
- Full solution template with all projects and layers
- Mechanism for selecting authentication providers
- EF Core Identity implementation with UI
- Microsoft Entra ID integration
- Domain model for UserAccount and ApplicationRole
- Permission-based authorization system:
  - Permission entities and relationships
  - Authorization handlers for permission checks
  - UI for permission management
  - Integration with ASP.NET Core authorization system
- Role synchronization from both identity providers:
  - EF Core Identity: Mapping IdentityRoles to ApplicationRoles
  - Entra ID: Synchronizing security groups as ApplicationRoles
- Features for user, role, and permission management with UI
- Security group filtering and mapping for Entra ID
- Configuration files for the template
- Comprehensive documentation and examples
- Test coverage across all components

### 9. Development Approach
The development will proceed iteratively, emphasizing:

- Initial focus on EF Core Identity implementation
- Use of GitHub Copilot for code generation
- Adherence to standard ASP.NET Core Identity interfaces
- Compliance with official ASP.NET Core security guidelines
- Test-driven development for security-critical components
- Integration of Microsoft Entra ID following EF Core Identity completion
- Implementation of the UserAccount domain model pattern:
  1. Create domain UserAccount entity first
  2. Implement mapper between ApplicationUser and UserAccount for EF Core Identity
  3. Create synchronization mechanism between Entra ID and UserAccount for Microsoft Entra ID
  4. Develop a unified ICurrentUserService interface for access to UserAccount data

### 10. Conclusion
This Clean Architecture Template offers a robust, enterprise-ready foundation for .NET applications, accommodating both local and cloud-based authentication. With clear separation of concerns, modularity, and an intuitive setup process, it meets the needs of developers and IT managers alike, balancing enterprise requirements with simplicity for smaller projects.

## Solution and Project Structure Diagram for Developers
Below is a simplified diagram of the solution and project structure to guide developers in understanding the organization:

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

**Key Points:**

- **Core Layer**: Defines domain entities, value objects, and application interfaces.
- **Infrastructure Layer**: Implements data access, identity providers, and migration tools.
- **Presentation Layer**: Exposes the API and web interfaces.
- **Shared Layer**: Provides reusable utilities and configurations.
- **Tests**: Ensures quality and reliability across all layers.

This structure promotes modularity, testability, and maintainability, laying a solid foundation for scalable application development.

This development plan consolidates all prior decisions, offering a clear roadmap for stakeholders and developers. Let me know if you need further details on any section!