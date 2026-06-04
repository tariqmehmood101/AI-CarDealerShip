using CarDealership.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealership.Api.Persistence.Configurations;

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(au => au.Id);

        builder.Property(au => au.TenantId)
            .IsRequired();

        builder.Property(au => au.FirstName)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnType("nvarchar(255)");

        builder.Property(au => au.LastName)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnType("nvarchar(255)");

        builder.Property(au => au.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnType("nvarchar(255)");

        builder.Property(au => au.IsVerified)
            .HasDefaultValue(false);

        builder.Property(au => au.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(au => au.UpdatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(au => au.DeletedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(au => au.IsDeleted)
            .HasDefaultValue(false);

        // Constraints and indexes
        builder.HasIndex(au => new { au.TenantId, au.Email })
            .IsUnique()
            .HasDatabaseName("IX_AppUser_TenantId_Email_Unique");

        builder.HasIndex(au => au.IsDeleted)
            .HasDatabaseName("IX_AppUser_IsDeleted");

        // Foreign key to Tenant
        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(au => au.TenantId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Relationships
        builder.HasMany<UserRole>()
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
