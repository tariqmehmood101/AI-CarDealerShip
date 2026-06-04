using CarDealership.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealership.Api.Persistence.Configurations;

public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(ur => ur.Id);

        builder.Property(ur => ur.TenantId)
            .IsRequired();

        builder.Property(ur => ur.UserId)
            .IsRequired();

        builder.Property(ur => ur.RoleId)
            .IsRequired();

        builder.Property(ur => ur.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(ur => ur.UpdatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(ur => ur.DeletedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(ur => ur.IsDeleted)
            .HasDefaultValue(false);

        // Constraints and indexes
        builder.HasIndex(ur => new { ur.TenantId, ur.UserId, ur.RoleId })
            .IsUnique()
            .HasDatabaseName("IX_UserRole_TenantId_UserId_RoleId_Unique");

        builder.HasIndex(ur => ur.IsDeleted)
            .HasDatabaseName("IX_UserRole_IsDeleted");

        // Foreign keys
        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(ur => ur.TenantId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.User)
            .WithMany()
            .HasForeignKey(ur => ur.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Role)
            .WithMany()
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
