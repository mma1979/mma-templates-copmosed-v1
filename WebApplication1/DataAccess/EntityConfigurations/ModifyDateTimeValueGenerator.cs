using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace WebApplication1.DataAccess.EntityConfigurations;

public class ModifyDateTimeValueGenerator : ValueGenerator<DateTime?>
{
    public override DateTime? Next(EntityEntry entry)
    {
        if (entry.State == EntityState.Modified && entry.Property("IsDeleted").CurrentValue as bool? != true)
        {
            return DateTime.UtcNow;
        }
        return null;
    }

    public override bool GeneratesTemporaryValues => false;
}