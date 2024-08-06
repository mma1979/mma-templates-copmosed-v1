using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Core.Database.Identity;
using WebApplication1.Common.Extensions;

namespace WebApplication1.DataAccess.EntityConfigurations.Identity;

public class AppRefreshTokenConfig : IEntityTypeConfiguration<AppRefreshToken>
{
    private readonly string _schema;
    public AppRefreshTokenConfig(string schema = "dbo")
    {
        _schema = schema;
    }


    public void Configure(EntityTypeBuilder<AppRefreshToken> builder)
    {
        builder.ToTable("AppRefreshTokens", _schema);
        builder.Property(e => e.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();

        builder.HasQueryFilter(e => e.IsDeleted != true);
        builder.Property(e => e.IsDeleted).IsRequired()
            .HasDefaultValueSql("((0))");
        
        builder.Property(e => e.CreatedDate)
            .HasColumnType("datetime")
            .ValueGeneratedOnAdd();
        
        builder.Property(e => e.ModifiedDate)
            .HasColumnType("datetime")
            .HasValueGenerator<ModifyDateTimeValueGenerator>()
            .ValueGeneratedOnUpdateSometimes();
        
        
        builder.HasIndex(e => e.IsDeleted);
        builder.Property(e => e.DeletedDate).HasColumnType("datetime")
            .ValueGeneratedOnUpdateSometimes()
            .HasValueGenerator<DeletedDateTimeValueGenerator>();

        builder.HasIndex(e => e.Hash);

        builder.Property(e => e.Token).HasMaxLength(2000);
        builder.Property(e => e.JwtId).HasMaxLength(1000);

        builder.HasIndex(e => e.UserId);


    }
}