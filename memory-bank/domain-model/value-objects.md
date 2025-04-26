# Domain Value Objects

This document details all value objects in the domain model. Value objects are immutable objects that represent concepts in the domain that have no identity. They are defined by their attributes and are used to replace primitive types with more meaningful domain concepts.

## Core Principles for Value Objects

1. **Immutability**: Value objects are immutable; once created, they cannot be changed
2. **Value Equality**: Two value objects with the same attributes are considered equal
3. **Self-Validation**: Value objects validate their own state during creation
4. **No Identity**: Value objects are defined by their attributes, not by an identity
5. **Encapsulated Behavior**: Value objects can contain domain logic related to the concept they represent

## Implementation Pattern

All value objects follow this implementation pattern:

```csharp
public record [ValueObjectName]
{
    private [ValueObjectName]([PrimitiveType] value)
    {
        Value = value;
    }

    public [PrimitiveType] Value { get; }

    public static [ValueObjectName] Create([PrimitiveType] value)
    {
        // Validation logic
        if (!IsValid(value))
            throw new DomainException("[Error message]");
            
        return new [ValueObjectName](value);
    }
    
    // Additional factory methods and behavior
}
```

## Identity Value Objects

### UserAccountId

```csharp
public record UserAccountId
{
    private UserAccountId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static UserAccountId Create(Guid id)
    {
        if (id == Guid.Empty)
            throw new DomainException("User ID cannot be empty");
            
        return new UserAccountId(id);
    }

    public static UserAccountId CreateUnique()
    {
        return new UserAccountId(Guid.NewGuid());
    }
}
```

**Usage**: Identifies a UserAccount entity

**Validation Rules**:
- Cannot be an empty GUID

### RoleId

```csharp
public record RoleId
{
    private RoleId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static RoleId Create(Guid id)
    {
        if (id == Guid.Empty)
            throw new DomainException("Role ID cannot be empty");
            
        return new RoleId(id);
    }

    public static RoleId CreateUnique()
    {
        return new RoleId(Guid.NewGuid());
    }
}
```

**Usage**: Identifies an ApplicationRole entity

**Validation Rules**:
- Cannot be an empty GUID

### PermissionId

```csharp
public record PermissionId
{
    private PermissionId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static PermissionId Create(Guid id)
    {
        if (id == Guid.Empty)
            throw new DomainException("Permission ID cannot be empty");
            
        return new PermissionId(id);
    }

    public static PermissionId CreateUnique()
    {
        return new PermissionId(Guid.NewGuid());
    }
}
```

**Usage**: Identifies a Permission entity

**Validation Rules**:
- Cannot be an empty GUID

## User-Related Value Objects

### Username

```csharp
public record Username
{
    private Username(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Username Create(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new DomainException("Username cannot be empty");
            
        if (username.Length < 3)
            throw new DomainException("Username must be at least 3 characters");
            
        if (username.Length > 50)
            throw new DomainException("Username cannot exceed 50 characters");
            
        // Additional validation (e.g., allowed characters)
        if (!IsValidUsernameFormat(username))
            throw new DomainException("Username contains invalid characters");
            
        return new Username(username);
    }
    
    private static bool IsValidUsernameFormat(string username)
    {
        // Regex or other validation logic
        return System.Text.RegularExpressions.Regex.IsMatch(
            username, 
            @"^[a-zA-Z0-9_\-\.]+$");
    }
}
```

**Usage**: Represents a user's login name

**Validation Rules**:
- Cannot be null or empty
- Minimum length of 3 characters
- Maximum length of 50 characters
- Contains only alphanumeric characters, underscores, hyphens, and periods

### Email

```csharp
public record Email
{
    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty");
            
        if (email.Length > 320)
            throw new DomainException("Email is too long");
            
        if (!IsValidEmailFormat(email))
            throw new DomainException("Email format is invalid");
            
        return new Email(email);
    }
    
    private static bool IsValidEmailFormat(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    public string GetDomain()
    {
        return Value.Split('@')[1];
    }
    
    public string GetLocalPart()
    {
        return Value.Split('@')[0];
    }
}
```

**Usage**: Represents a user's email address

**Validation Rules**:
- Cannot be null or empty
- Maximum length of 320 characters
- Must be in valid email format (local-part@domain)

### PersonName

```csharp
public record PersonName
{
    private PersonName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; }
    public string LastName { get; }
    
    public string FullName => $"{FirstName} {LastName}";

    public static PersonName Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name cannot be empty");
            
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name cannot be empty");
            
        if (firstName.Length > 50)
            throw new DomainException("First name is too long");
            
        if (lastName.Length > 50)
            throw new DomainException("Last name is too long");
            
        return new PersonName(firstName, lastName);
    }
}
```

**Usage**: Represents a person's name with first and last components

**Validation Rules**:
- First name cannot be null or empty
- Last name cannot be null or empty
- Maximum length of 50 characters for first name
- Maximum length of 50 characters for last name

### UserStatus

```csharp
public record UserStatus
{
    private UserStatus(string value)
    {
        Value = value;
    }

    public string Value { get; }
    
    public static UserStatus Active => new UserStatus("Active");
    public static UserStatus Inactive => new UserStatus("Inactive");
    public static UserStatus Locked => new UserStatus("Locked");
    public static UserStatus PendingActivation => new UserStatus("PendingActivation");
    
    public static UserStatus Create(string status)
    {
        var normalizedStatus = status.Trim();
        
        return normalizedStatus switch
        {
            "Active" => Active,
            "Inactive" => Inactive,
            "Locked" => Locked,
            "PendingActivation" => PendingActivation,
            _ => throw new DomainException($"Invalid user status: {status}")
        };
    }
    
    public bool IsActive => Value == Active.Value;
    public bool IsInactive => Value == Inactive.Value;
    public bool IsLocked => Value == Locked.Value;
    public bool IsPendingActivation => Value == PendingActivation.Value;
}
```

**Usage**: Represents the status of a user account

**Validation Rules**:
- Must be one of the predefined statuses: Active, Inactive, Locked, PendingActivation

### AuthenticationProvider

```csharp
public record AuthenticationProvider
{
    private AuthenticationProvider(string value)
    {
        Value = value;
    }

    public string Value { get; }
    
    public static AuthenticationProvider EFCoreIdentity => new AuthenticationProvider("EFCoreIdentity");
    public static AuthenticationProvider MicrosoftEntraID => new AuthenticationProvider("MicrosoftEntraID");
    
    public static AuthenticationProvider Create(string provider)
    {
        var normalizedProvider = provider.Trim();
        
        return normalizedProvider switch
        {
            "EFCoreIdentity" => EFCoreIdentity,
            "MicrosoftEntraID" => MicrosoftEntraID,
            _ => throw new DomainException($"Invalid authentication provider: {provider}")
        };
    }
    
    public bool IsEFCoreIdentity => Value == EFCoreIdentity.Value;
    public bool IsMicrosoftEntraID => Value == MicrosoftEntraID.Value;
}
```

**Usage**: Represents the authentication provider for a user account

**Validation Rules**:
- Must be one of the predefined providers: EFCoreIdentity, MicrosoftEntraID

## Role and Permission Value Objects

### RoleName

```csharp
public record RoleName
{
    private RoleName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static RoleName Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Role name cannot be empty");
            
        if (name.Length > 50)
            throw new DomainException("Role name cannot exceed 50 characters");
            
        return new RoleName(name);
    }
}
```

**Usage**: Represents the name of a role

**Validation Rules**:
- Cannot be null or empty
- Maximum length of 50 characters

### PermissionName

```csharp
public record PermissionName
{
    private PermissionName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static PermissionName Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Permission name cannot be empty");
            
        if (name.Length > 100)
            throw new DomainException("Permission name cannot exceed 100 characters");
            
        return new PermissionName(name);
    }
}
```

**Usage**: Represents the name of a permission

**Validation Rules**:
- Cannot be null or empty
- Maximum length of 100 characters

### PermissionType

```csharp
public record PermissionType
{
    private PermissionType(string value)
    {
        Value = value;
    }

    public string Value { get; }
    
    public static PermissionType Create => new PermissionType("Create");
    public static PermissionType Read => new PermissionType("Read");
    public static PermissionType Update => new PermissionType("Update");
    public static PermissionType Delete => new PermissionType("Delete");
    public static PermissionType Execute => new PermissionType("Execute");
    
    public static PermissionType Create(string type)
    {
        var normalizedType = type.Trim();
        
        return normalizedType switch
        {
            "Create" => Create,
            "Read" => Read,
            "Update" => Update,
            "Delete" => Delete,
            "Execute" => Execute,
            _ => throw new DomainException($"Invalid permission type: {type}")
        };
    }
}
```

**Usage**: Represents the type of a permission (CRUD + Execute)

**Validation Rules**:
- Must be one of the predefined types: Create, Read, Update, Delete, Execute

### ResourceName

```csharp
public record ResourceName
{
    private ResourceName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static ResourceName Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Resource name cannot be empty");
            
        if (name.Length > 100)
            throw new DomainException("Resource name cannot exceed 100 characters");
            
        return new ResourceName(name);
    }
    
    public static ResourceName Global => new ResourceName("*");
    
    public bool IsGlobal => Value == Global.Value;
}
```

**Usage**: Represents the resource a permission applies to

**Validation Rules**:
- Cannot be null or empty
- Maximum length of 100 characters
- Special value "*" represents global scope

## Other Value Objects

### Description

```csharp
public record Description
{
    private Description(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Description Create(string description)
    {
        if (description == null)
            throw new DomainException("Description cannot be null");
            
        if (description.Length > 500)
            throw new DomainException("Description cannot exceed 500 characters");
            
        return new Description(description);
    }
    
    public static Description Empty => new Description(string.Empty);
}
```

**Usage**: Represents a description for various entities

**Validation Rules**:
- Cannot be null (but can be empty)
- Maximum length of 500 characters

### CreatedAt

```csharp
public record CreatedAt
{
    private CreatedAt(DateTime value)
    {
        Value = value;
    }

    public DateTime Value { get; }

    public static CreatedAt Create(DateTime dateTime)
    {
        if (dateTime == default)
            throw new DomainException("Created date cannot be default");
            
        if (dateTime > DateTime.UtcNow)
            throw new DomainException("Created date cannot be in the future");
            
        return new CreatedAt(dateTime);
    }
    
    public static CreatedAt Now()
    {
        return new CreatedAt(DateTime.UtcNow);
    }
}
```

**Usage**: Represents when an entity was created

**Validation Rules**:
- Cannot be default DateTime
- Cannot be in the future
