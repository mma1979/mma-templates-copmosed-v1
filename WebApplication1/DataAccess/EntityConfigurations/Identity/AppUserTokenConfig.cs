using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Common.Extensions;
using WebApplication1.Core.Database.Identity;

namespace WebApplication1.DataAccess.EntityConfigurations.Identity;

public class AppUserTokenConfig: IEntityTypeConfiguration<AppUserToken>
{
    private readonly string _schema;
    public AppUserTokenConfig(string schema = "dbo")
    {
        _schema = schema;
    }


    public void Configure(EntityTypeBuilder<AppUserToken> builder)
    {
        builder.ToTable("AppUserTokens", _schema);


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


        builder.Property(e => e.Name).HasMaxLength(100);

    }
}