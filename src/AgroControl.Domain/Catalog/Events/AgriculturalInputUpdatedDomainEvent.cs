using AgroControl.Domain.Common;

namespace AgroControl.Domain.Catalog.Events;

public sealed record AgriculturalInputUpdatedDomainEvent(
    Guid AgriculturalInputId,
    string Name) : DomainEvent;
