using System.ComponentModel.DataAnnotations;

namespace Domain.Shared
{
    public abstract class BaseEntity
    {
        protected BaseEntity(Guid id)
        {
            CreatedAt = DateTime.UtcNow;
            Id = id;
        }

        protected BaseEntity() { }

        [Key]
        public Guid Id { get; private set; }

        public DateTime CreatedAt { get; set; }
        public int Order { get; set; }
    }
}
