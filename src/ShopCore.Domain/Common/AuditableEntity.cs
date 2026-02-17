namespace ShopCore.Domain.Common;

/// <summary>
/// Base class for entities that require audit tracking
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    /// <summary>
    /// When the entity was created (UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// User ID who created the entity (nullable for system-created)
    /// </summary>
    public int? CreatedBy { get; set; }

    /// <summary>
    /// When the entity was last updated (UTC)
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// User ID who last updated the entity
    /// </summary>
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// When the entity was last deleted (UTC)
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Soft delete flag - if true, entity is considered deleted
    /// </summary>
    public bool IsDeleted { get; set; }
}
