using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Common.Extensions;
using WebApplication1.Core.Database.Identity;

namespace WebApplication1.DataAccess.EntityConfigurations.Identity;

public class AppUserClaimConfig: IEntityTypeConfiguration<AppUserClaim>
{
    private readonly string _schema;
    public AppUserClaimConfig(string schema = "dbo")
    {
        _schema = schema;
    }

    public void Configure(EntityTypeBuilder<AppUserClaim> builder)
    {
        builder.ToTable("AppUserClaims", _schema);
        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasDefaultValue(Guid.NewGuid().V7());

        builder.HasQueryFilter(e => e.IsDeleted != true);
        builder.Property(e => e.IsDeleted).IsRequired()
            .HasDefaultValueSql("((0))");
        
        builder.Property(e => e.CreatedDate)
            .HasColumnType("datetime")
            .ValueGeneratedOnAdd();
        
        builder.Property(e => e.ModifiedBy)
            .HasColumnType("datetime")
            .ValueGeneratedOnUpdate();
        
        
        builder.HasIndex(e => e.IsDeleted);
        builder.Property(e => e.DeletedDate).HasColumnType("datetime")
            .HasValueGenerator<DeletedDateTimeValueGenerator>();

      
        builder.Property(e => e.ClaimType).HasMaxLength(2000);
        builder.Property(e => e.ClaimValue).HasMaxLength(1000);

        builder.HasIndex(e => e.UserId);
    }
}