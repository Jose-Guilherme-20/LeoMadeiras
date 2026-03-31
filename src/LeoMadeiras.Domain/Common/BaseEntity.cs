namespace LeoMadeiras.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }

        public void SetCreatedAt() => CreatedAt = DateTime.UtcNow;
        public void SetUpdatedAt() => UpdatedAt = DateTime.UtcNow;
    }
}
