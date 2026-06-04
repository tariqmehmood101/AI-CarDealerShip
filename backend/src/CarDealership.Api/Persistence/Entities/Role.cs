using CarDealership.Api.Shared.Domain;

namespace CarDealership.Api.Persistence.Entities;

public sealed class Role : BaseEntity
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
