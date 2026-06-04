using CarDealership.Api.Shared.Domain;

namespace CarDealership.Api.Persistence.Entities;

public sealed class UserRole : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public AppUser? User { get; set; }
    public Role? Role { get; set; }
}
