using System.ComponentModel.DataAnnotations;

namespace Domain.Shared
{
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; private set; }

        public DateTime CreatedAt { get; private set; }
    }
}
