namespace Travaloud.Domain.Catalog.Galleries;

public class Gallery : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }

    public virtual IEnumerable<GalleryImage> GalleryImages { get; set; } = default!;
}