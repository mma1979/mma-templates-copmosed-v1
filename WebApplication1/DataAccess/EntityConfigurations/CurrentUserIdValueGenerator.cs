using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace WebApplication1.DataAccess.EntityConfigurations;

public class CurrentUserIdValueGenerator(IHttpContextAccessor accessor) : ValueGenerator<Guid>
{
    private readonly IHttpContextAccessor _accessor = accessor;

    public override Guid Next(EntityEntry entry)
    {
        return Guid.Parse(CurrentUserId());
    }

    public override bool GeneratesTemporaryValues => false;

    private string CurrentUserId()
    {
        return "123654-852-789-6dfdfd4545";
    }
}