# Domain Aggregates

This document details the aggregate boundaries in the domain model. Aggregates are clusters of domain objects that are treated as a single unit for data changes. Each aggregate has a root entity that controls access to the aggregate's members.

## Aggregate Design Principles

1. **Transactional Consistency**: Aggregates define the boundaries of transactional consistency
2. **References Between Aggregates**: References across aggregate boundaries are by ID only
3. **Invariant Protection**: Aggregates protect their invariants and enforce business rules
4. **Size Consideration**: Aggregates should be as small as possible while still protecting invariants
5. **Root Control**: All access to aggregate members goes through the aggregate root

## User Management Aggregate

### UserAccount Aggregate

The UserAccount aggregate represents a user in the system and their role assignments.

#### Aggregate Root
- **UserAccount**: The root entity that controls access to the aggregate

#### Aggregate Members
- **UserRole**: Represents the assignment of a role to the user

#### Aggregate Boundaries

```
┌─────────────────────────────────────────┐
│ UserAccount Aggregate                    │
│                                         │
│  ┌───────────────┐      ┌────────────┐  │
│  │  UserAccount  │──1:*─┤  UserRole  │  │
│  └───────────────┘      └────────────┘  │
│                                         │
└─────────────────────────────────────────┘
```

#### Invariants and Business Rules
- A user can only be assigned to a specific role once
- User status transitions follow specific rules (e.g., locked users must be unlocked before activation)
- Username and email must be unique (enforced at repository level)

#### External References
- **ApplicationRole**: Referenced by ID through UserRole
- **AuthenticationProvider**: Value object representing the authentication provider

#### Transaction Boundaries
- Creating a user
- Updating user information
- Changing user status
- Assigning or removing roles

#### Implementation Considerations

The UserAccount aggregate root controls all access to its UserRole members:

```csharp
// Inside UserAccount class

// Adding a role
public Result AssignToRole(ApplicationRole role)
{
    if (_roles.Any(r => r.RoleId == role.Id))
        return Result.Failure("User is already assigned to this role");
        
    var userRole = UserRole.Create(Id, role.Id);
    _roles.Add(userRole);
    
    _events.Add(new UserAssignedToRoleEvent(Id, role.Id));
    
    return Result.Success();
}

// Removing a role
public Result RemoveFromRole(RoleId roleId)
{
    var userRole = _roles.FirstOrDefault(r => r.RoleId == roleId);
    
    if (userRole == null)
        return Result.Failure("User is not assigned to this role");
        
    _roles.Remove(userRole);
    
    _events.Add(new UserRemovedFromRoleEvent(Id, roleId));
    
    return Result.Success();
}
```

## Role and Permission Aggregate

### ApplicationRole Aggregate

The ApplicationRole aggregate represents a role in the system and its permission assignments.

#### Aggregate Root
- **ApplicationRole**: The root entity that controls access to the aggregate

#### Aggregate Members
- **RolePermission**: Represents the assignment of a permission to the role

#### Aggregate Boundaries

```
┌─────────────────────────────────────────────────┐
│ ApplicationRole Aggregate                        │
│                                                 │
│  ┌─────────────────┐      ┌────────────────┐    │
│  │ ApplicationRole │──1:*─┤ RolePermission │    │
│  └─────────────────┘      └────────────────┘    │
│                                                 │
└─────────────────────────────────────────────────┘
```

#### Invariants and Business Rules
- A permission can only be assigned to a specific role once
- Role names must be unique (enforced at repository level)

#### External References
- **Permission**: Referenced by ID through RolePermission

#### Transaction Boundaries
- Creating a role
- Updating role information
- Adding or removing permissions

#### Implementation Considerations

The ApplicationRole aggregate root controls all access to its RolePermission members:

```csharp
// Inside ApplicationRole class

// Adding a permission
public Result AddPermission(Permission permission)
{
    if (_permissions.Any(p => p.PermissionId == permission.Id))
        return Result.Failure("Permission is already assigned to this role");
        
    var rolePermission = RolePermission.Create(Id, permission.Id);
    _permissions.Add(rolePermission);
    
    _events.Add(new PermissionAddedToRoleEvent(Id, permission.Id));
    
    return Result.Success();
}

// Removing a permission
public Result RemovePermission(PermissionId permissionId)
{
    var rolePermission = _permissions.FirstOrDefault(p => p.PermissionId == permissionId);
    
    if (rolePermission == null)
        return Result.Failure("Permission is not assigned to this role");
        
    _permissions.Remove(rolePermission);
    
    _events.Add(new PermissionRemovedFromRoleEvent(Id, permissionId));
    
    return Result.Success();
}
```

## Permission Aggregate

### Permission Aggregate

The Permission aggregate represents a permission in the system.

#### Aggregate Root
- **Permission**: The root entity that controls access to the aggregate

#### Aggregate Members
- None (single entity aggregate)

#### Aggregate Boundaries

```
┌─────────────────────┐
│ Permission Aggregate │
│                     │
│   ┌────────────┐    │
│   │ Permission │    │
│   └────────────┘    │
│                     │
└─────────────────────┘
```

#### Invariants and Business Rules
- System permissions cannot be modified
- Permission names must be unique for a given resource and type (enforced at repository level)

#### External References
- None

#### Transaction Boundaries
- Creating a permission
- Updating permission information (for non-system permissions)

#### Implementation Considerations

The Permission entity enforces its own invariants:

```csharp
// Inside Permission class

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
```

## Cross-Aggregate Operations

### User Role Assignment

When assigning a role to a user, the operation spans two aggregates:

1. The UserAccount aggregate (to add the role assignment)
2. The ApplicationRole aggregate (to verify the role exists)

This operation should be implemented in an application service:

```csharp
public class UserRoleService
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IApplicationRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    // Constructor with dependencies
    
    public async Task<Result> AssignRoleToUserAsync(UserAccountId userId, RoleId roleId)
    {
        // Load aggregates
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Result.Failure("User not found");
            
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            return Result.Failure("Role not found");
            
        // Perform operation on UserAccount aggregate
        var result = user.AssignToRole(role);
        if (result.IsFailure)
            return result;
            
        // Save changes
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.CommitAsync();
        
        return Result.Success();
    }
}
```

### Permission Assignment to Role

When assigning a permission to a role, the operation spans two aggregates:

1. The ApplicationRole aggregate (to add the permission assignment)
2. The Permission aggregate (to verify the permission exists)

This operation should be implemented in an application service:

```csharp
public class RolePermissionService
{
    private readonly IApplicationRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    // Constructor with dependencies
    
    public async Task<Result> AssignPermissionToRoleAsync(RoleId roleId, PermissionId permissionId)
    {
        // Load aggregates
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            return Result.Failure("Role not found");
            
        var permission = await _permissionRepository.GetByIdAsync(permissionId);
        if (permission == null)
            return Result.Failure("Permission not found");
            
        // Perform operation on ApplicationRole aggregate
        var result = role.AddPermission(permission);
        if (result.IsFailure)
            return result;
            
        // Save changes
        await _roleRepository.UpdateAsync(role);
        await _unitOfWork.CommitAsync();
        
        return Result.Success();
    }
}
```

## Aggregate Loading and Persistence

### Loading Aggregates

When loading aggregates, all members of the aggregate should be loaded together to ensure consistency:

```csharp
// UserAccount repository implementation
public async Task<UserAccount> GetByIdAsync(UserAccountId id)
{
    // Load UserAccount with its UserRoles
    var user = await _dbContext.UserAccounts
        .Include(u => u.Roles)
        .FirstOrDefaultAsync(u => u.Id == id);
        
    return user;
}

// ApplicationRole repository implementation
public async Task<ApplicationRole> GetByIdAsync(RoleId id)
{
    // Load ApplicationRole with its RolePermissions
    var role = await _dbContext.ApplicationRoles
        .Include(r => r.Permissions)
        .FirstOrDefaultAsync(r => r.Id == id);
        
    return role;
}
```

### Persisting Aggregates

When persisting aggregates, all changes to the aggregate should be saved in a single transaction:

```csharp
// UserAccount repository implementation
public async Task UpdateAsync(UserAccount user)
{
    // Update UserAccount and its UserRoles
    _dbContext.UserAccounts.Update(user);
    
    // Process domain events
    foreach (var domainEvent in user.DomainEvents)
    {
        await _mediator.Publish(domainEvent);
    }
    
    user.ClearDomainEvents();
}

// ApplicationRole repository implementation
public async Task UpdateAsync(ApplicationRole role)
{
    // Update ApplicationRole and its RolePermissions
    _dbContext.ApplicationRoles.Update(role);
    
    // Process domain events
    foreach (var domainEvent in role.DomainEvents)
    {
        await _mediator.Publish(domainEvent);
    }
    
    role.ClearDomainEvents();
}
```

## Consistency Considerations

### Eventual Consistency

Some operations that span multiple aggregates may require eventual consistency:

1. **User Authorization Check**: When checking if a user has a specific permission, the system needs to:
   - Load the UserAccount aggregate to get the user's roles
   - Load the ApplicationRole aggregates to get the permissions for each role
   - This operation can be optimized with a denormalized view or cache

2. **Role Deletion**: When deleting a role, the system needs to:
   - Remove the role from all users that have it assigned
   - Delete the role itself
   - This may require a background job to maintain eventual consistency

### Consistency Boundaries

The aggregate boundaries define the consistency boundaries in the system:

- **Strong Consistency**: Within an aggregate, all operations are strongly consistent
- **Eventual Consistency**: Between aggregates, operations may be eventually consistent

## Aggregate Size Considerations

The aggregates in this domain model are designed to be small and focused:

- **UserAccount Aggregate**: Contains only the UserAccount and its UserRoles
- **ApplicationRole Aggregate**: Contains only the ApplicationRole and its RolePermissions
- **Permission Aggregate**: Contains only the Permission entity

This design ensures that aggregates are small enough to be loaded and persisted efficiently, while still protecting their invariants.
