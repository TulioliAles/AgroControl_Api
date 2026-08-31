using AgroControl.Domain.Common;

namespace AgroControl.Domain.Catalog.Events;

public sealed record AgriculturalInputCreatedDomainEvent(
    Guid AgriculturalInputId,
    string Name) : DomainEvent;
