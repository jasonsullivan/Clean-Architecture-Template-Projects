# Product Context: Clean Architecture Template with Authentication

## Why This Project Exists

This project addresses several key challenges in modern .NET application development:

1. **Reducing Setup Time**: Developers often spend significant time setting up the initial architecture and authentication for new projects. This template eliminates that overhead.

2. **Standardizing Architecture**: Organizations need consistent architectural patterns across projects. This template enforces Clean Architecture principles from the start.

3. **Authentication Flexibility**: Projects may require different authentication approaches based on deployment scenarios (on-premises vs. cloud). This template supports both local and cloud-based identity management.

4. **Separation of Concerns**: Many applications struggle with tight coupling between identity and application logic. This template enforces clear separation.

## Problems It Solves

### For Developers
- Eliminates boilerplate code for authentication and authorization
- Provides a well-structured starting point with clear separation of concerns
- Reduces the learning curve for implementing secure authentication
- Offers flexibility to choose between identity providers without architectural changes

### For IT Managers
- Ensures consistent implementation of security best practices
- Simplifies deployment with configurable database providers
- Provides enterprise-grade architecture suitable for various organizational needs
- Reduces technical debt through proper architectural foundations

### For Organizations
- Accelerates time-to-market for new applications
- Ensures compliance with security standards through built-in best practices
- Reduces maintenance costs through consistent architecture
- Facilitates knowledge transfer between projects due to standardized structure

## How It Should Work

### Authentication Flow

1. **EF Core Identity Path**:
   - Users authenticate via local login endpoints
   - Credentials are validated against the identity database
   - Upon successful authentication, a token is issued
   - Application uses ICurrentUserService to access user information

2. **Microsoft Entra ID Path**:
   - Users are redirected to Azure AD for authentication
   - Azure AD handles credential validation
   - Upon successful authentication, a token is returned
   - Application uses ICurrentUserService to access user information

### Configuration Experience

1. **Initial Setup**:
   - Admin setup UI appears on first launch if no configuration exists
   - Administrator selects database provider and identity provider
   - Administrator configures connection strings
   - System applies configuration and performs initial migrations

2. **Developer Experience**:
   - Clone template repository
   - Run the application with default settings or customize via UI
   - Begin implementing domain-specific functionality
   - Leverage ICurrentUserService for user context in domain operations

3. **Deployment Experience**:
   - Configure environment variables or use admin UI
   - Deploy via Docker or traditional hosting
   - Migration worker handles database setup
   - Application runs with selected identity provider

## User Experience Goals

### For Administrators
- Intuitive setup process with clear configuration options
- Flexibility to choose appropriate database and identity providers
- Ability to modify configuration post-setup
- Clear documentation on security implications of different choices

### For Developers
- Minimal friction when starting new projects
- Clear architecture that guides proper implementation
- Consistent patterns for accessing user information
- Comprehensive examples for common authentication scenarios
- Seamless integration of user context into domain models

### For End Users
- Secure authentication experience
- Consistent login flow regardless of underlying provider
- Appropriate session management
- Proper authorization controls

## Target Scenarios

1. **Enterprise Applications**:
   - Integration with existing Microsoft Entra ID
   - Role-based access control
   - Departmental data isolation

2. **SaaS Applications**:
   - Local identity management with EF Core Identity
   - Multi-tenancy support
   - User self-registration

3. **Internal Tools**:
   - Quick setup with default configurations
   - Integration with organizational identity
   - Simplified deployment

4. **Proof of Concept Projects**:
   - Rapid initialization
   - Default settings for quick starts
   - Easy transition to production-ready architecture
