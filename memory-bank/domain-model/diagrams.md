# Domain Model Diagrams

This document provides visual representations of the domain model to help understand the structure and relationships between entities, value objects, and aggregates.

## Class Diagram: Core Domain Model

The following diagram shows the core entities and their relationships in the domain model:

```mermaid
classDiagram
    %% Entities
    class UserAccount {
        +UserAccountId Id
        +Username Username
        +Email Email
        +PersonName Name
        +UserStatus Status
        +CreatedAt CreatedAt
        +DateTime? LastLoggedIn
        +AuthenticationProvider AuthenticationProvider
        +IReadOnlyCollection~UserRole~ Roles
        +Create(Username, Email, PersonName, AuthenticationProvider) UserAccount
        +UpdateEmail(Email) Result
        +UpdateName(PersonName) Result
        +Deactivate() Result
        +Activate() Result
        +Lock() Result
        +Unlock() Result
        +RecordLogin() Result
        +AssignToRole(ApplicationRole) Result
        +RemoveFromRole(RoleId) Result
    }

    class ApplicationRole {
        +RoleId Id
        +RoleName Name
        +Description Description
        +CreatedAt CreatedAt
        +IReadOnlyCollection~RolePermission~ Permissions
        +Create(RoleName, Description) ApplicationRole
        +UpdateName(RoleName) Result
        +UpdateDescription(Description) Result
        +AddPermission(Permission) Result
        +RemovePermission(PermissionId) Result
    }

    class Permission {
        +PermissionId Id
        +PermissionName Name
        +Description Description
        +PermissionType Type
        +ResourceName Resource
        +bool IsSystemPermission
        +Create(PermissionName, Description, PermissionType, ResourceName, bool) Permission
        +UpdateName(PermissionName) Result
        +UpdateDescription(Description) Result
    }

    class UserRole {
        +UserAccountId UserId
        +RoleId RoleId
        +DateTime AssignedAt
        +Create(UserAccountId, RoleId) UserRole
    }

    class RolePermission {
        +RoleId RoleId
        +PermissionId PermissionId
        +DateTime AssignedAt
        +Create(RoleId, PermissionId) RolePermission
    }

    %% Relationships
    UserAccount "1" -- "many" UserRole : contains
    ApplicationRole "1" -- "many" UserRole : referenced by
    ApplicationRole "1" -- "many" RolePermission : contains
    Permission "1" -- "many" RolePermission : referenced by
```

## Class Diagram: Value Objects

The following diagram shows the value objects used in the domain model:

```mermaid
---
id: 0665e223-5260-4975-a1bb-9e1eae8cbdd0
---
classDiagram
    %% Identity Value Objects
    class UserAccountId {
        +Guid Value
        -UserAccountId(Guid)
        +Create(Guid) UserAccountId
        +CreateUnique() UserAccountId
    }

    class RoleId {
        +Guid Value
        -RoleId(Guid)
        +Create(Guid) RoleId
        +CreateUnique() RoleId
    }

    class PermissionId {
        +Guid Value
        -PermissionId(Guid)
        +Create(Guid) PermissionId
        +CreateUnique() PermissionId
    }

    %% User-Related Value Objects
    class Username {
        +string Value
        -Username(string)
        +Create(string) Username
    }

    class Email {
        +string Value
        -Email(string)
        +Create(string) Email
        +GetDomain() string
        +GetLocalPart() string
    }

    class PersonName {
        +string FirstName
        +string LastName
        +string FullName
        -PersonName(string, string)
        +Create(string, string) PersonName
    }

    class UserStatus {
        +string Value
        -UserStatus(string)
        +Active UserStatus
        +Inactive UserStatus
        +Locked UserStatus
        +PendingActivation UserStatus
        +Create(string) UserStatus
        +IsActive bool
        +IsInactive bool
        +IsLocked bool
        +IsPendingActivation bool
    }

    class AuthenticationProvider {
        +string Value
        -AuthenticationProvider(string)
        +EFCoreIdentity AuthenticationProvider
        +MicrosoftEntraID AuthenticationProvider
        +Create(string) AuthenticationProvider
        +IsEFCoreIdentity bool
        +IsMicrosoftEntraID bool
    }

    %% Role and Permission Value Objects
    class RoleName {
        +string Value
        -RoleName(string)
        +Create(string) RoleName
    }

    class PermissionName {
        +string Value
        -PermissionName(string)
        +Create(string) PermissionName
    }

    class PermissionType {
        +string Value
        -PermissionType(string)
        +Create PermissionType
        +Read PermissionType
        +Update PermissionType
        +Delete PermissionType
        +Execute PermissionType
        +Create(string) PermissionType
    }

    class ResourceName {
        +string Value
        -ResourceName(string)
        +Create(string) ResourceName
        +Global ResourceName
        +IsGlobal bool
    }

    %% Other Value Objects
    class Description {
        +string Value
        -Description(string)
        +Create(string) Description
        +Empty Description
    }

    class CreatedAt {
        +DateTime Value
        -CreatedAt(DateTime)
        +Create(DateTime) CreatedAt
        +Now() CreatedAt
    }

    %% Relationships to Entities
    UserAccountId -- UserAccount : identifies
    RoleId -- ApplicationRole : identifies
    PermissionId -- Permission : identifies
    Username -- UserAccount : property of
    Email -- UserAccount : property of
    PersonName -- UserAccount : property of
    UserStatus -- UserAccount : property of
    AuthenticationProvider -- UserAccount : property of
    RoleName -- ApplicationRole : property of
    PermissionName -- Permission : property of
    PermissionType -- Permission : property of
    ResourceName -- Permission : property of
    Description -- ApplicationRole : property of
    Description -- Permission : property of
    CreatedAt -- UserAccount : property of
    CreatedAt -- ApplicationRole : property of
```

## Aggregate Boundaries

The following diagram shows the aggregate boundaries in the domain model:

```mermaid
graph TD
    subgraph "UserAccount Aggregate"
        UA[UserAccount]
        UR[UserRole]
        UA -->|contains| UR
    end

    subgraph "ApplicationRole Aggregate"
        AR[ApplicationRole]
        RP[RolePermission]
        AR -->|contains| RP
    end

    subgraph "Permission Aggregate"
        P[Permission]
    end

    %% Cross-Aggregate References (by ID only)
    UR -.->|references by ID| AR
    RP -.->|references by ID| P
```

## Domain Events Flow

The following diagram shows the flow of domain events in the system:

```mermaid
sequenceDiagram
    participant UA as UserAccount
    participant Repo as UserAccountRepository
    participant Disp as DomainEventDispatcher
    participant H1 as EmailHandler
    participant H2 as AuditLogHandler

    UA->>UA: Create()
    Note over UA: Raises UserAccountCreatedEvent
    UA->>Repo: AddAsync(userAccount)
    Repo->>Repo: Save to database
    Repo->>Disp: DispatchEventsAsync(events)
    Disp->>H1: Handle(UserAccountCreatedEvent)
    Disp->>H2: Handle(UserAccountCreatedEvent)
    H1->>H1: Send welcome email
    H2->>H2: Create audit log entry
    Repo->>UA: ClearDomainEvents()
```

## Authentication Flow

The following diagram shows the authentication flow for both providers:

```mermaid
graph TD
    subgraph "EF Core Identity Authentication"
        EF1[User enters credentials]
        EF2[EFCoreIdentityProvider authenticates]
        EF3[Create UserAccount if not exists]
        EF4[Generate JWT token]
        EF5[Return token to client]
        
        EF1 --> EF2
        EF2 --> EF3
        EF3 --> EF4
        EF4 --> EF5
    end

    subgraph "Microsoft Entra ID Authentication"
        MS1[Redirect to Microsoft login]
        MS2[User authenticates with Microsoft]
        MS3[Receive token from Microsoft]
        MS4[Validate token]
        MS5[Create/Update UserAccount]
        MS6[Generate application token]
        MS7[Return token to client]
        
        MS1 --> MS2
        MS2 --> MS3
        MS3 --> MS4
        MS4 --> MS5
        MS5 --> MS6
        MS6 --> MS7
    end
```

## Authorization Flow

The following diagram shows the authorization flow:

```mermaid
graph TD
    A[Request with token] --> B[Extract user ID from token]
    B --> C[Load UserAccount]
    C --> D[Get user's roles]
    D --> E[Get permissions for each role]
    E --> F{Check if user has required permission}
    F -->|Yes| G[Allow access]
    F -->|No| H[Deny access]
```

## Factory Pattern for Authentication Providers

The following diagram shows the factory pattern used for authentication providers:

```mermaid
classDiagram
    class IAuthenticationProvider {
        <<interface>>
        +AuthenticateAsync(credentials) Task~Result~
        +GetUserInfoAsync(token) Task~UserInfo~
        +ValidateTokenAsync(token) Task~bool~
    }

    class IAuthenticationProviderFactory {
        <<interface>>
        +CreateProvider(AuthenticationProvider) IAuthenticationProvider
    }

    class AuthenticationProviderFactory {
        -IServiceProvider _serviceProvider
        +CreateProvider(AuthenticationProvider) IAuthenticationProvider
    }

    class EFCoreIdentityProvider {
        +AuthenticateAsync(credentials) Task~Result~
        +GetUserInfoAsync(token) Task~UserInfo~
        +ValidateTokenAsync(token) Task~bool~
    }

    class MicrosoftEntraIDProvider {
        +AuthenticateAsync(credentials) Task~Result~
        +GetUserInfoAsync(token) Task~UserInfo~
        +ValidateTokenAsync(token) Task~bool~
    }

    IAuthenticationProvider <|.. EFCoreIdentityProvider
    IAuthenticationProvider <|.. MicrosoftEntraIDProvider
    IAuthenticationProviderFactory <|.. AuthenticationProviderFactory
    AuthenticationProviderFactory ..> EFCoreIdentityProvider : creates
    AuthenticationProviderFactory ..> MicrosoftEntraIDProvider : creates
```

## Domain Services Interaction

The following diagram shows how domain services interact with each other and with repositories:

```mermaid
graph TD
    subgraph "Application Layer"
        AS[AuthenticationService]
        URS[UserRegistrationService]
    end

    subgraph "Domain Layer"
        UUC[UserUniquenessChecker]
        PHS[PasswordHashingService]
        UAS[UserAuthorizationService]
        APF[AuthenticationProviderFactory]
        CUS[CurrentUserService]
    end

    subgraph "Infrastructure Layer"
        UAR[UserAccountRepository]
        ARR[ApplicationRoleRepository]
        PR[PermissionRepository]
    end

    AS --> APF
    AS --> UAR
    URS --> UUC
    URS --> PHS
    URS --> UAR
    UUC --> UAR
    UAS --> UAR
    UAS --> ARR
    UAS --> PR
    CUS --> UAR
    CUS --> UAS
```

## Entity-Repository Relationship

The following diagram shows the relationship between entities and repositories:

```mermaid
classDiagram
    class IUserAccountRepository {
        <<interface>>
        +GetByIdAsync(UserAccountId) Task~UserAccount~
        +GetByUsernameAsync(Username) Task~UserAccount~
        +GetByEmailAsync(Email) Task~UserAccount~
        +GetUsersByRoleIdAsync(RoleId) Task~IReadOnlyCollection~UserAccount~~
        +AddAsync(UserAccount) Task
        +UpdateAsync(UserAccount) Task
        +DeleteAsync(UserAccountId) Task
    }

    class IApplicationRoleRepository {
        <<interface>>
        +GetByIdAsync(RoleId) Task~ApplicationRole~
        +GetByNameAsync(RoleName) Task~ApplicationRole~
        +GetByIdsAsync(IEnumerable~RoleId~) Task~IReadOnlyCollection~ApplicationRole~~
        +AddAsync(ApplicationRole) Task
        +UpdateAsync(ApplicationRole) Task
        +DeleteAsync(RoleId) Task
    }

    class IPermissionRepository {
        <<interface>>
        +GetByIdAsync(PermissionId) Task~Permission~
        +GetByNameTypeAndResourceAsync(PermissionName, PermissionType, ResourceName) Task~Permission~
        +GetByIdsAsync(IEnumerable~PermissionId~) Task~IReadOnlyCollection~Permission~~
        +GetByResourceAsync(ResourceName) Task~IReadOnlyCollection~Permission~~
        +GetSystemPermissionsAsync() Task~IReadOnlyCollection~Permission~~
        +AddAsync(Permission) Task
        +UpdateAsync(Permission) Task
        +DeleteAsync(PermissionId) Task
    }

    class UserAccount {
        +UserAccountId Id
        +Username Username
        +Email Email
        +PersonName Name
        +UserStatus Status
        +CreatedAt CreatedAt
        +DateTime? LastLoggedIn
        +AuthenticationProvider AuthenticationProvider
        +IReadOnlyCollection~UserRole~ Roles
    }

    class ApplicationRole {
        +RoleId Id
        +RoleName Name
        +Description Description
        +CreatedAt CreatedAt
        +IReadOnlyCollection~RolePermission~ Permissions
    }

    class Permission {
        +PermissionId Id
        +PermissionName Name
        +Description Description
        +PermissionType Type
        +ResourceName Resource
        +bool IsSystemPermission
    }

    IUserAccountRepository ..> UserAccount : operates on
    IApplicationRoleRepository ..> ApplicationRole : operates on
    IPermissionRepository ..> Permission : operates on
```

## Domain Events Hierarchy

The following diagram shows the hierarchy of domain events:

```mermaid
classDiagram
    class IDomainEvent {
        <<interface>>
        +DateTime OccurredOn
    }

    %% User Events
    class UserAccountCreatedEvent {
        +UserAccountId Id
        +Username Username
        +Email Email
        +AuthenticationProvider AuthenticationProvider
        +DateTime OccurredOn
    }

    class UserEmailChangedEvent {
        +UserAccountId Id
        +Email OldEmail
        +Email NewEmail
        +DateTime OccurredOn
    }

    class UserNameChangedEvent {
        +UserAccountId Id
        +PersonName NewName
        +DateTime OccurredOn
    }

    class UserStatusChangedEvent {
        +UserAccountId Id
        +DateTime OccurredOn
    }

    class UserActivatedEvent {
        +UserAccountId Id
        +DateTime OccurredOn
    }

    class UserDeactivatedEvent {
        +UserAccountId Id
        +DateTime OccurredOn
    }

    class UserLockedEvent {
        +UserAccountId Id
        +DateTime OccurredOn
    }

    class UserUnlockedEvent {
        +UserAccountId Id
        +DateTime OccurredOn
    }

    class UserLoggedInEvent {
        +UserAccountId Id
        +DateTime OccurredOn
    }

    %% Role Events
    class ApplicationRoleCreatedEvent {
        +RoleId Id
        +RoleName Name
        +DateTime OccurredOn
    }

    class ApplicationRoleNameChangedEvent {
        +RoleId Id
        +RoleName OldName
        +RoleName NewName
        +DateTime OccurredOn
    }

    class ApplicationRoleDescriptionChangedEvent {
        +RoleId Id
        +Description NewDescription
        +DateTime OccurredOn
    }

    %% Permission Events
    class PermissionCreatedEvent {
        +PermissionId Id
        +PermissionName Name
        +PermissionType Type
        +ResourceName Resource
        +DateTime OccurredOn
    }

    class PermissionNameChangedEvent {
        +PermissionId Id
        +PermissionName OldName
        +PermissionName NewName
        +DateTime OccurredOn
    }

    class PermissionDescriptionChangedEvent {
        +PermissionId Id
        +Description NewDescription
        +DateTime OccurredOn
    }

    %% Role-Permission Events
    class PermissionAddedToRoleEvent {
        +RoleId RoleId
        +PermissionId PermissionId
        +DateTime OccurredOn
    }

    class PermissionRemovedFromRoleEvent {
        +RoleId RoleId
        +PermissionId PermissionId
        +DateTime OccurredOn
    }

    %% User-Role Events
    class UserAssignedToRoleEvent {
        +UserAccountId UserId
        +RoleId RoleId
        +DateTime OccurredOn
    }

    class UserRemovedFromRoleEvent {
        +UserAccountId UserId
        +RoleId RoleId
        +DateTime OccurredOn
    }

    IDomainEvent <|.. UserAccountCreatedEvent
    IDomainEvent <|.. UserEmailChangedEvent
    IDomainEvent <|.. UserNameChangedEvent
    IDomainEvent <|.. UserStatusChangedEvent
    UserStatusChangedEvent <|-- UserActivatedEvent
    UserStatusChangedEvent <|-- UserDeactivatedEvent
    UserStatusChangedEvent <|-- UserLockedEvent
    UserStatusChangedEvent <|-- UserUnlockedEvent
    IDomainEvent <|.. UserLoggedInEvent
    IDomainEvent <|.. ApplicationRoleCreatedEvent
    IDomainEvent <|.. ApplicationRoleNameChangedEvent
    IDomainEvent <|.. ApplicationRoleDescriptionChangedEvent
    IDomainEvent <|.. PermissionCreatedEvent
    IDomainEvent <|.. PermissionNameChangedEvent
    IDomainEvent <|.. PermissionDescriptionChangedEvent
    IDomainEvent <|.. PermissionAddedToRoleEvent
    IDomainEvent <|.. PermissionRemovedFromRoleEvent
    IDomainEvent <|.. UserAssignedToRoleEvent
    IDomainEvent <|.. UserRemovedFromRoleEvent
```

## Database Schema

The following diagram shows the database schema for the domain model:

```mermaid
erDiagram
    UserAccounts {
        Guid Id PK
        string Username
        string Email
        string FirstName
        string LastName
        string Status
        DateTime CreatedAt
        DateTime LastLoggedIn
        string AuthenticationProvider
    }

    ApplicationRoles {
        Guid Id PK
        string Name
        string Description
        DateTime CreatedAt
    }

    Permissions {
        Guid Id PK
        string Name
        string Description
        string Type
        string Resource
        bool IsSystemPermission
    }

    UserRoles {
        Guid UserId PK,FK
        Guid RoleId PK,FK
        DateTime AssignedAt
    }

    RolePermissions {
        Guid RoleId PK,FK
        Guid PermissionId PK,FK
        DateTime AssignedAt
    }

    UserAccounts ||--o{ UserRoles : "has"
    ApplicationRoles ||--o{ UserRoles : "assigned to"
    ApplicationRoles ||--o{ RolePermissions : "has"
    Permissions ||--o{ RolePermissions : "assigned to"
```

These diagrams provide a comprehensive visual representation of the domain model, helping to understand the structure, relationships, and behavior of the system.
