using CarDealership.Api.Shared.Domain;
using CarDealership.Api.Shared.Common;

namespace CarDealership.Api.Persistence.Entities;

public sealed class TenantSettings : BaseEntity
{
    public Guid TenantId { get; set; }
    public TenantEnvironment Environment { get; set; }
    public string LegalName { get; set; } = string.Empty;
    public string Dba { get; set; } = string.Empty;
    public string DealerLicenseNumber { get; set; } = string.Empty;
    public string PhysicalAddress { get; set; } = string.Empty;
    public string MailingAddress { get; set; } = string.Empty;
    public string County { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string AuthorizedSigner { get; set; } = string.Empty;
    public string SignerTitle { get; set; } = string.Empty;
    public string LogoReference { get; set; } = string.Empty;
    public bool FeatureBillingEnabled { get; set; }
    public bool FeatureDocuSignEnabled { get; set; }
    public bool FeatureInStoreSignaturesEnabled { get; set; }
    public bool FeatureFutureAIEnabled { get; set; }
    public bool FeatureDmvSubmissionEnabled { get; set; }
}
