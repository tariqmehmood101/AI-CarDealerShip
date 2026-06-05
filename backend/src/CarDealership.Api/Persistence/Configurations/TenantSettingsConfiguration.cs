using CarDealership.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealership.Api.Persistence.Configurations;

public sealed class TenantSettingsConfiguration : IEntityTypeConfiguration<TenantSettings>
{
    public void Configure(EntityTypeBuilder<TenantSettings> builder)
    {
        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.TenantId)
            .IsRequired();

        builder.Property(ts => ts.Environment)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasColumnType("nvarchar(50)");

        builder.Property(ts => ts.LegalName)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.Property(ts => ts.Dba)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.Property(ts => ts.DealerLicenseNumber)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)");

        builder.Property(ts => ts.PhysicalAddress)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.Property(ts => ts.MailingAddress)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.Property(ts => ts.County)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)");

        builder.Property(ts => ts.Phone)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnType("nvarchar(20)");

        builder.Property(ts => ts.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnType("nvarchar(255)");

        builder.Property(ts => ts.Website)
            .HasMaxLength(255)
            .HasColumnType("nvarchar(255)");

        builder.Property(ts => ts.AuthorizedSigner)
            .HasMaxLength(255)
            .HasColumnType("nvarchar(255)");

        builder.Property(ts => ts.SignerTitle)
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)");

        builder.Property(ts => ts.LogoReference)
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.Property(ts => ts.FeatureBillingEnabled)
            .HasDefaultValue(false);

        builder.Property(ts => ts.FeatureDocuSignEnabled)
            .HasDefaultValue(false);

        builder.Property(ts => ts.FeatureInStoreSignaturesEnabled)
            .HasDefaultValue(false);

        builder.Property(ts => ts.FeatureFutureAIEnabled)
            .HasDefaultValue(false);

        builder.Property(ts => ts.FeatureDmvSubmissionEnabled)
            .HasDefaultValue(false);

        builder.Property(ts => ts.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(ts => ts.UpdatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(ts => ts.DeletedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(ts => ts.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .ValueGeneratedNever();

        // Constraints and indexes
        builder.HasIndex(ts => new { ts.TenantId, ts.Environment })
            .IsUnique()
            .HasDatabaseName("IX_TenantSettings_TenantId_Environment_Unique");

        builder.HasIndex(ts => ts.IsDeleted)
            .HasDatabaseName("IX_TenantSettings_IsDeleted");

        // Foreign key to Tenant
        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(ts => ts.TenantId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
