using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using WebApplication1.Common.Extensions;

namespace WebApplication1.DataAccess.EntityConfigurations;

public class GuidV7ValueGenerator: ValueGenerator<Guid>
{
    public override Guid Next(EntityEntry entry)
    {
        return Guid.NewGuid().V7();
    }

    public override bool GeneratesTemporaryValues => false;
}