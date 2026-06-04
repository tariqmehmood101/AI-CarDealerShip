using CarDealership.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealership.Api.Persistence.Configurations;

public sealed class TenantEnvironmentRecordConfiguration : IEntityTypeConfiguration<TenantEnvironmentRecord>
{
    public void Configure(EntityTypeBuilder<TenantEnvironmentRecord> builder)
    {
        builder.HasKey(ter => ter.Id);

        builder.Property(ter => ter.TenantId)
            .IsRequired();

        builder.Property(ter => ter.Environment)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasColumnType("nvarchar(50)");

        builder.Property(ter => ter.Description)
            .HasMaxLength(1000)
            .HasColumnType("nvarchar(1000)");

        builder.Property(ter => ter.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(ter => ter.UpdatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(ter => ter.DeletedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(ter => ter.IsDeleted)
            .HasDefaultValue(false);

        // Constraints and indexes
        builder.HasIndex(ter => new { ter.TenantId, ter.Environment })
            .IsUnique()
            .HasDatabaseName("IX_TenantEnvironmentRecord_TenantId_Environment_Unique");

        builder.HasIndex(ter => ter.IsDeleted)
            .HasDatabaseName("IX_TenantEnvironmentRecord_IsDeleted");

        // Foreign key to Tenant
        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(ter => ter.TenantId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
