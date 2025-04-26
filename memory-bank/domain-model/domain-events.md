# Domain Events

This document details the domain events in the domain model. Domain events represent significant occurrences within the domain that domain experts care about. They are used to communicate changes between aggregates and to trigger side effects.

## Core Principles for Domain Events

1. **Past Tense Naming**: Events are named in past tense as they represent something that has already happened
2. **Immutability**: Events are immutable; once created, they cannot be changed
3. **Relevant Data**: Events contain all relevant data about what happened
4. **No Side Effects**: Events themselves do not cause side effects; they are handled by event handlers
5. **Serializable**: Events should be serializable for persistence and messaging

## Implementation Pattern

All domain events follow this implementation pattern:

```csharp
public class [EntityName][EventName]Event : IDomainEvent
{
    public [EntityName][EventName]Event([ValueObjectId] id, /* other relevant data */)
    {
        Id = id;
        // Initialize other properties
        OccurredOn = DateTime.UtcNow;
    }

    public [ValueObjectId] Id { get; }
    // Other relevant properties
    public DateTime OccurredOn { get; }
}
```

## User Management Events

### UserAccountCreatedEvent

Raised when a new user account is created.

```csharp
public class UserAccountCreatedEvent : IDomainEvent
{
    public UserAccountCreatedEvent(
        UserAccountId id,
        Username username,
        Email email,
        AuthenticationProvider authProvider)
    {
        Id = id;
        Username = username;
        Email = email;
        AuthenticationProvider = authProvider;
        OccurredOn = DateTime.UtcNow;
    }

    public UserAccountId Id { get; }
    public Username Username { get; }
    public Email Email { get; }
    public AuthenticationProvider AuthenticationProvider { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `UserAccount.Create()`

**Handlers**:
- Send welcome email to the user
- Create audit log entry
- Initialize user preferences
- Synchronize with external systems (if needed)

### UserEmailChangedEvent

Raised when a user's email address is changed.

```csharp
public class UserEmailChangedEvent : IDomainEvent
{
    public UserEmailChangedEvent(
        UserAccountId id,
        Email oldEmail,
        Email newEmail)
    {
        Id = id;
        OldEmail = oldEmail;
        NewEmail = newEmail;
        OccurredOn = DateTime.UtcNow;
    }

    public UserAccountId Id { get; }
    public Email OldEmail { get; }
    public Email NewEmail { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `UserAccount.UpdateEmail()`

**Handlers**:
- Send email verification to the new email address
- Create audit log entry
- Update email in external systems (if needed)

### UserNameChangedEvent

Raised when a user's name is changed.

```csharp
public class UserNameChangedEvent : IDomainEvent
{
    public UserNameChangedEvent(
        UserAccountId id,
        PersonName newName)
    {
        Id = id;
        NewName = newName;
        OccurredOn = DateTime.UtcNow;
    }

    public UserAccountId Id { get; }
    public PersonName NewName { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `UserAccount.UpdateName()`

**Handlers**:
- Create audit log entry
- Update user profile in UI

### UserStatusChangedEvents

A set of events raised when a user's status changes.

#### UserActivatedEvent

```csharp
public class UserActivatedEvent : IDomainEvent
{
    public UserActivatedEvent(UserAccountId id)
    {
        Id = id;
        OccurredOn = DateTime.UtcNow;
    }

    public UserAccountId Id { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `UserAccount.Activate()`

**Handlers**:
- Create audit log entry
- Send notification to user
- Update user status in UI

#### UserDeactivatedEvent

```csharp
public class UserDeactivatedEvent : IDomainEvent
{
    public UserDeactivatedEvent(UserAccountId id)
    {
        Id = id;
        OccurredOn = DateTime.UtcNow;
    }

    public UserAccountId Id { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `UserAccount.Deactivate()`

**Handlers**:
- Create audit log entry
- Send notification to user
- Update user status in UI
- Revoke active sessions

#### UserLockedEvent

```csharp
public class UserLockedEvent : IDomainEvent
{
    public UserLockedEvent(UserAccountId id)
    {
        Id = id;
        OccurredOn = DateTime.UtcNow;
    }

    public UserAccountId Id { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `UserAccount.Lock()`

**Handlers**:
- Create audit log entry
- Send notification to user
- Update user status in UI
- Revoke active sessions

#### UserUnlockedEvent

```csharp
public class UserUnlockedEvent : IDomainEvent
{
    public UserUnlockedEvent(UserAccountId id)
    {
        Id = id;
        OccurredOn = DateTime.UtcNow;
    }

    public UserAccountId Id { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `UserAccount.Unlock()`

**Handlers**:
- Create audit log entry
- Send notification to user
- Update user status in UI

### UserLoggedInEvent

Raised when a user logs in.

```csharp
public class UserLoggedInEvent : IDomainEvent
{
    public UserLoggedInEvent(UserAccountId id)
    {
        Id = id;
        OccurredOn = DateTime.UtcNow;
    }

    public UserAccountId Id { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `UserAccount.RecordLogin()`

**Handlers**:
- Create audit log entry
- Update last login timestamp
- Track login activity for security monitoring

### UserRoleEvents

Events related to user role assignments.

#### UserAssignedToRoleEvent

```csharp
public class UserAssignedToRoleEvent : IDomainEvent
{
    public UserAssignedToRoleEvent(
        UserAccountId userId,
        RoleId roleId)
    {
        UserId = userId;
        RoleId = roleId;
        OccurredOn = DateTime.UtcNow;
    }

    public UserAccountId UserId { get; }
    public RoleId RoleId { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `UserAccount.AssignToRole()`

**Handlers**:
- Create audit log entry
- Update user permissions cache
- Notify user of new role assignment

#### UserRemovedFromRoleEvent

```csharp
public class UserRemovedFromRoleEvent : IDomainEvent
{
    public UserRemovedFromRoleEvent(
        UserAccountId userId,
        RoleId roleId)
    {
        UserId = userId;
        RoleId = roleId;
        OccurredOn = DateTime.UtcNow;
    }

    public UserAccountId UserId { get; }
    public RoleId RoleId { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `UserAccount.RemoveFromRole()`

**Handlers**:
- Create audit log entry
- Update user permissions cache
- Notify user of role removal

## Role and Permission Events

### ApplicationRoleCreatedEvent

Raised when a new role is created.

```csharp
public class ApplicationRoleCreatedEvent : IDomainEvent
{
    public ApplicationRoleCreatedEvent(
        RoleId id,
        RoleName name)
    {
        Id = id;
        Name = name;
        OccurredOn = DateTime.UtcNow;
    }

    public RoleId Id { get; }
    public RoleName Name { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `ApplicationRole.Create()`

**Handlers**:
- Create audit log entry
- Update role cache
- Initialize role in UI

### ApplicationRoleNameChangedEvent

Raised when a role's name is changed.

```csharp
public class ApplicationRoleNameChangedEvent : IDomainEvent
{
    public ApplicationRoleNameChangedEvent(
        RoleId id,
        RoleName oldName,
        RoleName newName)
    {
        Id = id;
        OldName = oldName;
        NewName = newName;
        OccurredOn = DateTime.UtcNow;
    }

    public RoleId Id { get; }
    public RoleName OldName { get; }
    public RoleName NewName { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `ApplicationRole.UpdateName()`

**Handlers**:
- Create audit log entry
- Update role cache
- Update role name in UI

### ApplicationRoleDescriptionChangedEvent

Raised when a role's description is changed.

```csharp
public class ApplicationRoleDescriptionChangedEvent : IDomainEvent
{
    public ApplicationRoleDescriptionChangedEvent(
        RoleId id,
        Description newDescription)
    {
        Id = id;
        NewDescription = newDescription;
        OccurredOn = DateTime.UtcNow;
    }

    public RoleId Id { get; }
    public Description NewDescription { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `ApplicationRole.UpdateDescription()`

**Handlers**:
- Create audit log entry
- Update role cache
- Update role description in UI

### RolePermissionEvents

Events related to role permission assignments.

#### PermissionAddedToRoleEvent

```csharp
public class PermissionAddedToRoleEvent : IDomainEvent
{
    public PermissionAddedToRoleEvent(
        RoleId roleId,
        PermissionId permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
        OccurredOn = DateTime.UtcNow;
    }

    public RoleId RoleId { get; }
    public PermissionId PermissionId { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `ApplicationRole.AddPermission()`

**Handlers**:
- Create audit log entry
- Update permissions cache
- Update UI to reflect new permission

#### PermissionRemovedFromRoleEvent

```csharp
public class PermissionRemovedFromRoleEvent : IDomainEvent
{
    public PermissionRemovedFromRoleEvent(
        RoleId roleId,
        PermissionId permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
        OccurredOn = DateTime.UtcNow;
    }

    public RoleId RoleId { get; }
    public PermissionId PermissionId { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `ApplicationRole.RemovePermission()`

**Handlers**:
- Create audit log entry
- Update permissions cache
- Update UI to reflect removed permission

### PermissionEvents

Events related to permissions.

#### PermissionCreatedEvent

```csharp
public class PermissionCreatedEvent : IDomainEvent
{
    public PermissionCreatedEvent(
        PermissionId id,
        PermissionName name,
        PermissionType type,
        ResourceName resource)
    {
        Id = id;
        Name = name;
        Type = type;
        Resource = resource;
        OccurredOn = DateTime.UtcNow;
    }

    public PermissionId Id { get; }
    public PermissionName Name { get; }
    public PermissionType Type { get; }
    public ResourceName Resource { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `Permission.Create()`

**Handlers**:
- Create audit log entry
- Update permissions cache
- Initialize permission in UI

#### PermissionNameChangedEvent

```csharp
public class PermissionNameChangedEvent : IDomainEvent
{
    public PermissionNameChangedEvent(
        PermissionId id,
        PermissionName oldName,
        PermissionName newName)
    {
        Id = id;
        OldName = oldName;
        NewName = newName;
        OccurredOn = DateTime.UtcNow;
    }

    public PermissionId Id { get; }
    public PermissionName OldName { get; }
    public PermissionName NewName { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `Permission.UpdateName()`

**Handlers**:
- Create audit log entry
- Update permissions cache
- Update permission name in UI

#### PermissionDescriptionChangedEvent

```csharp
public class PermissionDescriptionChangedEvent : IDomainEvent
{
    public PermissionDescriptionChangedEvent(
        PermissionId id,
        Description newDescription)
    {
        Id = id;
        NewDescription = newDescription;
        OccurredOn = DateTime.UtcNow;
    }

    public PermissionId Id { get; }
    public Description NewDescription { get; }
    public DateTime OccurredOn { get; }
}
```

**Raised By**: `Permission.UpdateDescription()`

**Handlers**:
- Create audit log entry
- Update permissions cache
- Update permission description in UI

## Event Handling

### Event Dispatcher

The domain model uses an event dispatcher to publish domain events to their handlers:

```csharp
public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync(IEnumerable<IDomainEvent> domainEvents);
}

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task DispatchEventsAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }
    }
}
```

### Event Handlers

Event handlers are implemented using the mediator pattern:

```csharp
public class UserAccountCreatedEventHandler : INotificationHandler<UserAccountCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly IAuditLogService _auditLogService;

    public UserAccountCreatedEventHandler(
        IEmailService emailService,
        IAuditLogService auditLogService)
    {
        _emailService = emailService;
        _auditLogService = auditLogService;
    }

    public async Task Handle(UserAccountCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Send welcome email
        await _emailService.SendWelcomeEmailAsync(notification.Email);

        // Create audit log entry
        await _auditLogService.LogAsync(
            "UserCreated",
            $"User {notification.Username.Value} was created",
            notification.Id.Value.ToString());
    }
}
```

### Repository Implementation

Repositories are responsible for dispatching domain events after saving entities:

```csharp
public class UserAccountRepository : IUserAccountRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public UserAccountRepository(
        ApplicationDbContext dbContext,
        IDomainEventDispatcher eventDispatcher)
    {
        _dbContext = dbContext;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<UserAccount> GetByIdAsync(UserAccountId id)
    {
        return await _dbContext.UserAccounts
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task AddAsync(UserAccount user)
    {
        await _dbContext.UserAccounts.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        await _eventDispatcher.DispatchEventsAsync(user.DomainEvents);
        user.ClearDomainEvents();
    }

    public async Task UpdateAsync(UserAccount user)
    {
        _dbContext.UserAccounts.Update(user);
        await _dbContext.SaveChangesAsync();

        await _eventDispatcher.DispatchEventsAsync(user.DomainEvents);
        user.ClearDomainEvents();
    }
}
```

## Event Sourcing Considerations

While the current domain model does not use event sourcing, the domain events are designed to be compatible with event sourcing if needed in the future:

- Events contain all relevant data about what happened
- Events are immutable and serializable
- Events are named in past tense
- Entities raise events for all significant state changes

If event sourcing is implemented, the domain events would be stored as the primary source of truth, and entities would be reconstructed from their event history.
