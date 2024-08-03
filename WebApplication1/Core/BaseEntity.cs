namespace WebApplication1.Core;

public class BaseEntity<T>
{
    public T Id { get; protected set; }
    public long? CreatedBy { get; protected set; }
    public DateTime? CreatedDate { get; protected set; }
    public long? ModifiedBy { get; protected set; }
    public DateTime? ModifiedDate { get; protected set; }
    public bool? IsDeleted { get; protected set; }
    public long? DeletedBy { get; protected set; }
    public DateTime? DeletedDate { get; protected set; }
}