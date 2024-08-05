using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Core.Database.Identity;

namespace WebApplication1.DataAccess.EntityConfigurations.Identity;

public class AppUserLoginConfig: IEntityTypeConfiguration<AppUserLogin>
{
    private readonly string _schema;

    public AppUserLoginConfig(string schema="dbo")
    {
        _schema = schema;
    }

    public void Configure(EntityTypeBuilder<AppUserLogin> builder)
    {
        builder.ToTable("AppUserLogins", _schema);

        builder.HasQueryFilter(e => e.IsDeleted != true);
        builder.Property(e => e.IsDeleted).IsRequired()
            .HasDefaultValueSql("((0))");
        
        builder.Property(e => e.CreatedDate)
            .HasColumnType("datetime")
            .ValueGeneratedOnAdd();
        
        builder.Property(e => e.ModifiedDate)
            .HasColumnType("datetime")
            .ValueGeneratedOnUpdate();
        
        
        builder.HasIndex(e => e.IsDeleted);
        builder.Property(e => e.DeletedDate).HasColumnType("datetime")
            .HasValueGenerator<DeletedDateTimeValueGenerator>();

      
        builder.Property(e => e.LoginProvider).HasMaxLength(500);
        builder.Property(e => e.ProviderDisplayName).HasMaxLength(500);
        builder.Property(e => e.ProviderKey).HasMaxLength(2000);

        builder.HasIndex(e => e.UserId);
    }
}