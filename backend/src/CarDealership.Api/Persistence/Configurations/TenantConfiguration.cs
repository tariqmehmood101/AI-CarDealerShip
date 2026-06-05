using CarDealership.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealership.Api.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnType("nvarchar(255)");

        builder.Property(t => t.LegalName)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("nvarchar(50)");

        builder.Property(t => t.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(t => t.UpdatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(t => t.DeletedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .ValueGeneratedNever();

        // Constraints and indexes
        builder.HasIndex(t => t.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Tenant_Slug_Unique");

        builder.HasIndex(t => t.IsDeleted)
            .HasDatabaseName("IX_Tenant_IsDeleted");

        // Relationships
        builder.HasMany<TenantEnvironmentRecord>()
            .WithOne()
            .HasForeignKey(ter => ter.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<TenantSettings>()
            .WithOne()
            .HasForeignKey(ts => ts.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<AppUser>()
            .WithOne()
            .HasForeignKey(au => au.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<Role>()
            .WithOne()
            .HasForeignKey(r => r.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<UserRole>()
            .WithOne()
            .HasForeignKey(ur => ur.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
