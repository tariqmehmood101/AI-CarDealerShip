using CarDealership.Api.Shared.Domain;

namespace CarDealership.Api.Persistence.Entities;

public sealed class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
