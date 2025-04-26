# Domain Model Overview

## Introduction

This document provides an overview of the domain model for the Clean Architecture Template with Authentication. The domain model is designed following Domain-Driven Design (DDD) principles, with a focus on creating a rich domain model that encapsulates business logic and enforces invariants.

The domain model is the core of the application, residing in the innermost layer of the Clean Architecture. It contains entities, value objects, domain events, and domain services that represent the business concepts and rules of the application.

## Key Design Principles

### Rich Domain Model

We've implemented a rich domain model rather than an anemic one. This means:

- Business logic is encapsulated within domain entities and value objects
- Entities enforce their own invariants and business rules
- Domain objects have behavior, not just data
- State changes are controlled through methods that enforce business rules

### Value Objects for Primitives

Instead of using primitive types directly, we use value objects to represent domain concepts:

- Each primitive type (string, int, Guid, etc.) that has domain meaning has a corresponding value object
- Value objects are immutable and enforce validation rules
- Value objects encapsulate domain-specific behavior related to the concept they represent
- This approach prevents primitive obsession and makes the code more expressive

### Factory Methods for Creation

Entity and value object creation follows a consistent pattern:

- Private or protected constructors to prevent direct instantiation
- Static factory methods (Create, From, etc.) that validate inputs and create valid instances
- Factory methods that raise appropriate domain events upon creation
- Creation logic centralized in factory methods for consistency

### Encapsulation of State

The domain model strictly encapsulates its state:

- Properties have private setters or are read-only
- State changes occur through methods that enforce business rules
- Methods raise domain events when significant state changes occur
- Invariants are checked before state changes are committed

### Aggregate Boundaries

The domain model defines clear aggregate boundaries:

- Each aggregate has a root entity that controls access to the aggregate
- References across aggregate boundaries are by ID only, not direct object references
- Consistency is enforced within aggregate boundaries
- Transactions align with aggregate boundaries

### Domain Events

The domain model uses events to communicate significant state changes:

- Events are raised when important domain actions occur
- Events are named in past tense (UserCreated, EmailChanged)
- Events contain relevant data about what happened
- Events are used for cross-aggregate communication

## Implementation Patterns

### Value Object Pattern

```csharp
public record UserId
{
    private UserId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static UserId Create(Guid id)
    {
        if (id == Guid.Empty)
            throw new DomainException("User ID cannot be empty");
            
        return new UserId(id);
    }

    public static UserId CreateUnique()
    {
        return new UserId(Guid.NewGuid());
    }
}
```

### Entity Pattern

```csharp
public class UserAccount
{
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
        CreatedAt = DateTime.UtcNow;
    }

    public UserAccountId Id { get; }
    public Username Username { get; }
    public Email Email { get; private set; }
    public PersonName Name { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? LastLoggedIn { get; private set; }
    public AuthenticationProvider AuthenticationProvider { get; }

    public static UserAccount Create(
        Username username,
        Email email,
        PersonName name,
        AuthenticationProvider authProvider)
    {
        var id = UserAccountId.CreateUnique();
        var status = UserStatus.Active;
        
        var user = new UserAccount(id, username, email, name, status, authProvider);
        
        // Raise domain event
        user._events.Add(new UserAccountCreatedEvent(user.Id, username, email, authProvider));
        
        return user;
    }

    public Result UpdateEmail(Email newEmail)
    {
        // Business logic and validation
        Email = newEmail;
        
        // Raise domain event
        _events.Add(new UserEmailChangedEvent(Id, Email, newEmail));
        
        return Result.Success();
    }

    // Other methods...
}
```

## Navigation

The domain model documentation is organized into the following sections:

- **[Value Objects](value-objects.md)**: Documentation of all value objects in the domain model
- **[Entities](entities.md)**: Documentation of all entities in the domain model
- **[Aggregates](aggregates.md)**: Documentation of aggregate boundaries and rules
- **[Domain Events](domain-events.md)**: Documentation of domain events
- **[Domain Services](domain-services.md)**: Documentation of domain services
- **[Diagrams](diagrams.md)**: Visual representations of the domain model

Each section provides detailed information about the corresponding aspect of the domain model, including purpose, implementation, and usage examples.
