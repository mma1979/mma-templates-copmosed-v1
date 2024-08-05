using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Core.Database.Identity;
using WebApplication1.Common.Extensions;

namespace WebApplication1.DataAccess.EntityConfigurations.Identity;


    public class AppRolesConfig : IEntityTypeConfiguration<AppRole>
    {
        private readonly string _schema;
        public AppRolesConfig(string schema = "dbo")
        {
            _schema = schema;
        }


        public void Configure(EntityTypeBuilder<AppRole> builder)
        {
            builder.ToTable("AppRoles", _schema);

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

            builder.HasMany(e => e.AppUserRoles)
                .WithOne(e => e.AppRole)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.Name).HasMaxLength(100);
            builder.Property(e => e.NormalizedName).HasMaxLength(100);
            builder.Property(e => e.ConcurrencyStamp).HasMaxLength(1000);

        }
    }
