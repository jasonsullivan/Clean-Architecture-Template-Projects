using MediatR;

namespace CleanArchitectureTemplate.Shared.Primitives;

/// <summary>
/// Represents a domain event in the system.
/// </summary>
public interface IDomainEvent : INotification;
