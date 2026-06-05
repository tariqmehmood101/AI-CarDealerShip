namespace CarDealership.Api.Models;

public sealed record CreateTenantRequest(
    string Slug,
    string LegalName
);

public sealed record CreateTenantResponse(
    Guid TenantId,
    string Slug,
    string LegalName,
    string Status,
    DateTime CreatedAtUtc,
    Guid SandboxEnvironmentId,
    Guid ProductionEnvironmentId
);

public sealed record TenantCurrentResponse(
    Guid TenantId,
    string Slug,
    string LegalName,
    string Status
);
