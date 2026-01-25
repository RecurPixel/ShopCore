namespace ShopCore.Domain.Common;

/// <summary>
/// Base class for entities with integer primary key
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
}
