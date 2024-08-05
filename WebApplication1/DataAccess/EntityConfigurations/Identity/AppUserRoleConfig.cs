using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Core.Database.Identity;

namespace WebApplication1.DataAccess.EntityConfigurations.Identity;

public class AppUserRoleConfig: IEntityTypeConfiguration<AppUserRole>
{
    private readonly string _schema;
    public AppUserRoleConfig(string schema = "dbo")
    {
        _schema = schema;
    }

    public void Configure(EntityTypeBuilder<AppUserRole> builder)
    {
        builder.ToTable("AppUserRoles", _schema);
       

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

        builder.HasIndex(e => e.UserId);
    }
}