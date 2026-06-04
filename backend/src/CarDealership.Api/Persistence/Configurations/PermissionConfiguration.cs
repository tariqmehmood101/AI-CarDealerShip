using CarDealership.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealership.Api.Persistence.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnType("nvarchar(255)");

        builder.Property(p => p.Description)
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.Property(p => p.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(p => p.UpdatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(p => p.DeletedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(p => p.IsDeleted)
            .HasDefaultValue(false);

        // Constraints and indexes
        builder.HasIndex(p => p.Name)
            .IsUnique()
            .HasDatabaseName("IX_Permission_Name_Unique");

        builder.HasIndex(p => p.IsDeleted)
            .HasDatabaseName("IX_Permission_IsDeleted");
    }
}
