# Domain Entities

This document details all entities in the domain model. Entities are objects that have an identity that runs through time and different states. They are the core building blocks of the domain model and encapsulate business logic and rules.

## Core Principles for Entities

1. **Identity**: Entities are defined by their identity, not their attributes
2. **Encapsulation**: Entities encapsulate state and behavior
3. **Invariants**: Entities enforce their own invariants and business rules
4. **Rich Behavior**: Entities contain business logic, not just data
5. **Factory Methods**: Entities are created through factory methods that ensure valid state

## Implementation Pattern

All entities follow this implementation pattern:

```csharp
public class [EntityName]
{
    private readonly List<IDomainEvent> _events = new();
    
    // Private constructor to enforce creation through factory methods
    private [EntityName]([ValueObjectId] id, /* other parameters */)
    {
        Id = id;
        // Initialize other properties
    }

    // Identity and properties
    public [ValueObjectId] Id { get; }
    public [ValueObject1] Property1 { get; private set; }
    public [ValueObject2] Property2 { get; private set; }
    // ...

    // Factory methods
    public static [EntityName] Create(/* parameters */)
    {
        // Validation logic
        // Create the entity
        var entity = new [EntityName](/* parameters */);
        
        // Add domain event
        entity._events.Add(new [EntityName]CreatedEvent(/* parameters */));
        
        return entity;
    }
    
    // Behavior methods
    public Result DoSomething(/* parameters */)
    {
        // Business logic
        // Validate preconditions
        // Update state
        // Add domain event
        
        return Result.Success();
    }
    
    // Domain events access
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _events.AsReadOnly();
    public void ClearDomainEvents() => _events.Clear();
}
```

## User Management Entities

### UserAccount

The UserAccount entity represents a user in the system, independent of the authentication provider. It is the core entity for user management and serves as the aggregate root for user-related entities.

```csharp
public class UserAccount
{
    private readonly List<IDomainEvent> _events = new();
    private readonly List<UserRole> _roles = new();
    
    private UserAccount(
        UserAccountId id,
        Username username,
        Email email,
        PersonName name,
        UserStatus status,
        AuthenticationProvider authProvider)
    {
        Id = id;
        Username = username;
        Email = email;
        Name = name;
        Status = status;
        AuthenticationProvider = authProvider;
        CreatedAt = CreatedAt.Now();
    }

    // Identity and properties
    public UserAccountId Id { get; }
    public Username Username { get; }
    public Email Email { get; private set; }
    public PersonName Name { get; private set; }
    public UserStatus Status { get; private set; }
    public CreatedAt CreatedAt { get; }
    public DateTime? LastLoggedIn { get; private set; }
    public AuthenticationProvider AuthenticationProvider { get; }
    
    // Roles collection
    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

    // Factory methods
    public static UserAccount Create(
        Username username,
        Email email,
        PersonName name,
        AuthenticationProvider authProvider)
    {
        var id = UserAccountId.CreateUnique();
        var status = UserStatus.Active;
        
        var user = new UserAccount(id, username, email, name, status, authProvider);
        
        // Add domain event
        user._events.Add(new UserAccountCreatedEvent(user.Id, username, email, authProvider));
        
        return user;
    }
    
    // Behavior methods
    public Result UpdateEmail(Email newEmail)
    {
        // Business logic and validation
        var oldEmail = Email;
        Email = newEmail;
        
        // Add domain event
        _events.Add(new UserEmailChangedEvent(Id, oldEmail, newEmail));
        
        return Result.Success();
    }
    
    public Result UpdateName(PersonName newName)
    {
        Name = newName;
        
        _events.Add(new UserNameChangedEvent(Id, newName));
        
        return Result.Success();
    }
    
    public Result Deactivate()
    {
        if (Status.IsInactive)
            return Result.Failure("User is already inactive");
            
        Status = UserStatus.Inactive;
        
        _events.Add(new UserDeactivatedEvent(Id));
        
        return Result.Success();
    }
    
    public Result Activate()
    {
        if (Status.IsActive)
            return Result.Failure("User is already active");
            
        Status = UserStatus.Active;
        
        _events.Add(new UserActivatedEvent(Id));
        
        return Result.Success();
    }
    
    public Result Lock()
    {
        if (Status.IsLocked)
            return Result.Failure("User is already locked");
            
        Status = UserStatus.Locked;
        
        _events.Add(new UserLockedEvent(Id));
        
        return Result.Success();
    }
    
    public Result Unlock()
    {
        if (!Status.IsLocked)
            return Result.Failure("User is not locked");
            
        Status = UserStatus.Active;
        
        _events.Add(new UserUnlockedEvent(Id));
        
        return Result.Success();
    }
    
    public Result RecordLogin()
    {
        LastLoggedIn = DateTime.UtcNow;
        
        _events.Add(new UserLoggedInEvent(Id));
        
        return Result.Success();
    }
    
    public Result AssignToRole(ApplicationRole role)
    {
        if (_roles.Any(r => r.RoleId == role.Id))
            return Result.Failure("User is already assigned to this role");
            
        var userRole = UserRole.Create(Id, role.Id);
        _roles.Add(userRole);
        
        _events.Add(new UserAssignedToRoleEvent(Id, role.Id));
        
        return Result.Success();
    }
    
    public Result RemoveFromRole(RoleId roleId)
    {
        var userRole = _roles.FirstOrDefault(r => r.RoleId == roleId);
        
        if (userRole == null)
            return Result.Failure("User is not assigned to this role");
            
        _roles.Remove(userRole);
        
        _events.Add(new UserRemovedFromRoleEvent(Id, roleId));
        
        return Result.Success();
    }
    
    // Domain events access
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _events.AsReadOnly();
    public void ClearDomainEvents() => _events.Clear();
}
```

**Key Responsibilities**:
- Manage user identity and profile information
- Control user status (active, inactive, locked)
- Track user authentication and login activity
- Manage role assignments

**Business Rules**:
- Username and email must be unique (enforced at repository level)
- Users can be assigned to multiple roles
- User status transitions follow specific rules (e.g., locked users must be unlocked before activation)

### UserRole

The UserRole entity represents the assignment of a role to a user. It is a join entity for the many-to-many relationship between UserAccount and ApplicationRole.

```csharp
public class UserRole
{
    private UserRole(UserAccountId userId, RoleId roleId)
    {
        UserId = userId;
        RoleId = roleId;
        AssignedAt = DateTime.UtcNow;
    }

    public UserAccountId UserId { get; }
    public RoleId RoleId { get; }
    public DateTime AssignedAt { get; }

    public static UserRole Create(UserAccountId userId, RoleId roleId)
    {
        return new UserRole(userId, roleId);
    }
}
```

**Key Responsibilities**:
- Connect users to roles
- Track when the role was assigned

**Business Rules**:
- A user can only be assigned to a specific role once

## Role and Permission Entities

### ApplicationRole

The ApplicationRole entity represents a role in the system. Roles are collections of permissions that can be assigned to users.

```csharp
public class ApplicationRole
{
    private readonly List<IDomainEvent> _events = new();
    private readonly List<RolePermission> _permissions = new();
    
    private ApplicationRole(
        RoleId id,
        RoleName name,
        Description description)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAt = CreatedAt.Now();
    }

    public RoleId Id { get; }
    public RoleName Name { get; private set; }
    public Description Description { get; private set; }
    public CreatedAt CreatedAt { get; }
    
    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    public static ApplicationRole Create(
        RoleName name,
        Description description)
    {
        var id = RoleId.CreateUnique();
        
        var role = new ApplicationRole(id, name, description);
        
        role._events.Add(new ApplicationRoleCreatedEvent(role.Id, name));
        
        return role;
    }
    
    public Result UpdateName(RoleName newName)
    {
        var oldName = Name;
        Name = newName;
        
        _events.Add(new ApplicationRoleNameChangedEvent(Id, oldName, newName));
        
        return Result.Success();
    }
    
    public Result UpdateDescription(Description newDescription)
    {
        Description = newDescription;
        
        _events.Add(new ApplicationRoleDescriptionChangedEvent(Id, newDescription));
        
        return Result.Success();
    }
    
    public Result AddPermission(Permission permission)
    {
        if (_permissions.Any(p => p.PermissionId == permission.Id))
            return Result.Failure("Permission is already assigned to this role");
            
        var rolePermission = RolePermission.Create(Id, permission.Id);
        _permissions.Add(rolePermission);
        
        _events.Add(new PermissionAddedToRoleEvent(Id, permission.Id));
        
        return Result.Success();
    }
    
    public Result RemovePermission(PermissionId permissionId)
    {
        var rolePermission = _permissions.FirstOrDefault(p => p.PermissionId == permissionId);
        
        if (rolePermission == null)
            return Result.Failure("Permission is not assigned to this role");
            
        _permissions.Remove(rolePermission);
        
        _events.Add(new PermissionRemovedFromRoleEvent(Id, permissionId));
        
        return Result.Success();
    }
    
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _events.AsReadOnly();
    public void ClearDomainEvents() => _events.Clear();
}
```

**Key Responsibilities**:
- Define roles in the system
- Manage permissions assigned to roles
- Track role creation and changes

**Business Rules**:
- Role names must be unique (enforced at repository level)
- A permission can only be assigned to a role once

### Permission

The Permission entity represents a permission in the system. Permissions define what actions users can perform on specific resources.

```csharp
public class Permission
{
    private readonly List<IDomainEvent> _events = new();
    
    private Permission(
        PermissionId id,
        PermissionName name,
        Description description,
        PermissionType type,
        ResourceName resource,
        bool isSystemPermission)
    {
        Id = id;
        Name = name;
        Description = description;
        Type = type;
        Resource = resource;
        IsSystemPermission = isSystemPermission;
    }

    public PermissionId Id { get; }
    public PermissionName Name { get; private set; }
    public Description Description { get; private set; }
    public PermissionType Type { get; }
    public ResourceName Resource { get; }
    public bool IsSystemPermission { get; }

    public static Permission Create(
        PermissionName name,
        Description description,
        PermissionType type,
        ResourceName resource,
        bool isSystemPermission = false)
    {
        var id = PermissionId.CreateUnique();
        
        var permission = new Permission(id, name, description, type, resource, isSystemPermission);
        
        permission._events.Add(new PermissionCreatedEvent(permission.Id, name, type, resource));
        
        return permission;
    }
    
    public Result UpdateName(PermissionName newName)
    {
        if (IsSystemPermission)
            return Result.Failure("Cannot modify system permission");
            
        var oldName = Name;
        Name = newName;
        
        _events.Add(new PermissionNameChangedEvent(Id, oldName, newName));
        
        return Result.Success();
    }
    
    public Result UpdateDescription(Description newDescription)
    {
        if (IsSystemPermission)
            return Result.Failure("Cannot modify system permission");
            
        Description = newDescription;
        
        _events.Add(new PermissionDescriptionChangedEvent(Id, newDescription));
        
        return Result.Success();
    }
    
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _events.AsReadOnly();
    public void ClearDomainEvents() => _events.Clear();
}
```

**Key Responsibilities**:
- Define permissions for specific actions on resources
- Distinguish between system and custom permissions
- Track permission creation and changes

**Business Rules**:
- System permissions cannot be modified
- Permission names must be unique for a given resource and type (enforced at repository level)

### RolePermission

The RolePermission entity represents the assignment of a permission to a role. It is a join entity for the many-to-many relationship between ApplicationRole and Permission.

```csharp
public class RolePermission
{
    private RolePermission(RoleId roleId, PermissionId permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
        AssignedAt = DateTime.UtcNow;
    }

    public RoleId RoleId { get; }
    public PermissionId PermissionId { get; }
    public DateTime AssignedAt { get; }

    public static RolePermission Create(RoleId roleId, PermissionId permissionId)
    {
        return new RolePermission(roleId, permissionId);
    }
}
```

**Key Responsibilities**:
- Connect roles to permissions
- Track when the permission was assigned to the role

**Business Rules**:
- A permission can only be assigned to a specific role once

## Common Patterns and Considerations

### Entity Creation

All entities are created through factory methods that:
- Validate inputs
- Create a valid entity instance
- Raise appropriate domain events
- Return the new entity

### State Changes

State changes in entities:
- Are performed through methods that enforce business rules
- Validate preconditions before changing state
- Raise appropriate domain events
- Return a Result object indicating success or failure

### Collections

Entities that contain collections:
- Expose collections as IReadOnlyCollection to prevent external modification
- Provide methods to add and remove items from collections
- Enforce business rules when modifying collections
- Raise appropriate domain events when collections change

### Domain Events

All entities:
- Maintain a private list of domain events
- Expose domain events as IReadOnlyCollection
- Provide a method to clear domain events after they are processed
- Raise domain events when significant state changes occur
