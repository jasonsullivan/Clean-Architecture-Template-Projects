# Active Context: Clean Architecture Template with Authentication

## Current Work Focus

The project is in its initial setup phase. The solution structure has been created with all necessary projects following Clean Architecture principles. The current focus is on:

1. **Core Domain Modeling**: Defining the essential domain entities and interfaces that will form the foundation of the application.

2. **Authentication Infrastructure**: Setting up the dual authentication providers (EF Core Identity and Microsoft Entra ID) with proper abstraction.

3. **Database Configuration**: Establishing the database contexts and ensuring proper separation between identity and application data.

4. **Admin Setup UI**: Developing the initial configuration interface for administrators to select database and identity providers.

## Recent Changes

As this is the initial phase of the project, the recent changes include:

1. **Solution Structure Creation**: The full solution structure has been established with all projects and proper references.

2. **Project References**: Set up project references following Clean Architecture dependency rules.

3. **Basic Project Files**: Added placeholder files in each project to establish the structure.

4. **Development Plan Documentation**: Created comprehensive development plan outlining the project scope, architecture, and technical requirements.

## Next Steps

The immediate next steps for the project are:

1. **Domain Layer Implementation**:
   - Define base entity classes with audit properties
   - Create repository interfaces
   - Establish domain events and handlers

2. **Identity Abstraction Layer**:
   - Implement `ICurrentUserService` interface
   - Create factory for identity provider selection
   - Set up authentication middleware configuration

3. **EF Core Identity Implementation**:
   - Configure IdentityDbContext
   - Set up user and role management
   - Implement token generation and validation

4. **Database Context Setup**:
   - Create ApplicationDbContext
   - Configure entity mappings
   - Set up database provider selection mechanism

5. **Admin Setup UI Development**:
   - Design configuration interface
   - Implement provider selection logic
   - Create connection string management

## Active Decisions and Considerations

### Authentication Strategy
- **Decision**: Implement EF Core Identity first, then add Microsoft Entra ID support
- **Rationale**: EF Core Identity provides a more straightforward development experience for local testing
- **Consideration**: Need to ensure the abstraction layer is robust enough to support both providers without code changes

### Database Architecture
- **Decision**: Separate databases for identity and application data
- **Rationale**: Provides cleaner separation of concerns and allows for different scaling strategies
- **Consideration**: Need to ensure proper transaction handling when operations span both databases

### Configuration Approach
- **Decision**: Use a combination of environment variables and admin UI for configuration
- **Rationale**: Provides flexibility for different deployment scenarios
- **Consideration**: Need to establish precedence rules for configuration sources

### Project Structure
- **Decision**: Organize by Clean Architecture layers first, then by feature within each layer
- **Rationale**: Emphasizes architectural boundaries while allowing for feature-based organization
- **Consideration**: May need to adjust as the project grows to maintain maintainability

## Important Patterns and Preferences

### Coding Standards
- Use C# 12 features where appropriate
- Follow Microsoft's .NET coding conventions
- Use nullable reference types with appropriate annotations
- Prefer async/await for all I/O operations
- Use .ConfigureAwait(false) in library based async/await calls

### Architecture Enforcement
- Strict adherence to Clean Architecture dependency rules
- Use of interfaces for all external dependencies
- Dependency injection for all services
- No direct database access from Application layer

### Testing Approach
- Unit tests for all business logic
- Integration tests for infrastructure components
- Test-driven development for critical components
- Use of test doubles (mocks, stubs) for external dependencies

### Error Handling
- Use of Result pattern for domain operations
- Consistent exception handling strategy
- Detailed logging for troubleshooting
- User-friendly error messages for UI

## Learnings and Project Insights

### Initial Observations
- Clean Architecture provides clear boundaries but requires careful planning for cross-cutting concerns
- Authentication abstraction is complex but essential for provider flexibility
- Database provider abstraction adds complexity but offers deployment flexibility

### Technical Challenges
- Balancing abstraction with simplicity for authentication providers
- Managing separate databases while maintaining data consistency
- Configuring different providers through a unified interface

### Approach Adjustments
- Start with minimal viable implementations and iterate
- Focus on core abstractions before provider-specific details
- Use feature flags to enable/disable capabilities during development

### Knowledge Gaps
- Microsoft Entra ID integration details
- Aspire configuration for multi-project solutions
- PostgreSQL-specific optimizations for EF Core

## Current Environment

### Development Tools
- Visual Studio 2022 / Visual Studio Code
- .NET 9 SDK
- Entity Framework Core 9
- Docker Desktop (for containerized development)

### Testing Environment
- Local development machines
- xUnit for testing framework
- GitHub Actions for CI/CD
