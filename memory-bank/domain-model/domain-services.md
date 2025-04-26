# Domain Services

This document details the domain services in the domain model. Domain services encapsulate domain logic that doesn't naturally fit within a single entity or value object. They represent operations, calculations, or business processes that involve multiple domain objects.

## Core Principles for Domain Services

1. **Stateless**: Domain services are stateless; they don't maintain any state between method calls
2. **Domain Logic**: Domain services contain domain logic, not infrastructure or application concerns
3. **Named After Activities**: Domain services are named after activities or operations, not entities
4. **Multiple Entities**: Domain services typically operate on multiple entities or aggregates
5. **Part of Domain Layer**: Domain services belong to the domain layer, not the application layer

## When to Use Domain Services

Domain services should be used when:

1. The operation involves multiple aggregates
2. The operation doesn't conceptually belong to any single entity
3. The operation represents a significant domain concept or process
4. The operation enforces domain rules that span multiple entities

## Implementation Pattern

Domain services follow this implementation pattern:

```csharp
public interface I[ServiceName]
{
    Task<Result> [OperationName]Async([Parameters]);
}

public class [ServiceName] : I[ServiceName]
{
    // Dependencies
    
    public [ServiceName]([Dependencies])
    {
        // Initialize dependencies
    }
    
    public async Task<Result> [OperationName]Async([Parameters])
    {
        // Domain logic
        return Result.Success();
    }
}
```

## User Management Domain Services

### IUserUniquenessChecker

This domain service checks if a username or email is unique in the system.

```csharp
public interface IUserUniquenessChecker
{
    Task<bool> IsUsernameUniqueAsync(Username username, UserAccountId? excludeUserId = null);
    Task<bool> IsEmailUniqueAsync(Email email, UserAccountId? excludeUserId = null);
}

public class UserUniquenessChecker : IUserUniquenessChecker
{
    private readonly IUserAccountRepository _userRepository;
    
    public UserUniquenessChecker(IUserAccountRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<bool> IsUsernameUniqueAsync(Username username, UserAccountId? excludeUserId = null)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(username);
        
        if (existingUser == null)
            return true;
            
        if (excludeUserId != null && existingUser.Id == excludeUserId)
            return true;
            
        return false;
    }
    
    public async Task<bool> IsEmailUniqueAsync(Email email, UserAccountId? excludeUserId = null)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email);
        
        if (existingUser == null)
            return true;
            
        if (excludeUserId != null && existingUser.Id == excludeUserId)
            return true;
            
        return false;
    }
}
```

**Responsibilities**:
- Check if a username is unique in the system
- Check if an email is unique in the system
- Support excluding a specific user from the uniqueness check (for updates)

**Used By**:
- Application services when creating or updating users
- Domain entities for validation during creation or updates

### IPasswordHashingService

This domain service handles password hashing and verification.

```csharp
public interface IPasswordHashingService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public class PasswordHashingService : IPasswordHashingService
{
    public string HashPassword(string password)
    {
        // Use a secure hashing algorithm (e.g., BCrypt, Argon2)
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    
    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
```

**Responsibilities**:
- Hash passwords securely
- Verify passwords against stored hashes

**Used By**:
- Authentication services
- User registration and password change operations

### IUserAuthorizationService

This domain service checks if a user has specific permissions.

```csharp
public interface IUserAuthorizationService
{
    Task<bool> HasPermissionAsync(UserAccountId userId, PermissionType permissionType, ResourceName resource);
    Task<IReadOnlyCollection<Permission>> GetUserPermissionsAsync(UserAccountId userId);
}

public class UserAuthorizationService : IUserAuthorizationService
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IApplicationRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    
    public UserAuthorizationService(
        IUserAccountRepository userRepository,
        IApplicationRoleRepository roleRepository,
        IPermissionRepository permissionRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }
    
    public async Task<bool> HasPermissionAsync(UserAccountId userId, PermissionType permissionType, ResourceName resource)
    {
        // Get user with roles
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return false;
            
        // Get all role IDs for the user
        var roleIds = user.Roles.Select(r => r.RoleId).ToList();
        
        // Get all roles with their permissions
        var roles = await _roleRepository.GetByIdsAsync(roleIds);
        
        // Check if any role has the required permission
        foreach (var role in roles)
        {
            foreach (var rolePermission in role.Permissions)
            {
                var permission = await _permissionRepository.GetByIdAsync(rolePermission.PermissionId);
                
                if (permission != null && 
                    permission.Type == permissionType && 
                    (permission.Resource == resource || permission.Resource.IsGlobal))
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    public async Task<IReadOnlyCollection<Permission>> GetUserPermissionsAsync(UserAccountId userId)
    {
        // Get user with roles
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return new List<Permission>();
            
        // Get all role IDs for the user
        var roleIds = user.Roles.Select(r => r.RoleId).ToList();
        
        // Get all roles with their permissions
        var roles = await _roleRepository.GetByIdsAsync(roleIds);
        
        // Collect all permission IDs
        var permissionIds = new HashSet<PermissionId>();
        foreach (var role in roles)
        {
            foreach (var rolePermission in role.Permissions)
            {
                permissionIds.Add(rolePermission.PermissionId);
            }
        }
        
        // Get all permissions
        var permissions = await _permissionRepository.GetByIdsAsync(permissionIds.ToList());
        
        return permissions;
    }
}
```

**Responsibilities**:
- Check if a user has a specific permission for a resource
- Get all permissions for a user across all their roles

**Used By**:
- Application services for authorization checks
- API controllers for securing endpoints
- UI components for showing/hiding features based on permissions

## Role and Permission Domain Services

### IRolePermissionService

This domain service manages the assignment of permissions to roles.

```csharp
public interface IRolePermissionService
{
    Task<Result> AssignPermissionToRoleAsync(RoleId roleId, PermissionId permissionId);
    Task<Result> RemovePermissionFromRoleAsync(RoleId roleId, PermissionId permissionId);
    Task<IReadOnlyCollection<Permission>> GetPermissionsForRoleAsync(RoleId roleId);
}

public class RolePermissionService : IRolePermissionService
{
    private readonly IApplicationRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public RolePermissionService(
        IApplicationRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
    }
    
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
    
    public async Task<Result> RemovePermissionFromRoleAsync(RoleId roleId, PermissionId permissionId)
    {
        // Load aggregate
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            return Result.Failure("Role not found");
            
        // Perform operation on ApplicationRole aggregate
        var result = role.RemovePermission(permissionId);
        if (result.IsFailure)
            return result;
            
        // Save changes
        await _roleRepository.UpdateAsync(role);
        await _unitOfWork.CommitAsync();
        
        return Result.Success();
    }
    
    public async Task<IReadOnlyCollection<Permission>> GetPermissionsForRoleAsync(RoleId roleId)
    {
        // Load aggregate
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            return new List<Permission>();
            
        // Get permission IDs
        var permissionIds = role.Permissions.Select(p => p.PermissionId).ToList();
        
        // Get permissions
        var permissions = await _permissionRepository.GetByIdsAsync(permissionIds);
        
        return permissions;
    }
}
```

**Responsibilities**:
- Assign permissions to roles
- Remove permissions from roles
- Get all permissions for a role

**Used By**:
- Role management UI
- Application services for role configuration

### IPermissionManagementService

This domain service manages the creation and organization of permissions.

```csharp
public interface IPermissionManagementService
{
    Task<Result<Permission>> CreatePermissionAsync(
        PermissionName name,
        Description description,
        PermissionType type,
        ResourceName resource,
        bool isSystemPermission = false);
    
    Task<Result> UpdatePermissionAsync(
        PermissionId id,
        PermissionName name,
        Description description);
    
    Task<IReadOnlyCollection<Permission>> GetPermissionsByResourceAsync(ResourceName resource);
    Task<IReadOnlyCollection<Permission>> GetSystemPermissionsAsync();
}

public class PermissionManagementService : IPermissionManagementService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public PermissionManagementService(
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork)
    {
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Permission>> CreatePermissionAsync(
        PermissionName name,
        Description description,
        PermissionType type,
        ResourceName resource,
        bool isSystemPermission = false)
    {
        // Check if permission already exists
        var existingPermission = await _permissionRepository.GetByNameTypeAndResourceAsync(name, type, resource);
        if (existingPermission != null)
            return Result.Failure<Permission>("Permission already exists");
            
        // Create permission
        var permission = Permission.Create(name, description, type, resource, isSystemPermission);
        
        // Save permission
        await _permissionRepository.AddAsync(permission);
        await _unitOfWork.CommitAsync();
        
        return Result.Success(permission);
    }
    
    public async Task<Result> UpdatePermissionAsync(
        PermissionId id,
        PermissionName name,
        Description description)
    {
        // Load permission
        var permission = await _permissionRepository.GetByIdAsync(id);
        if (permission == null)
            return Result.Failure("Permission not found");
            
        // Check if permission with new name already exists
        if (!permission.Name.Equals(name))
        {
            var existingPermission = await _permissionRepository.GetByNameTypeAndResourceAsync(
                name, permission.Type, permission.Resource);
                
            if (existingPermission != null && existingPermission.Id != permission.Id)
                return Result.Failure("Permission with this name already exists");
        }
        
        // Update permission
        var nameResult = permission.UpdateName(name);
        if (nameResult.IsFailure)
            return nameResult;
            
        var descriptionResult = permission.UpdateDescription(description);
        if (descriptionResult.IsFailure)
            return descriptionResult;
            
        // Save changes
        await _permissionRepository.UpdateAsync(permission);
        await _unitOfWork.CommitAsync();
        
        return Result.Success();
    }
    
    public async Task<IReadOnlyCollection<Permission>> GetPermissionsByResourceAsync(ResourceName resource)
    {
        return await _permissionRepository.GetByResourceAsync(resource);
    }
    
    public async Task<IReadOnlyCollection<Permission>> GetSystemPermissionsAsync()
    {
        return await _permissionRepository.GetSystemPermissionsAsync();
    }
}
```

**Responsibilities**:
- Create and update permissions
- Ensure permission uniqueness for a given resource and type
- Retrieve permissions by resource or system status

**Used By**:
- Permission management UI
- System initialization for creating default permissions

## User Role Management Domain Services

### IUserRoleService

This domain service manages the assignment of roles to users.

```csharp
public interface IUserRoleService
{
    Task<Result> AssignRoleToUserAsync(UserAccountId userId, RoleId roleId);
    Task<Result> RemoveRoleFromUserAsync(UserAccountId userId, RoleId roleId);
    Task<IReadOnlyCollection<ApplicationRole>> GetRolesForUserAsync(UserAccountId userId);
    Task<IReadOnlyCollection<UserAccount>> GetUsersInRoleAsync(RoleId roleId);
}

public class UserRoleService : IUserRoleService
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IApplicationRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UserRoleService(
        IUserAccountRepository userRepository,
        IApplicationRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }
    
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
    
    public async Task<Result> RemoveRoleFromUserAsync(UserAccountId userId, RoleId roleId)
    {
        // Load aggregate
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Result.Failure("User not found");
            
        // Perform operation on UserAccount aggregate
        var result = user.RemoveFromRole(roleId);
        if (result.IsFailure)
            return result;
            
        // Save changes
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.CommitAsync();
        
        return Result.Success();
    }
    
    public async Task<IReadOnlyCollection<ApplicationRole>> GetRolesForUserAsync(UserAccountId userId)
    {
        // Load user with roles
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return new List<ApplicationRole>();
            
        // Get role IDs
        var roleIds = user.Roles.Select(r => r.RoleId).ToList();
        
        // Get roles
        var roles = await _roleRepository.GetByIdsAsync(roleIds);
        
        return roles;
    }
    
    public async Task<IReadOnlyCollection<UserAccount>> GetUsersInRoleAsync(RoleId roleId)
    {
        return await _userRepository.GetUsersByRoleIdAsync(roleId);
    }
}
```

**Responsibilities**:
- Assign roles to users
- Remove roles from users
- Get all roles for a user
- Get all users in a role

**Used By**:
- User management UI
- Role management UI
- Application services for user and role management

## Authentication Provider Domain Services

### IAuthenticationProviderFactory

This domain service creates the appropriate authentication provider based on configuration.

```csharp
public interface IAuthenticationProviderFactory
{
    IAuthenticationProvider CreateProvider(AuthenticationProvider provider);
}

public class AuthenticationProviderFactory : IAuthenticationProviderFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public AuthenticationProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IAuthenticationProvider CreateProvider(AuthenticationProvider provider)
    {
        return provider.Value switch
        {
            "EFCoreIdentity" => _serviceProvider.GetRequiredService<IEFCoreIdentityProvider>(),
            "MicrosoftEntraID" => _serviceProvider.GetRequiredService<IMicrosoftEntraIDProvider>(),
            _ => throw new ArgumentException($"Unsupported authentication provider: {provider.Value}")
        };
    }
}
```

**Responsibilities**:
- Create the appropriate authentication provider based on configuration
- Provide a unified interface for authentication operations

**Used By**:
- Application services for authentication
- User management operations

### ICurrentUserService

This domain service provides access to the current authenticated user.

```csharp
public interface ICurrentUserService
{
    UserAccountId? GetCurrentUserId();
    Task<UserAccount?> GetCurrentUserAsync();
    Task<bool> HasPermissionAsync(PermissionType permissionType, ResourceName resource);
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserAccountRepository _userRepository;
    private readonly IUserAuthorizationService _authorizationService;
    
    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        IUserAccountRepository userRepository,
        IUserAuthorizationService authorizationService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _authorizationService = authorizationService;
    }
    
    public UserAccountId? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return null;
            
        return UserAccountId.Create(userId);
    }
    
    public async Task<UserAccount?> GetCurrentUserAsync()
    {
        var userId = GetCurrentUserId();
        
        if (userId == null)
            return null;
            
        return await _userRepository.GetByIdAsync(userId);
    }
    
    public async Task<bool> HasPermissionAsync(PermissionType permissionType, ResourceName resource)
    {
        var userId = GetCurrentUserId();
        
        if (userId == null)
            return false;
            
        return await _authorizationService.HasPermissionAsync(userId, permissionType, resource);
    }
}
```

**Responsibilities**:
- Get the current authenticated user's ID
- Get the current authenticated user
- Check if the current user has a specific permission

**Used By**:
- Application services for authorization checks
- Controllers for securing endpoints
- Domain entities for auditing (CreatedBy, ModifiedBy)

## Domain Service Composition

Domain services can be composed to implement more complex domain operations. For example, a user registration service might use multiple domain services:

```csharp
public interface IUserRegistrationService
{
    Task<Result<UserAccount>> RegisterUserAsync(
        Username username,
        Email email,
        string password,
        PersonName name,
        AuthenticationProvider authProvider);
}

public class UserRegistrationService : IUserRegistrationService
{
    private readonly IUserUniquenessChecker _uniquenessChecker;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IUserAccountRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UserRegistrationService(
        IUserUniquenessChecker uniquenessChecker,
        IPasswordHashingService passwordHashingService,
        IUserAccountRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _uniquenessChecker = uniquenessChecker;
        _passwordHashingService = passwordHashingService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<UserAccount>> RegisterUserAsync(
        Username username,
        Email email,
        string password,
        PersonName name,
        AuthenticationProvider authProvider)
    {
        // Check uniqueness
        if (!await _uniquenessChecker.IsUsernameUniqueAsync(username))
            return Result.Failure<UserAccount>("Username is already taken");
            
        if (!await _uniquenessChecker.IsEmailUniqueAsync(email))
            return Result.Failure<UserAccount>("Email is already in use");
            
        // Hash password (for EF Core Identity)
        var passwordHash = _passwordHashingService.HashPassword(password);
        
        // Create user
        var user = UserAccount.Create(username, email, name, authProvider);
        
        // Save user
        await _userRepository.AddAsync(user);
        await _unitOfWork.CommitAsync();
        
        return Result.Success(user);
    }
}
```

This composition allows for complex domain operations while keeping individual domain services focused and maintainable.
