using CarDealership.Api.Shared.Domain;

namespace CarDealership.Api.Persistence.Entities;

public sealed class AppUser : BaseEntity
{
    public Guid TenantId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
}
