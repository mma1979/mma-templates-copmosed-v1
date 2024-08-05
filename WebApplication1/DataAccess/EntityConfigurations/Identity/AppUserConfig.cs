using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NodaTime;
using WebApplication1.Core.Database.Identity;

namespace WebApplication1.DataAccess.EntityConfigurations.Identity;

public class AppUserConfig:IEntityTypeConfiguration<AppUser>
{
    private readonly string _schema;

    public AppUserConfig(string schema="dbo")
    {
        _schema = schema;
    }

    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("AppUsers", _schema);
            builder.HasMany(x => x.AppUserRoles).WithOne(x => x.AppUser).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);


            builder.HasQueryFilter(e => e.IsDeleted != true);
            builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValueSql("((0))");
            
            builder.Property(e => e.CreatedDate).HasColumnType("datetime")
                .ValueGeneratedOnAdd();
            
            builder.Property(e => e.ModifiedDate).HasColumnType("datetime")
                .ValueGeneratedOnUpdate();
            
            builder.HasIndex(e => e.IsDeleted);
            builder.Property(e => e.DeletedDate).HasColumnType("datetime")
                .HasValueGenerator<DeletedDateTimeValueGenerator>();


            builder.HasMany(e => e.AppUserRoles)
                    .WithOne(e => e.AppUser)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.AppUserTokens)
                .WithOne(e => e.AppUser)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(navigationExpression: e => e.AppRefreshTokens)
               .WithOne(e => e.AppUser)
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Cascade);


            builder.Property(propertyExpression: e => e.UserName)
              .HasMaxLength(100);
            builder.Property(e => e.NormalizedUserName)
              .HasMaxLength(100);
            builder.Property(propertyExpression: e => e.Email)
              .HasMaxLength(100);
            builder.Property(e => e.NormalizedEmail)
              .HasMaxLength(maxLength: 100);

            builder.Property(e => e.PasswordHash)
           .HasMaxLength(maxLength: 1000);

            builder.Property(e => e.SecurityStamp)
           .HasMaxLength(maxLength: 1000);
            builder.Property(e => e.ConcurrencyStamp)
           .HasMaxLength(maxLength: 1000);

            builder.Property(e => e.PhoneNumber)
           .HasMaxLength(maxLength: 20);
    }
}


