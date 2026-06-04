using CarDealership.Api.Shared.Domain;
using CarDealership.Api.Shared.Common;

namespace CarDealership.Api.Persistence.Entities;

public sealed class Tenant : BaseEntity
{
    public string Slug { get; private set; }
    public string LegalName { get; private set; }
    public string Status { get; private set; }

    private Tenant(string slug, string legalName, string status)
    {
        Slug = slug;
        LegalName = legalName;
        Status = status;
    }

    public static Tenant Create(string slug, string legalName)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("Tenant slug is required.", nameof(slug));
        }

        if (string.IsNullOrWhiteSpace(legalName))
        {
            throw new ArgumentException("Tenant legal name is required.", nameof(legalName));
        }

        return new Tenant(slug.Trim(), legalName.Trim(), "Active");
    }
}
