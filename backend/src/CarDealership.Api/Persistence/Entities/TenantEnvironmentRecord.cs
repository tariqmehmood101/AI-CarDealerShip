using CarDealership.Api.Shared.Domain;
using CarDealership.Api.Shared.Common;

namespace CarDealership.Api.Persistence.Entities;

public sealed class TenantEnvironmentRecord : BaseEntity
{
    public Guid TenantId { get; set; }
    public TenantEnvironment Environment { get; set; }
    public string? Description { get; set; }
}
