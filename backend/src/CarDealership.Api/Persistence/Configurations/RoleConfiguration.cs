using CarDealership.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealership.Api.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.TenantId)
            .IsRequired();

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnType("nvarchar(255)");

        builder.Property(r => r.Description)
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.Property(r => r.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(r => r.UpdatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(r => r.DeletedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(r => r.IsDeleted)
            .HasDefaultValue(false);

        // Constraints and indexes
        builder.HasIndex(r => new { r.TenantId, r.Name })
            .IsUnique()
            .HasDatabaseName("IX_Role_TenantId_Name_Unique");

        builder.HasIndex(r => r.IsDeleted)
            .HasDatabaseName("IX_Role_IsDeleted");

        // Foreign key to Tenant
        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(r => r.TenantId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Relationships
        builder.HasMany<UserRole>()
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
